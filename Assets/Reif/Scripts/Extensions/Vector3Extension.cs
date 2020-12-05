using UnityEngine;

public static class Vector3Extension {

    /// <summary>
    /// Evaluates whether Vector3 is not a number
    /// </summary>
    public static bool IsNan(this Vector3 vector) {
        if(float.IsNaN(vector.x) && float.IsNaN(vector.y) && float.IsNaN(vector.z))
            return true;

        return false;
    }

    /// <summary>
    /// Evaluates whether Vector3 is negative infinity
    /// </summary>
    public static bool IsNegativeInfinity(this Vector3 vector)
    {
        return float.IsNegativeInfinity(vector.x) && float.IsNegativeInfinity(vector.y) && float.IsNegativeInfinity(vector.z);
    }
}