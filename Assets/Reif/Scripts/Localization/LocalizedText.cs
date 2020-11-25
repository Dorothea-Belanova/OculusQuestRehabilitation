using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText: MonoBehaviour {

    [SerializeField] private CurrentLocalizationLanguage localizationLanguage;
    [SerializeField] private string key;

    private bool boldAtFirst;

    private void Start() => Initialize();

    private void OnDestroy() => localizationLanguage.OnLanguageChanged -= OnLanguageChanged;

    private void Initialize()
    {
        localizationLanguage.OnLanguageChanged += OnLanguageChanged;

        boldAtFirst = IsBold();

        if (localizationLanguage.IsLoaded())
            SetText();
    }

    private void OnLanguageChanged() => SetText();

    private void SetText()
    {
        string value = localizationLanguage.GetLocalizedValue(key);

        if (GetComponent<Text>())
        {
            Text text = GetComponent<Text>();
            text.text = value;
        }
        else if (GetComponent<TextMeshPro>())
        {
            TextMeshPro text = GetComponent<TextMeshPro>();
            text.text = value;
        }
        else if (GetComponent<TextMeshProUGUI>())
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.text = value;
        }

        TakeCareOfBoldText();
    }

    private void TakeCareOfBoldText() {
        if(!boldAtFirst)
            return;
        if(boldAtFirst && IsBold() && localizationLanguage.language.code == "jp") {
            SetBold(false);
        }
        else if(boldAtFirst && !IsBold() && localizationLanguage.language.code != "jp") {
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

    public void SetValues(string key, CurrentLocalizationLanguage localizationLanguage)
    {
        this.key = key;
        this.localizationLanguage = localizationLanguage;

        Initialize();
    }
}
