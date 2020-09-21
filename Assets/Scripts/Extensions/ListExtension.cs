using System.Collections.Generic;
using UnityEngine;

public static class ListExtension {

    /// <summary>
    /// Returns average vector from list of vectors.
    /// </summary>
    public static Vector3 GetAverage(this List<Vector3> vectors) {
        Vector3 sum = Vector3.zero;

        for(int i = 0; i < vectors.Count; ++i) {
            sum += vectors[i];
        }

        return sum / vectors.Count;
    }

}