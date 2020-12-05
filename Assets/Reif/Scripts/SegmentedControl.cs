using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SegmentedControl: MonoBehaviour {

    [SerializeField] private CurrentLocalizationLanguage localizationLanguage;
    [SerializeField] private int selected = -1;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.grey;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.green;
    [SerializeField] private Sprite sprite;
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private int fontSize = 30;
    [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Center;
    [SerializeField] private string[] segments = new string[] { "Segment 1", "Segment 2", "Segment 3" };
    [SerializeField] private bool supportsLocalization = false;
    [SerializeField] private string[] segmentsTranslationKeywords = new string[] { "Keyword 1", "Keyword 2", "Keyword 3" };

    public event Action<SelectedHand> OnValueChanged;

    private LocalizedText[] localizedTexts;

    /// <summary>
    /// Handles creation of segmented control
    /// </summary>
    public void Awake() {
        float segmentWidth = 1f / segments.Length;
        localizedTexts = new LocalizedText[segments.Length];

        for (int i = 0; i < segments.Length; ++i) {
            GameObject button = new GameObject();
            button.name = "Button" + ((i > 0) ? ( " (" + i.ToString() + ")") : "");
            button.transform.parent = this.transform;
            button.AddComponent<RectTransform>();
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<Image>().sprite = sprite;
            button.GetComponent<Image>().type = Image.Type.Sliced;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(i * segmentWidth, 0);
            button.GetComponent<RectTransform>().anchorMax = new Vector2((i + 1) * segmentWidth, 1);

            // Two following two lines are needed if world canvas
            button.GetComponent<RectTransform>().localPosition = Vector3.zero;
            button.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            button.GetComponent<RectTransform>().SetZero();
            
            button.GetComponent<RectTransform>().SetSiblingIndex(i);
            button.GetComponent<Button>().onClick.AddListener(delegate { OnPressed(button); });

            GameObject text = new GameObject();
            text.name = "Text";
            text.transform.parent = button.transform;
            text.AddComponent<RectTransform>();
            text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

            // Two following two lines are needed if world canvas
            text.GetComponent<RectTransform>().localPosition = Vector3.zero;
            text.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

            text.AddComponent<TextMeshProUGUI>();
            text.GetComponent<TextMeshProUGUI>().text = segments[i];
            text.GetComponent<TextMeshProUGUI>().font = fontAsset;
            text.GetComponent<TextMeshProUGUI>().fontSize = fontSize;
            text.GetComponent<TextMeshProUGUI>().color = normalTextColor;
            text.GetComponent<TextMeshProUGUI>().alignment = alignment;

            // It has to be here because TMPro text stretches the dimensions
            text.GetComponent<RectTransform>().SetZero();

            if (supportsLocalization)
            {
                text.AddComponent<LocalizedText>();
                text.GetComponent<LocalizedText>().SetLocalizationValues(segmentsTranslationKeywords[i], localizationLanguage);
                localizedTexts[i] = text.GetComponent<LocalizedText>();
            }
        }
    }

    /// <summary>
    /// Handles removal of listeners if segmented control supports localization
    /// </summary>
    private void OnDestroy()
    {
        if (supportsLocalization)
        {
            for (int i = 0; i < segments.Length; ++i)
                localizedTexts[i].OnDestroy();
        }
    }

    /// <summary>
    /// Handles on press action of one of the segments
    /// </summary>
    private void OnPressed(GameObject gameObject) {
        int index = gameObject.GetComponent<RectTransform>().GetSiblingIndex();

        if(selected != -1)
            ChangeColors(selected, normalColor, normalTextColor);

        selected = index;
        ChangeColors(selected, selectedColor, selectedTextColor);
        OnValueChanged?.Invoke((SelectedHand)index);
    }

    /// <summary>
    /// Changes colors of tex and button for the corresponding segment
    /// </summary>
    private void ChangeColors(int index, Color buttonColor, Color textColor) {
        this.transform.GetChild(index).GetComponent<Image>().color = buttonColor;
        this.transform.GetChild(index).GetChild(0).GetComponent<TextMeshProUGUI>().color = textColor;
    }

    /// <summary>
    /// Returns index of selected segment
    /// </summary>
    public int GetSelectedIndex() {
        return selected;
    }
}