using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ErrorCorrection
{
    public class LowPassFilter : Filter
    {
        private Vector3 threshold; // Threshold for filtering current position depending on distance to previous position
        private Vector3 previousPosition; // Used for calculation of distance between previous and current position
        private LowPassApproach lowPassApproach; // Influences recalculation of threshold in each frame

        /// <param name="lowPassApproach">Influences recalculation of threshold in each frame.</param>
        public LowPassFilter(LowPassApproach lowPassApproach)
        {
            this.lowPassApproach = lowPassApproach;
        }

        /// <summary>
        /// Passes current position, if the distance between current position and previous
        /// position is smaller than threshold. If it is higher, it returns altered current position.
        /// </summary>
        /// <returns>
        /// Filtered current position.
        /// </returns>
        /// <param name="positions">List of positions representing hand position over time.</param>
        public override Vector3 Filtrate(List<Vector3> positions)
        {
            Vector3 currentPosition = positions[positions.Count - 1];

            // If threshold is not set
            if (threshold.Equals(Vector3.zero))
            {
                for (int i = 0; i < 3; ++i)
                {
                    // Sets threshold as double the distance of current position
                    // and previous position in each axis
                    threshold[i] = 2 * Mathf.Abs(currentPosition[i] - previousPosition[i]);
                }

                previousPosition = currentPosition;
                return currentPosition;
            }
            else
            {
                Vector3 newPosition = new Vector3();

                // Iterates through each axis
                for (int i = 0; i < 3; ++i)
                {
                    // Calculates difference between current position and past position
                    // in single axis
                    float distance = Mathf.Abs(currentPosition[i] - previousPosition[i]);

                    // If difference is smaller than threshold in single axis, value in
                    // this axis passes filter
                    if (distance < threshold[i])
                    {
                        newPosition[i] = currentPosition[i];
                    }
                    // If difference is smaller higher than threshold, value is altered
                    else
                    {
                        int multiplicator = (previousPosition[i] > currentPosition[i]) ? -1 : 1;
                        newPosition[i] = previousPosition[i] + multiplicator * threshold[i];
                    }

                    bool isIncreasing = (currentPosition[i] > previousPosition[i]);
                    RecalculateThreshold(distance, isIncreasing, i);
                }

                previousPosition = newPosition;
                return newPosition;
            }
        }

        /// <summary>
        /// Recalculates threshold to adjust it to decreasing or increasing distances in previous
        /// and current positions.
        /// </summary>
        /// <param name="distance">Distance between previous and current position in single axis.</param>
        /// <param name="isIncreasing">Marks if value in a single axis of current position is higher than in previous position.</param>
        /// <param name="i">Index of the axis in which to recalculate threshold.</param>
        private void RecalculateThreshold(float distance, bool isIncreasing, int i)
        {
            switch (lowPassApproach)
            {
                // Puts more weight to threshold value
                case LowPassApproach.EmphasisOnThreshold:
                    {
                        threshold[i] = (threshold[i] * 2 + distance) / 3;
                        break;
                    }
                // Puts more weight to distance
                case LowPassApproach.EmphasisOnDistance:
                    {
                        threshold[i] = (threshold[i] + distance * 2) / 3;
                        break;
                    }
                // Puts more weight to threshold when increasing
                // Puts more weight to distance when decreasing
                case LowPassApproach.CombinedPass:
                    {
                        if (isIncreasing)
                        {
                            threshold[i] = (threshold[i] + distance * 2) / 3;
                        }
                        else
                        {
                            threshold[i] = (threshold[i] * 2 + distance) / 3;
                        }
                        break;
                    }
            }
        }
    }

    public enum LowPassApproach
    {
        EmphasisOnThreshold,
        EmphasisOnDistance,
        CombinedPass
    }
}