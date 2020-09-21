using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Key : MonoBehaviour {

    [SerializeField] public KeyType keyType;

    private Keyboard keyboard;

    public void Awake() {
        this.GetComponentInChildren<Text>().text = this.name.ToLower();
        this.GetComponent<Button>().onClick.AddListener(delegate {
            Clicked();
        });

        keyboard = GameObject.FindGameObjectWithTag("Keyboard").GetComponent<Keyboard>();
        if(keyType == KeyType.Letter && this.GetComponentInChildren<Text>().text.Length < 2) {
            keyboard.OnCapslockToggle += ChangeLetter;
        }
    }

    public void Clicked() {
        switch(keyType) {
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
    Enter
}