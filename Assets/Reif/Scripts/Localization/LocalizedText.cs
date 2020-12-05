using UnityEngine;
using TMPro;

public class LocalizedText: MonoBehaviour {

    [SerializeField] private CurrentLocalizationLanguage localizationLanguage;
    [SerializeField] private string key;

    private bool boldAtFirst;

    private void Start() => Initialize();

    public void OnDestroy() => localizationLanguage.OnLanguageChanged -= OnLanguageChanged;

    /// <summary>
    /// Initializes the properties for localized text
    /// </summary>
    private void Initialize()
    {
        localizationLanguage.OnLanguageChanged += OnLanguageChanged;

        boldAtFirst = IsBold();

        if (localizationLanguage.IsLoaded())
            SetText();
    }

    private void OnLanguageChanged() => SetText();

    /// <summary>
    /// Sets text with its localized value
    /// </summary>
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

    /// <summary>
    /// Handles localization of bold text
    /// </summary>
    private void HandleBoldText() {
        // If is bold and currently used localization language is japanese, it un-bolds it
        if(IsBold() && localizationLanguage.language.code == "jp")
            SetTextBold(false);
        // If text is bold and currently used localization language is not japanese, it bolds it
        else if (!IsBold() && localizationLanguage.language.code != "jp")
            SetTextBold(true);
    }

    /// <summary>
    /// Sets text bold
    /// </summary>
    private void SetTextBold(bool value) {
        var fontStype = value ? FontStyles.Bold : FontStyles.Normal;

        if (GetComponent<TextMeshPro>())
            GetComponent<TextMeshPro>().fontStyle = fontStype;
        else if (GetComponent<TextMeshProUGUI>())
            GetComponent<TextMeshProUGUI>().fontStyle = fontStype;
    }

    /// <summary>
    /// Returns true if text is bold
    /// </summary>
    private bool IsBold() {
        if (GetComponent<TextMeshPro>())
            return GetComponent<TextMeshPro>().fontStyle == FontStyles.Bold;
        else if (GetComponent<TextMeshProUGUI>())
            return GetComponent<TextMeshProUGUI>().fontStyle == FontStyles.Bold;

        return false;
    }

    /// <summary>
    /// Sets localization values for a localized text
    /// </summary>
    /// <remarks>
    /// !!! Only used in Segmented control !!!
    /// </remarks>
    public void SetLocalizationValues(string key, CurrentLocalizationLanguage localizationLanguage)
    {
        this.key = key;
        this.localizationLanguage = localizationLanguage;

        Initialize();
    }
}
