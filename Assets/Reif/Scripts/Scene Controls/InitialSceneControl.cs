using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InitialSceneControl: MonoBehaviour {

    [SerializeField] public TMP_InputField patientIDInputField;
    [SerializeField] private SliderInputFieldTMProPanel verticalGamePanel;
    /*[SerializeField] public Slider verticalGameSlider;
    [SerializeField] public TMP_InputField verticalGameInputField;*/
    [SerializeField] public GameObject segmentedControl;
    [SerializeField] public GameObject afterHandSelectionGroup;
    [SerializeField] public GameObject afterHandSelectionLabelGroup;

    [SerializeField] private SliderInputFieldTMProPanel maxHandDistancePanel;
    /*[SerializeField] public Slider maxHandDistanceSlider;
    [SerializeField] public TMP_InputField maxHandDistanceInputField;*/

    [SerializeField] public Toggle fixedExerciseDistanceToggle;

    [SerializeField] public GameObject pointsLengthGroup;
    [SerializeField] public GameObject pointsLengthLabelGroup;

    [SerializeField] private SliderInputFieldTMProPanel exerciseLengthPanel;
    /*[SerializeField] public Slider exerciseLengthSlider;
    [SerializeField] public TMP_InputField exerciseLengthInputField;*/
    [SerializeField] private SliderInputFieldTMProPanel numberOfPointsPanel;
    /*[SerializeField] public Slider numberOfPointsSlider;
    [SerializeField] public TMP_InputField numberOfPointsInputField;*/
    [SerializeField] public TextMeshProUGUI numberOfPointsErrorText;
    [SerializeField] public Button continueButton;

    [SerializeField] public GameObject keyboardCanvas;

    [SerializeField] private ExerciseInfo exerciseInfo;

    // STATIC VALUES
    static Limits VERTICAL_GAME_HEIGHT_LIMITS = new Limits(15f, 40f); // with step 1 cm
    static Limits MAX_HAND_DISTANCE_LIMITS = new Limits(35f, 85f); // with step 1 cm
    static Limits EXERCISE_LENGTH_LIMITS = new Limits(12f, 100f); // with step 10 cm
    static float MIN_DISTANCE = 25f;
    static float CALCULATION_MARGIN = 8f;
    static int EXERCISE_LENGTH_STEP = 10;

    private bool keyboardOn = false;

    private void Start() {
        InitializeUIElements();
        AddListeners();


        afterHandSelectionGroup.SetActive(false);
        afterHandSelectionLabelGroup.SetActive(false);
        //FilterTesting.Test();
    }

    private void Update() {
        /*if(patientIDInputField.isFocused && !keyboardOn) {
            keyboardOn = true;
            Debug.Log("keyboard is on");
            //keyboardCanvas.GetComponent<Image>().CrossFadeAlpha(0.1f, 2.0f, false);
            keyboardCanvas.SetActive(true);
            //StartCoroutine(FadeEffect.FadeCanvas(keyboardCanvas, 0f, 1f, 2f));
            //keyboardCanvas.alpha = 1f;
        }*/

        if (Input.GetKeyDown(KeyCode.Space))
            ContinueButtonClicked();
    }

    public void PatientIDInputClicked()
    {
        patientIDInputField.interactable = false;
        keyboardOn = true;
        //keyboardCanvas.GetComponent<Image>().CrossFadeAlpha(0.1f, 2.0f, false);
        keyboardCanvas.SetActive(true);
        //StartCoroutine(FadeEffect.FadeCanvas(keyboardCanvas, 0f, 1f, 2f));
        //keyboardCanvas.alpha = 1f;
    }

    public void KeyboardSet(string patientID) {
        patientIDInputField.interactable = true;
        patientIDInputField.text = patientID;
        keyboardCanvas.SetActive(false);
        keyboardOn = false;
    }

    private void OnPatientIDFocus() {
        keyboardCanvas.SetActive(true);
    }

    private void OnApplicationQuit() {
        StopAllCoroutines();
    }

    #region Initialization

    private void InitializeUIElements() {
        // Setting sliders
        SetLimitsOnSlider(verticalGamePanel.slider, VERTICAL_GAME_HEIGHT_LIMITS);
        SetLimitsOnSlider(maxHandDistancePanel.slider, MAX_HAND_DISTANCE_LIMITS);
        SetLimitsOnSlider(exerciseLengthPanel.slider, EXERCISE_LENGTH_LIMITS);
        SetNumberOfPointsSlider();

        // Seting Input Fields
        SetInputFieldValue(verticalGamePanel);
        SetInputFieldValue(maxHandDistancePanel);
        SetInputFieldValue(exerciseLengthPanel, EXERCISE_LENGTH_STEP);
        SetInputFieldValue(numberOfPointsPanel);

        continueButton.GetComponent<Button>().interactable = false;
    }

    private void SetLimitsOnSlider(Slider slider, Limits limits) {
        Debug.Log("SETTING LIMITS FOR: " + slider.name);

        slider.minValue = limits.min;
        slider.maxValue = limits.max;

        slider.value = limits.min;
    }

    #endregion

    #region Listeners
    private void AddListeners() {
        // Vertical Game Height Slider
        verticalGamePanel.slider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(verticalGamePanel);
        });

        // Max Hand Distance Slider
        maxHandDistancePanel.slider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(maxHandDistancePanel);
            SetNumberOfPointsSlider();
        });

        // Exercise Length Slider
        exerciseLengthPanel.slider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(exerciseLengthPanel, EXERCISE_LENGTH_STEP);
            SetNumberOfPointsSlider();
        });

        // Number of Points Slider
        numberOfPointsPanel.slider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(numberOfPointsPanel, AreBothHandsSelected() ? 2 : 1);
        });

        // Fixed Length Toggle
        fixedExerciseDistanceToggle.onValueChanged.AddListener(delegate {
            FixedExerciseLengthToggled();
        });

        // Selected Hand Segmented Control
        segmentedControl.GetComponent<SegmentedControl>().OnValueChanged += HandSelected;

        // Continue Button
        continueButton.onClick.AddListener(delegate {
            ContinueButtonClicked();
        });
    }

    private void HandSelected(SelectedHand selectedHand) {
        afterHandSelectionGroup.SetActive(true);
        afterHandSelectionLabelGroup.SetActive(true);
        continueButton.GetComponent<Button>().interactable = true;
        SetNumberOfPointsSlider();
    }

    public void FixedExerciseLengthToggled() {
        pointsLengthGroup.SetActive(fixedExerciseDistanceToggle.isOn);
        pointsLengthLabelGroup.SetActive(fixedExerciseDistanceToggle.isOn);

        if (!fixedExerciseDistanceToggle.isOn)
            continueButton.GetComponent<Button>().interactable = true;
        if(fixedExerciseDistanceToggle.isOn)
            SetNumberOfPointsSlider();
    }

    private void SetNumberOfPointsSlider() {
        /*Debug.Log("VYPOCET:");
        Debug.Log("hand max: " + maxHandDistanceSlider.value);
        Debug.Log("margin: " + CALCULATION_MARGIN);
        Debug.Log("length: " + exerciseLengthSlider.value * 10);
        Debug.Log("min: " + (exerciseLengthSlider.value * 10) / (maxHandDistanceSlider.value - CALCULATION_MARGIN));
        Debug.Log("max: " + (exerciseLengthSlider.value * 10) / (MIN_DISTANCE + CALCULATION_MARGIN));*/
        int min = (int)Math.Ceiling((exerciseLengthPanel.slider.value * EXERCISE_LENGTH_STEP) / (maxHandDistancePanel.slider.value));
        int max = (int)Math.Floor((exerciseLengthPanel.slider.value * EXERCISE_LENGTH_STEP) / MIN_DISTANCE);


        if(min > max) {
            max = min;
        }
        if(min == max) {
            numberOfPointsPanel.slider.enabled = false;
        }
        else {
            numberOfPointsPanel.slider.enabled = true;
        }

        NumberOfPointsErrorOccurred(false);

        Debug.Log("min: " + min);
        Debug.Log("max: " + max);

        bool errorOccurred = false;

        if(AreBothHandsSelected()) {
            if(min == max && !min.IsEven()) {
                Debug.Log("tu som");
                errorOccurred = true;
                if(errorOccurred) {
                    Debug.Log("vnorene");
                }
            }
            else if(min != max) {
                if(!min.IsEven()) {
                    if(min + 1 <= max) {
                        min += 1;
                    }
                    else {
                        errorOccurred = true;
                    }
                }
                if(!max.IsEven()) {
                    if(max - 1 >= min) {
                        max -= 1;
                    }
                    else {
                        errorOccurred = true;
                    }
                }
            }
            if(!errorOccurred) {
                Debug.Log("tadaa");
                min /= 2;
                max /= 2;
            }
        }

        Debug.Log("tadyyy");
        NumberOfPointsErrorOccurred(errorOccurred);

        Debug.Log("min: " + min);
        Debug.Log("max: " + max);
        Limits limits = new Limits(min, max);

        SetLimitsOnSlider(numberOfPointsPanel.slider, limits);
        SetInputFieldValue(numberOfPointsPanel, (AreBothHandsSelected() && !errorOccurred) ? 2 : 1);
    }

    private void NumberOfPointsErrorOccurred(bool occurred) {
        numberOfPointsPanel.slider.enabled = !occurred;
        numberOfPointsErrorText.gameObject.SetActive(occurred);
        continueButton.GetComponent<Button>().interactable = !occurred;
    }

    void ContinueButtonClicked() {
        SelectedHand selectedHand = (SelectedHand)segmentedControl.GetComponent<SegmentedControl>().GetSelectedIndex();
        bool bothHands = selectedHand == SelectedHand.BothHands;

        exerciseInfo.patientID = patientIDInputField.text;
        exerciseInfo.verticalGameHeight = verticalGamePanel.slider.value / 100;
        exerciseInfo.selectedHand = selectedHand;
        exerciseInfo.maxHandDistance = maxHandDistancePanel.slider.value / 100;
        exerciseInfo.fixedExerciseLength = fixedExerciseDistanceToggle.isOn;
        exerciseInfo.exerciseLength = (fixedExerciseDistanceToggle.isOn ? Mathf.Floor(exerciseLengthPanel.slider.value) / 10 : -1f);
        // I NEED TO FIX THIS, AS I AM NOT REACTING TO CHANGE OF SELECTED HAND VALUE
        exerciseInfo.numberOfPoints = fixedExerciseDistanceToggle.isOn ? (bothHands ? (int)numberOfPointsPanel.slider.value * 2 : (int)numberOfPointsPanel.slider.value) : -1;
        //exerciseInfo.numberOfPoints = fixedExerciseDistanceToggle.isOn ? (int)numberOfPointsSlider.value * 2 : -1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion

    private void SetInputFieldValue(SliderInputFieldTMProPanel panel, int multiplier = 1) {
        //Debug.Log("multiplier: " + multiplier);
        Debug.Log("SETTING INPUT FOR: " + panel.name);
        panel.inputField.text = ((int)(panel.slider.value * multiplier)).ToString();
        //Debug.Log("value: " + inputField.text);
    }

    private bool AreBothHandsSelected() {
        return segmentedControl.GetComponent<SegmentedControl>().GetSelectedIndex() == 0;
    }
}

public struct Limits {
    public float min { get; set; }
    public float max { get; set; }

    public Limits(float min, float max) : this() {
        this.min = min;
        this.max = max;
    }
}

public enum SelectedHand
{
    BothHands,
    LeftHand,
    RightHand
}