using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderInputFieldTMProPanel : MonoBehaviour
{
    public Slider slider { get; private set; }
    public TMP_InputField inputField { get; private set; }

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        inputField = GetComponentInChildren<TMP_InputField>();
    }
}
