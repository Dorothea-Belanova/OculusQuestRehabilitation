using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InitialSceneControl : MonoBehaviour {

    [Header("Main Canvas Elements")]
    [SerializeField] private TMP_InputField patientIDInputField;
    [SerializeField] private SliderInputFieldTMProPanel verticalGamePanel;
    [SerializeField] private GameObject segmentedControl;
    [SerializeField] private GameObject afterHandSelectionGroup;
    [SerializeField] private GameObject afterHandSelectionLabelGroup;
    [SerializeField] private SliderInputFieldTMProPanel maxHandDistancePanel;
    [SerializeField] private Toggle fixedExerciseDistanceToggle;
    [SerializeField] private GameObject pointsLengthGroup;
    [SerializeField] private GameObject pointsLengthLabelGroup;
    [SerializeField] private SliderInputFieldTMProPanel exerciseLengthPanel;
    [SerializeField] private SliderInputFieldTMProPanel numberOfPointsPanel;
    [SerializeField] private TextMeshProUGUI numberOfPointsErrorText;
    [SerializeField] private Button continueButton;

    [Header("")]
    [SerializeField] private GameObject keyboardCanvas;
    [SerializeField] private ExerciseInfo exerciseInfo;

    private bool isKeyboardOn = false;
    private SelectedHand selectedHand = SelectedHand.None;

    /// <remarks>
    /// Has to be in Start (not in Awake) as in Oculus Quest the elements
    /// of SliderInputFieldTMProPanel are not instantiated yet.
    /// </remarks>
    private void Start() {
        InitializeUIElements();
        AddListeners();

        afterHandSelectionGroup.SetActive(false);
        afterHandSelectionLabelGroup.SetActive(false);
    }

    // TODO: POTOM ZMAZAT
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            ContinueButtonClicked();
    }

    private void OnApplicationQuit() {
        StopAllCoroutines();
    }

    #region Initialization

    private void InitializeUIElements() {
        // Setting sliders - min, max and value
        SetLimitsOnSlider(verticalGamePanel.slider, Constants.VERTICAL_GAME_HEIGHT_LIMITS);
        SetLimitsOnSlider(maxHandDistancePanel.slider, Constants.MAX_HAND_DISTANCE_LIMITS);
        SetLimitsOnSlider(exerciseLengthPanel.slider, Constants.EXERCISE_LENGTH_LIMITS);
        SetNumberOfPointsSlider();

        // Seting Input Fields - current slider value
        SetInputFieldValue(verticalGamePanel);
        SetInputFieldValue(maxHandDistancePanel);
        SetInputFieldValue(exerciseLengthPanel, Constants.EXERCISE_LENGTH_STEP);
        SetInputFieldValue(numberOfPointsPanel);

        HandleContinueButtonInteractivity();
    }

    private void SetLimitsOnSlider(Slider slider, Limits limits) {
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
            SetInputFieldValue(exerciseLengthPanel, Constants.EXERCISE_LENGTH_STEP);
            SetNumberOfPointsSlider();
        });

        // Number of Points Slider
        numberOfPointsPanel.slider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(numberOfPointsPanel, selectedHand == SelectedHand.BothHands ? 2 : 1);
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

    public void OnPatientIDInputClicked()
    {
        patientIDInputField.interactable = false;
        isKeyboardOn = true;
        keyboardCanvas.SetActive(true);
    }

    public void OnKeyboardEnterClicked(string patientID)
    {
        patientIDInputField.interactable = true;
        patientIDInputField.text = patientID;
        keyboardCanvas.SetActive(false);
        isKeyboardOn = false;
        HandleContinueButtonInteractivity();
    }

    private void HandSelected(SelectedHand handSelected) {
        afterHandSelectionGroup.SetActive(true);
        afterHandSelectionLabelGroup.SetActive(true);
        selectedHand = (SelectedHand)segmentedControl.GetComponent<SegmentedControl>().GetSelectedIndex();

        SetNumberOfPointsSlider();
    }

    public void FixedExerciseLengthToggled() {
        pointsLengthGroup.SetActive(fixedExerciseDistanceToggle.isOn);
        pointsLengthLabelGroup.SetActive(fixedExerciseDistanceToggle.isOn);

        if (fixedExerciseDistanceToggle.isOn)
            SetNumberOfPointsSlider();
        else
            HandleContinueButtonInteractivity();
    }

    public void HandleContinueButtonInteractivity()
    {
        var interactable = false;

        // Rules when Continue button is active only when:
        // 1. patient ID is set
        // 2. hand was selected
        // 3.a it is not fixed length exercise
        // 3.b it is fixed length exercise with single hand selected
        // 3.c it is fixed exercise length with both hands and number of points is even

        if (patientIDInputField.text != "" &&
            selectedHand != SelectedHand.None &&
            (!fixedExerciseDistanceToggle.isOn ||
            (fixedExerciseDistanceToggle.isOn && selectedHand != SelectedHand.BothHands) ||
            (fixedExerciseDistanceToggle.isOn && selectedHand == SelectedHand.BothHands && !numberOfPointsErrorText.IsActive())))
            interactable = true;

        continueButton.GetComponent<Button>().interactable = interactable;
    }

    private void SetNumberOfPointsSlider() {
        /*Debug.Log("VYPOCET:");
        Debug.Log("hand max: " + maxHandDistanceSlider.value);
        Debug.Log("margin: " + CALCULATION_MARGIN);
        Debug.Log("length: " + exerciseLengthSlider.value * 10);
        Debug.Log("min: " + (exerciseLengthSlider.value * 10) / (maxHandDistanceSlider.value - CALCULATION_MARGIN));
        Debug.Log("max: " + (exerciseLengthSlider.value * 10) / (MIN_DISTANCE + CALCULATION_MARGIN));*/
        float difference = (maxHandDistancePanel.slider.value - Constants.MIN_HAND_DISTANCE) / 3f;
        Debug.Log("DIFFERENCE: " + difference);
        int min = (int)Math.Ceiling((exerciseLengthPanel.slider.value * Constants.EXERCISE_LENGTH_STEP) / (maxHandDistancePanel.slider.value - difference));
        int max = (int)Math.Floor((exerciseLengthPanel.slider.value * Constants.EXERCISE_LENGTH_STEP) / (Constants.MIN_HAND_DISTANCE + difference));

        if(min > max) {
            max = min;
        }
        if(min == max) {
            numberOfPointsPanel.slider.enabled = false;
        }
        else {
            numberOfPointsPanel.slider.enabled = true;
        }

        //NumberOfPointsErrorOccurred(false);

        bool errorOccurred = false;

        if(selectedHand == SelectedHand.BothHands) {
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
                Debug.Log("DELIM DVOMI");
                min /= 2;
                max /= 2;
            }
        }

        Debug.Log("EROR OCCURRED: " + errorOccurred);
        NumberOfPointsErrorOccurred(errorOccurred);

        Debug.Log("min: " + min);
        Debug.Log("max: " + max);
        Limits limits = new Limits(min, max);

        Debug.Log("OVERENIE: ");
        Debug.Log("min: " + limits.min);
        Debug.Log("max: " + limits.max);
        SetLimitsOnSlider(numberOfPointsPanel.slider, limits);
        SetInputFieldValue(numberOfPointsPanel, (selectedHand == SelectedHand.BothHands && !errorOccurred) ? 2 : 1);
        HandleContinueButtonInteractivity();
    }

    private void NumberOfPointsErrorOccurred(bool occurred) {
        numberOfPointsPanel.slider.enabled = !occurred;
        numberOfPointsErrorText.gameObject.SetActive(occurred);
    }

    void ContinueButtonClicked() {
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
        panel.inputField.text = ((int)(panel.slider.value * multiplier)).ToString();
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
    None = -1,
    BothHands,
    LeftHand,
    RightHand
}