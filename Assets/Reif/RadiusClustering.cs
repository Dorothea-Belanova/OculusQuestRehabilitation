using System.Collections.Generic;
using UnityEngine;

namespace ErrorCorrection
{
    public class RadiusClustering
    {
        private const float radiusMaxLimit = 0.025f; // 2.5cm
        private const float radiusMinLimit = 0.005f; // 0.5cm

        private float radius = 0f;
        private KalmanFilter kalmanFilter;
        private Vector3 clusterCenter;
        private float initialCameraDistance;

        /// <param name="leftHandInitialPositions">List of left hand positions during calibration.</param>
        /// <param name="righttHandInitialPositions">List of right hand positions during calibration.</param>
        public RadiusClustering(List<Vector3> leftHandInitialPositions, List<Vector3> rightHandInitialPositions)
        {
            // Calculates radius depending on all the hand positions during calibration
            CalculateRadius(leftHandInitialPositions);
            CalculateRadius(rightHandInitialPositions);

            // If radius is smaller or higher than limits, set closer limit
            if (radius < radiusMinLimit || radius > radiusMaxLimit)
            {
                radius = radius < radiusMinLimit ? radiusMinLimit : radiusMaxLimit;
            }
        }

        /// <summary>
        /// Calculates radius for initial hand positions.
        /// </summary>
        /// <param name="positions">List of positions representing hand position during calibration.</param>
        private void CalculateRadius(List<Vector3> positions)
        {
            Vector3 center = positions.GetAverage();

            // If distance between center and each position is smaller
            // than radius, set distance as radius
            for (int i = 0; i < positions.Count; ++i)
            {
                float distance = Vector3.Distance(center, positions[i]);
                if (radius < distance)
                {
                    radius = distance;
                }
            }
        }

        /// <summary>
        /// Updates the measurement for cluster center depending on current position.
        /// </summary>
        /// <returns>
        /// If current position belongs to previous cluster center, returns zero vector.
        /// If current position belongs to new cluster, returns previous cluster center.
        /// </returns>
        /// <param name="currentPosition">Current hand position obtained from LMC.</param>
        /// <param name="cameraDistance">Distance between main camera and current hand position.</param>
        public Vector3 UpdateMeasurement(Vector3 currentPosition, float cameraDistance)
        {
            // Initialization of Kalman Filter and calculation of initial camera distance 
            if (kalmanFilter == null)
            {
                kalmanFilter = new KalmanFilter(radius);
                clusterCenter = kalmanFilter.Filtrate(currentPosition);

                initialCameraDistance = cameraDistance;

                return new Vector3();
            }

            // Recalculation of radius, as the data is more scattered the further is hand
            // from the camera
            float currentRadius = (cameraDistance / initialCameraDistance) * radius;

            // Position belongs to the cluster, as its distance to the cluster center is
            // less than radius 
            if (Vector3.Distance(clusterCenter, currentPosition) <= currentRadius)
            {
                clusterCenter = kalmanFilter.Filtrate(currentPosition);

                return new Vector3();
            }
            // Position does not belong to cluster center, so we create new cluster and
            // return position of previous cluster center
            else
            {
                Vector3 previousClusterCenter = clusterCenter;

                kalmanFilter = new KalmanFilter(radius);
                clusterCenter = kalmanFilter.Filtrate(currentPosition);

                return previousClusterCenter;
            }
        }

        /// <summary>
        /// Returns cluster center of the current cluster.
        /// </summary>
        public Vector3 GetClusteredCenter()
        {
            return clusterCenter;
        }
    }
}