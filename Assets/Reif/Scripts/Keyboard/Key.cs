﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class Key : MonoBehaviour {

    [SerializeField] public KeyType keyType;

    private Keyboard keyboard;

    public void Awake() {
        keyboard = GameObject.FindGameObjectWithTag("Keyboard").GetComponent<Keyboard>();

        if (keyType == KeyType.Letter)
            keyboard.OnCapslockToggle += ToggleCapslock;

        GetComponent<Button>().onClick.AddListener(delegate {
            Clicked();
        });
    }

    /// <summary>
    /// Handles key being clicked
    /// </summary>
    public void Clicked() {
        switch (keyType) {
            case KeyType.Space:
                keyboard.AddCharacter(" ");
                break;
            case KeyType.Letter:
                keyboard.AddCharacter(this.GetComponentInChildren<TextMeshProUGUI>().text);
                break;
            case KeyType.Uppercase:
                keyboard.CapslockToggle();
                break;
            case KeyType.Backspace:
                keyboard.BackspacePressed();
                break;
            case KeyType.Clear:
                keyboard.Clear();
                break;
            case KeyType.Enter:
                keyboard.EnterPressed();
                break;
        }
    }

    /// <summary>
    /// Toggles capslock for Letter keytype keys
    /// </summary>
    public void ToggleCapslock() {
        if(keyboard.isCapsOn)
            this.GetComponentInChildren<TextMeshProUGUI>().text = this.GetComponentInChildren<TextMeshProUGUI>().text.ToUpper();
        else
            this.GetComponentInChildren<TextMeshProUGUI>().text = this.GetComponentInChildren<TextMeshProUGUI>().text.ToLower();
    }
}

public enum KeyType {
    Letter,
    Uppercase,
    Backspace,
    Clear,
    Space,
    Enter
}