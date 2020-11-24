using UnityEngine;

public static class GameObjectExtension {

    /// <summary>
    /// Toggles the visibility by enabling and disabling Renderer.
    /// Triggers error when GameObject does not have Renderer.
    /// </summary>
    public static void ToggleVisibility(this GameObject go) {
        Renderer renderer = go.GetComponent<Renderer>();
        if(renderer != null) {
            renderer.enabled = !renderer.enabled;
        }
        else {
            Debug.LogError($"{go.name} does not contain Renderer Component.");
        }
    }

}