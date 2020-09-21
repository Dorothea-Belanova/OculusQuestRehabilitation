using UnityEngine;

public static class PointRandomizer {

    private static float GetMinAngleForDistance(float distance, Vector3 initialPosition, float a) {
        float b = Mathf.Sqrt(distance * distance - a * a);
        if(float.IsNaN(b)) {
            //Debug.Log("CHYBA");
            return 20f;
        }
        float radians = Mathf.Asin(b / distance);
        float degrees = (radians * 180f) / Mathf.PI;
        //Debug.Log("degrees: " + degrees);
        return degrees;
    }

    private static float GetMaxAngleForDistance(float distance, Vector3 initialPosition, float a) {
        float b = Mathf.Sqrt(distance * distance - a * a);
        if(float.IsNaN(b)) {
            //Debug.Log("CHYBA");
            return 160f;
        }
        float radians = Mathf.Asin(b / distance);
        float degrees = (radians * 180f) / Mathf.PI;
        degrees = 180f - degrees;
        //Debug.Log("degrees: " + degrees);
        return degrees;
    }

    public static Vector3 GenerateRandomPointPosition(float distance, Vector3 initialPosition, GameObject table) {
        // Calculation of corners
        Vector3 center = new Vector3(table.transform.position.x, initialPosition.y, initialPosition.z);
        float halfWidth = table.transform.localScale.x / 2;
        Vector3 rightCorner = new Vector3(center.x + halfWidth, center.y, center.z);
        Vector3 leftCorner = new Vector3(center.x - halfWidth, center.y, center.z);

        float minAngle = GetMinAngleForDistance(distance, initialPosition, Vector3.Distance(rightCorner, initialPosition) - 0.05f);
        float maxAngle = GetMaxAngleForDistance(distance, initialPosition, Vector3.Distance(leftCorner, initialPosition) - 0.05f);

        Vector3 point;
        float angle = Random.Range(minAngle, maxAngle);
        float radians = (angle * Mathf.PI) / 180.0f;
        point.x = initialPosition.x + Mathf.Cos(radians) * distance;
        point.z = initialPosition.z + Mathf.Sin(radians) * distance;
        point.y = initialPosition.y;
        return point;
    }

    public static float[] GenerateDistances(float min, float max, float length, int points) {
        float sum = 0f;
        float cMin = min;
        float cMax = max;

        float[] distances = new float[points];
        /*Debug.Log("length: " + length);
        Debug.Log("points: " + points);
        Debug.Log("min: " + min);
        Debug.Log("min: " + max);*/

        for(int i = 0; i < points - 1; ++i) {
            if(points == 2) {
                (cMin, cMax) = RecalculateLimits(min, max, length, sum, points, i);
            }

            var random = Random.Range(cMin, cMax);
            distances[i] = random;
            sum += random;

            if(points != 2) {
                (cMin, cMax) = RecalculateLimits(min, max, length, sum, points, i);
            }
        }

        float lastDistance = length - sum;
        distances[points - 1] = lastDistance;

        if(lastDistance > max || lastDistance < min) {
            Debug.Log("PROBLEM: ");
            for(int i = 0; i < points; ++i) {
                Debug.Log(i + ": " + distances[i]);
            }
            return null;
        }
        else {
            return distances;
        }
    }

    private static (float cMin, float cMax) RecalculateLimits(float min, float max, float length, float sum, float points, float i) {
        var avg = (length - sum) / (points - i - 1);
        var diff = Mathf.Min(Mathf.Abs(max - avg), Mathf.Abs(avg - min));
        float cMin = avg - diff > min ? avg - diff : min;
        float cMax = avg + diff < max ? avg + diff : max;

        return (cMin, cMax);
    }
}