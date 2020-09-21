using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ErrorCorrection
{
    public class KalmanFilter : Filter
    {
        private Vector3 xk = Vector3.zero; // Estimate of the hand position
        private Vector3 pk; // Prior error covariance
        private Vector3 Kk = Vector3.zero; // Kalman Gain

        const float R = 0.00009f; // Noise estimation

        /// <param name="variance">Used as error covariance for positions.</param>
        public KalmanFilter(float variance)
        {
            pk = new Vector3(1, 1, 1);
        }

        public Vector3 SetFirst(Vector3 position)
        {
            xk = position;
            return position;
        }

        /// <summary>
        /// Filtrates list of positions into single position.
        /// </summary>
        /// <returns>
        /// Recalculated estimated position.
        /// </returns>
        /// <param name="positions">List of positions representing hand position over time.</param>
        public override Vector3 Filtrate(List<Vector3> positions)
        {
            Vector3 zk = positions[positions.Count - 1]; // Measurement update - last position

            return Filtrate(zk);
        }

        /// <summary>
        /// Estimates hand position depending on current position.
        /// Based on: http://bilgin.esme.org/BitsAndBytes/KalmanFilterforDummies
        /// </summary>
        /// <returns>
        /// Recalculated estimated position.
        /// </returns>
        /// <param name="zk">Last measured hand position.</param>
        public Vector3 Filtrate(Vector3 zk)
        {
            Kk.x = pk.x / (pk.x + R);
            Kk.y = pk.y / (pk.y + R);
            Kk.z = pk.z / (pk.z + R);

            xk.x = xk.x + Kk.x * (zk.x - xk.x);
            xk.y = xk.y + Kk.y * (zk.y - xk.y);
            xk.z = xk.z + Kk.z * (zk.z - xk.z);

            pk.x = pk.x * (1 - Kk.x);
            pk.y = pk.y * (1 - Kk.y);
            pk.z = pk.z * (1 - Kk.z);

            return xk;
        }
    }
}
