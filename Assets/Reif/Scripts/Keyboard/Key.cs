using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class Key : MonoBehaviour {

    [SerializeField] public KeyType keyType;

    private Keyboard keyboard;

    public void Awake() {
        keyboard = GameObject.FindGameObjectWithTag("Keyboard").GetComponent<Keyboard>();

        /*if (this.GetComponentsInChildren<TextMeshProUGUI>().Length > 0) {
            this.GetComponentInChildren<TextMeshProUGUI>().text = this.name.ToLower();

            if(keyType == KeyType.Letter)
                keyboard.OnCapslockToggle += ChangeLetter;
        }
        else
        {
            this.GetComponentInChildren<TextMeshProUGUI>().text = this.name.ToLower();
        }*/

        if (keyType == KeyType.Letter)
            keyboard.OnCapslockToggle += ChangeLetter;

        GetComponent<Button>().onClick.AddListener(delegate {
            Clicked();
        });
    }

    private void OnDestroy()
    {
        if (keyType == KeyType.Letter)
            keyboard.OnCapslockToggle -= ChangeLetter;

        GetComponent<Button>().onClick.RemoveListener(delegate {
            Clicked();
        });
    }

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

    public void ChangeLetter() {
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