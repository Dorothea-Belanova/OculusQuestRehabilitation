using UnityEngine;

public static class Vector3Extension {

    /// <summary>
    /// Evaluates whether Vector3 is not a number.
    /// </summary>
    public static bool IsNan(this Vector3 vector) {
        if(float.IsNaN(vector.x) && float.IsNaN(vector.y) && float.IsNaN(vector.z)) {
            return true;
        }
        return false;
    }

}