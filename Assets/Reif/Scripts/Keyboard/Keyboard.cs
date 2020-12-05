using System;
using UnityEngine;
using TMPro;

public class Keyboard: MonoBehaviour {

    public event Action OnCapslockToggle;

    [SerializeField] public TMP_InputField inputField;
    public bool isCapsOn = false;
    InitialSceneControl sceneControl;

    public void Awake() {
        inputField.text = "";

        sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<InitialSceneControl>();
    }

    /// <summary>
    /// Handles click on Letter keytype action
    /// </summary>
    public void AddCharacter(string character) {
        inputField.text = inputField.text + character;
    }

    /// <summary>
    /// Handles backspace click action
    /// </summary>
    public void BackspacePressed() {
        if(inputField.text.Length > 0)
            inputField.text = inputField.text.Remove(inputField.text.Length - 1);
    }

    /// <summary>
    /// Handles capslock clicked action
    /// </summary>
    public void CapslockToggle() {
        isCapsOn = !isCapsOn;
        OnCapslockToggle();
    }

    /// <summary>
    /// Handles enter clicked action
    /// </summary>
    public void EnterPressed() {
        // Removes whitespace from front and end of Patient ID
        inputField.text = inputField.text.Trim();

        sceneControl.OnKeyboardEnterClicked(inputField.text);
    }

    /// <summary>
    /// Clears keyboard's input field
    /// </summary>
    public void Clear() {
        inputField.text = "";
    }
}
