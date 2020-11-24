using UnityEngine;

public static class floatExtension {

    /// <summary>
    /// Returns angle in range -180 to +180 degrees.
    /// </summary>
    public static float WrapAngle(this float angle) {
        angle %= 360;
        if(angle > 180)
            return angle - 360;
        else if(angle < -180)
            return angle + 360;
        return angle;
    }

    /// <summary>
    /// Returns angle in range 0 to 360 degrees.
    /// </summary>
    public static float UnwrapAngle(this float angle) {
        if(angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    /// <summary>
    /// Returns value of angle in radians.
    /// </summary>
    public static float ConvertToRadians(this float angle) {
        return (float)(System.Math.PI / 180) * angle;
    }
}
