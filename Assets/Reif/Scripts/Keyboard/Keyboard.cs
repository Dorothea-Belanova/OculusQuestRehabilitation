﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Keyboard: MonoBehaviour {

    public event Action OnCapslockToggle = delegate { };


    [SerializeField] public TMP_InputField inputField;
    public bool isCapsOn = false;
    InitialSceneControl sceneControl;

    public void Awake() {
        Debug.Log("tu som");

        sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<InitialSceneControl>();
    }

    public void AddCharacter(string character) {
        //Debug.Log("called: " + character);
        inputField.text = inputField.text + (character.Length > 1 ? " " : character);
    }

    public void BackspacePressed() {
        if(inputField.text.Length > 0) {
            inputField.text = inputField.text.Remove(inputField.text.Length - 1);
        }
    }

    public void CapslockToggle() {
        isCapsOn = !isCapsOn;
        OnCapslockToggle();
    }

    public void EnterPressed() {
        Debug.Log("NAME: " + inputField.text);

        // Removes whitespace from front and end of Patient ID
        inputField.text = inputField.text.Trim();

        sceneControl.OnKeyboardEnterClicked(inputField.text);
    }

    public void Clear() {
        inputField.text = "";
    }
}
