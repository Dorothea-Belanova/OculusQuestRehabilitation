using System;
using System.Collections.Generic;
using UnityEngine;

namespace ErrorCorrection
{
    public class MedianFilter : Filter
    {
        private int n; // Number of positions from which to take median position
        private bool sortPositions; // Determines whether array is sorted before getting median


        /// <param name="n">Number of positions from which to take median.</param>
        /// <param name="sortPositions">Determines whether positions are sorted before getting median.</param>
        public MedianFilter(int n, bool sortPositions = false)
        {
            this.n = n;
            this.sortPositions = sortPositions;
        }

        /// <summary>
        /// Filtrates list of positions into single position.
        /// </summary>
        /// <returns>
        /// Median (middle value) of last n positions.
        /// </returns>
        /// <param name="positions">List of positions representing hand position over time.</param>
        public override Vector3 Filtrate(List<Vector3> positions)
        {
            // Creates list of last n positions
            var nPositions = positions.GetRange(positions.Count - n, n);

            float[] x = new float[n];
            float[] y = new float[n];
            float[] z = new float[n];

            // Creates array of x, y and z of values of last n positions
            for (int i = 0; i < n; ++i)
            {
                x[i] = nPositions[i].x;
                y[i] = nPositions[i].y;
                z[i] = nPositions[i].z;
            }

            return new Vector3(MedianValue(x), MedianValue(y), MedianValue(z));
        }

        /// <summary>
        /// Gets median value from an array.
        /// </summary>
        /// <param name="array">Array of last n values of positions in one dimension (x, y or z).</param>
        private float MedianValue(float[] array)
        {
            int middle = (n - 1) / 2; // Finds middle index of an array
            Debug.Log("middle: " + middle);
            Debug.Log("n: " + n);

            if (sortPositions)
                Array.Sort(array); // Sorts an array

            if (n % 2 != 0)
            {
                // If size of array is odd, returns value at middle index of an array
                return array[middle];
            }
            else
            {
                // If size of array is even, returns average value at middle index
                // and at middle index -1
                return (array[middle] + array[middle - 1]) / 2;
            }
        }
    }
}