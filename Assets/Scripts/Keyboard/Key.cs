using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class Key : MonoBehaviour {

    [SerializeField] public KeyType keyType;

    private Keyboard keyboard;

    public void Awake() {
        keyboard = GameObject.FindGameObjectWithTag("Keyboard").GetComponent<Keyboard>();

        if (this.GetComponentsInChildren<Text>().Length > 0) {
            this.GetComponentInChildren<Text>().text = this.name.ToLower();

            if(keyType == KeyType.Letter && this.GetComponentInChildren<Text>().text.Length < 2)
                keyboard.OnCapslockToggle += ChangeLetter;
        }
        else
        {
            this.GetComponentInChildren<TextMeshProUGUI>().text = this.name.ToLower();
        }

        this.GetComponent<Button>().onClick.AddListener(delegate {
            Clicked();
        });
    }

    public void Clicked() {
        switch (keyType) {
            case KeyType.Space:
                keyboard.AddCharacter(" ");
                break;
            case KeyType.Letter:
                keyboard.AddCharacter(this.GetComponentInChildren<Text>().text);
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

    public void ChangeLetter() {
        if(keyboard.isCapsOn)
            this.GetComponentInChildren<Text>().text = this.GetComponentInChildren<Text>().text.ToUpper();
        else
            this.GetComponentInChildren<Text>().text = this.GetComponentInChildren<Text>().text.ToLower();
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