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
        var value = localizationLanguage.GetLocalizedValue(key);

        if (GetComponent<TextMeshPro>())
            GetComponent<TextMeshPro>().text = value;
        else if (GetComponent<TextMeshProUGUI>())
            GetComponent<TextMeshProUGUI>().text = value;

        if(boldAtFirst)
            HandleBoldText();
    }

    private void HandleBoldText() {
        if(IsBold() && localizationLanguage.language.code == "jp") {
            SetTextBold(false);
        }

        else if(!IsBold() && localizationLanguage.language.code != "jp") {
            SetTextBold(true);
        }
    }

    private void SetTextBold(bool value) {
        var fontStype = value ? FontStyles.Bold : FontStyles.Normal;

        if (GetComponent<TextMeshPro>())
            GetComponent<TextMeshPro>().fontStyle = fontStype;
        else if (GetComponent<TextMeshProUGUI>())
            GetComponent<TextMeshProUGUI>().fontStyle = fontStype;
    }

    private bool IsBold() {
        if (GetComponent<TextMeshPro>())
            return GetComponent<TextMeshPro>().fontStyle == FontStyles.Bold;
        else if (GetComponent<TextMeshProUGUI>())
            return GetComponent<TextMeshProUGUI>().fontStyle == FontStyles.Bold;

        return false;
    }

    public void SetLocalizationValues(string key, CurrentLocalizationLanguage localizationLanguage)
    {
        this.key = key;
        this.localizationLanguage = localizationLanguage;

        Initialize();
    }
}
