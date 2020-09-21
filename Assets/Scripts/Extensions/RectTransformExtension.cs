using UnityEngine;

public static class RectTransformExtension {
    public static void SetZero(this RectTransform rt) {
        rt.SetAll(0, 0, 0, 0);
    }

    public static void SetAll(this RectTransform rt, float top, float bottom, float left, float right) {
        rt.SetTop(top);
        rt.SetBottom(bottom);
        rt.SetLeft(left);
        rt.SetRight(right);
    }

    public static void SetLeft(this RectTransform rt, float left) {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right) {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top) {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom) {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}