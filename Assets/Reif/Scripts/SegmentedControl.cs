using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[AddComponentMenu("UI/Segmented Control", 32)]
public class SegmentedControl: MonoBehaviour {

    [SerializeField] private CurrentLocalizationLanguage localizationLanguage;
    [SerializeField] public int selected = -1;
    [SerializeField] public Color normalColor = Color.white;
    [SerializeField] public Color selectedColor = Color.grey;
    [SerializeField] public Color normalTextColor = Color.black;
    [SerializeField] public Color selectedTextColor = Color.green;
    [SerializeField] public Sprite sprite;
    [SerializeField] public TMP_FontAsset fontAsset;
    [SerializeField] public int fontSize = 30;
    [SerializeField] public TextAlignmentOptions alignment = TextAlignmentOptions.Center;
    [SerializeField] public string[] segments = new string[] { "Segment 1", "Segment 2", "Segment 3" };
    [SerializeField] public bool supportsLocalization = false;
    [SerializeField] public string[] segmentsTranslationKeywords = new string[] { "Keyword 1", "Keyword 2", "Keyword 3" };

    public event Action<SelectedHand> OnValueChanged;

    public void Awake() {
        float segmentWidth = 1f / segments.Length;

        for(int i = 0; i < segments.Length; ++i) {
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

            // next two are needed if world canvas
            button.GetComponent<RectTransform>().localPosition = Vector3.zero;
            button.GetComponent<RectTransform>().SetZero();
            button.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            
            button.GetComponent<RectTransform>().SetSiblingIndex(i);
            button.GetComponent<Button>().onClick.AddListener(delegate { OnPressed(button); });

            GameObject text = new GameObject();
            text.name = "Text";
            text.transform.parent = button.transform;
            text.AddComponent<RectTransform>();
            text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

            // next two are needed if world canvas
            text.GetComponent<RectTransform>().localPosition = Vector3.zero;
            text.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            //SetAndStretchToParentSize(text.GetComponent<RectTransform>(), button.GetComponent<RectTransform>());
            //text.GetComponent<RectTransform>().SetZero();

            text.AddComponent<TextMeshProUGUI>();
            text.GetComponent<TextMeshProUGUI>().text = segments[i];
            text.GetComponent<TextMeshProUGUI>().font = fontAsset;
            text.GetComponent<TextMeshProUGUI>().fontSize = fontSize;
            text.GetComponent<TextMeshProUGUI>().color = normalTextColor;
            text.GetComponent<TextMeshProUGUI>().alignment = alignment;

            // it has to be here because TMPro text stretches the dimensions
            text.GetComponent<RectTransform>().SetZero();

            if (supportsLocalization)
            {
                text.AddComponent<LocalizedText>();
                text.GetComponent<LocalizedText>().SetValues(segmentsTranslationKeywords[i], localizationLanguage);
            }
        }
    }

    private void OnPressed(GameObject gameObject) {
        Debug.Log("PRESSED!");
        int index = gameObject.GetComponent<RectTransform>().GetSiblingIndex();

        Debug.Log("selected: " + gameObject);
        if(selected != -1) {
            ChangeColors(selected, normalColor, normalTextColor);
        }

        selected = index;
        ChangeColors(selected, selectedColor, selectedTextColor);
        OnValueChanged?.Invoke((SelectedHand)index);
    }

    private void ChangeColors(int index, Color buttonColor, Color textColor) {
        this.transform.GetChild(index).GetComponent<Image>().color = buttonColor;
        this.transform.GetChild(index).GetChild(0).GetComponent<TextMeshProUGUI>().color = textColor;
    }

    public int GetSelectedIndex() {
        return selected;
    }
}