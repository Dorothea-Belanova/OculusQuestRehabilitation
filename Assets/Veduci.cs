using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Veduci : MonoBehaviour {

    [SerializeField] GameObject keyboard;
    [SerializeField] GameObject inputField;
    [SerializeField] GameObject welcomeMessage;

    public void EnterPressed() {
        var name = inputField.GetComponent<InputField>().text;
        welcomeMessage.GetComponent<Text>().text = "Welcome, " + name + "!";

        keyboard.SetActive(false);
        inputField.SetActive(false);
        welcomeMessage.SetActive(true);
    }
}