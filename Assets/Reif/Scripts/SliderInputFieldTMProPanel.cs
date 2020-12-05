using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderInputFieldTMProPanel : MonoBehaviour
{
    public Slider slider { get; private set; }
    public TMP_InputField inputField { get; private set; }

    /// <summary>
    /// Sets values of slider and input field
    /// </summary>
    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        inputField = GetComponentInChildren<TMP_InputField>();
    }
}
