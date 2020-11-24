using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText: MonoBehaviour {

    [SerializeField] public string key;

    private LocalizationManager localizationManager;
    private bool boldAtFirst;

    private void Start() {
        localizationManager = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();
        localizationManager.OnLocalizationChange += ChangeText;

        boldAtFirst = IsBold();

        if(localizationManager.GetIsReady()) {
            ChangeText();
        }
    }

    public void ChangeText() {
        Debug.Log("changing text");
        string value = LocalizationManager.instance.GetLocalizedValue(key);

        if(GetComponent<Text>()) {
            Text text = GetComponent<Text>();
            text.text = value;
        }
        else if(GetComponent<TextMeshPro>()) {
            TextMeshPro text = GetComponent<TextMeshPro>();
            text.text = value;
        }
        else if(GetComponent<TextMeshProUGUI>()) {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.text = value;
        }

        TakeCareOfBoldText();
    }

    private void TakeCareOfBoldText() {
        if(!boldAtFirst)
            return;
        if(boldAtFirst && IsBold() && LocalizationManager.instance.GetLanguage().code == "jp") {
            SetBold(false);
        }
        else if(boldAtFirst && !IsBold() && LocalizationManager.instance.GetLanguage().code != "jp") {
            SetBold(true);
        }
    }

    private bool SetBold(bool value) {
        if(GetComponent<Text>()) {
            Text text = GetComponent<Text>();
            text.fontStyle = value ? FontStyle.Bold : FontStyle.Normal;
        }
        else if(GetComponent<TextMeshPro>() || GetComponent<TextMeshProUGUI>()) {
            TextMeshPro text = GetComponent<TextMeshPro>();
            text.fontStyle = value ? FontStyles.Bold : FontStyles.Normal;
        }

        return false;
    }

    private bool IsBold() {
        if(GetComponent<Text>()) {
            Text text = GetComponent<Text>();
            return text.fontStyle == FontStyle.Bold;
        }
        else if(GetComponent<TextMeshProUGUI>()) {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            return text.fontStyle == FontStyles.Bold;
        }

        return false;
    }
}
