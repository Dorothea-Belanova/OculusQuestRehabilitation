using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InitialSceneControl : MonoBehaviour {

    [Header("Main Canvas Elements")]
    [SerializeField] private TMP_InputField patientIDInputField;
    [SerializeField] private SliderInputFieldTMProPanel verticalGamePanel;
    [SerializeField] private GameObject handSelectionSegmentedControl;
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

    private SelectedHand selectedHand = SelectedHand.None;

    /// <remarks>
    /// Has to be in Start (not in Awake) as in Oculus Quest the elements
    /// of SliderInputFieldTMProPanel are not instantiated yet
    /// </remarks>
    private void Start() {
        exerciseInfo.Reset();

        InitializeUIElements();
        AddListeners();

        afterHandSelectionGroup.SetActive(false);
        afterHandSelectionLabelGroup.SetActive(false);
    }

    /// <summary>
    /// Initializes UI elements - sliders and input fields - with proper values
    /// </summary>
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

    /// <summary>
    /// Adds listeners to UI elements
    /// </summary>
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
        handSelectionSegmentedControl.GetComponent<SegmentedControl>().OnValueChanged += HandSelected;

        // Continue Button
        continueButton.onClick.AddListener(delegate {
            ContinueButtonClicked();
        });
    }

    /// <summary>
    /// Handles click on PatientID input field
    /// </summary>
    public void OnPatientIDInputClicked()
    {
        patientIDInputField.text = exerciseInfo.patientID;
        patientIDInputField.interactable = false;
        keyboardCanvas.SetActive(true);
    }

    /// <summary>
    /// Handles Enter click on a keyboard
    /// </summary>
    public void OnKeyboardEnterClicked(string patientID)
    {
        patientIDInputField.interactable = true;
        patientIDInputField.text = patientID;
        exerciseInfo.patientID = patientID;
        keyboardCanvas.SetActive(false);
        HandleContinueButtonInteractivity();
    }

    /// <summary>
    /// Handles hand selection
    /// </summary>
    private void HandSelected(SelectedHand handSelected) {
        afterHandSelectionGroup.SetActive(true);
        afterHandSelectionLabelGroup.SetActive(true);
        selectedHand = (SelectedHand)handSelectionSegmentedControl.GetComponent<SegmentedControl>().GetSelectedIndex();

        SetNumberOfPointsSlider();
    }

    /// <summary>
    /// Handles toggle of fixed exercise length
    /// </summary>
    public void FixedExerciseLengthToggled() {
        pointsLengthGroup.SetActive(fixedExerciseDistanceToggle.isOn);
        pointsLengthLabelGroup.SetActive(fixedExerciseDistanceToggle.isOn);

        if (fixedExerciseDistanceToggle.isOn)
            SetNumberOfPointsSlider();
        else
            HandleContinueButtonInteractivity();
    }

    /// <summary>
    /// Handles interactivity of Continue button based on states of other UI elements
    /// </summary>
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

    /// <summary>
    /// Calculates number of points and sets limits on number of points slider
    /// </summary>
    private void SetNumberOfPointsSlider() {
        (float min, float max, bool errorOccurred) = CalculateNumberOfPoints();

        /*if(min == max)
            numberOfPointsPanel.slider.enabled = false;
        else
            numberOfPointsPanel.slider.enabled = true;*/

        // Handles slider activity and error text based on error occurrer value
        numberOfPointsPanel.slider.enabled = !errorOccurred;
        numberOfPointsErrorText.gameObject.SetActive(errorOccurred);

        Limits limits = new Limits(min, max);
        SetLimitsOnSlider(numberOfPointsPanel.slider, limits);
        SetInputFieldValue(numberOfPointsPanel, (selectedHand == SelectedHand.BothHands && !errorOccurred) ? 2 : 1);
        HandleContinueButtonInteractivity();
    }

    /// <summary>
    /// Calculates minimum and maximum number of points possible
    /// </summary>
    private (float min, float max, bool errorOccurred) CalculateNumberOfPoints()
    {
        // Calculates minimum and maximum number of points
        float difference = (maxHandDistancePanel.slider.value - Constants.MIN_HAND_DISTANCE) / 3f;
        int min = (int)Math.Ceiling((exerciseLengthPanel.slider.value * Constants.EXERCISE_LENGTH_STEP) / (maxHandDistancePanel.slider.value - difference));
        int max = (int)Math.Floor((exerciseLengthPanel.slider.value * Constants.EXERCISE_LENGTH_STEP) / (Constants.MIN_HAND_DISTANCE + difference));

        // Fixes value of maximum, if minimum is bigger
        if (min > max)
            max = min;

        bool errorOccurred = false;

        if (selectedHand == SelectedHand.BothHands)
        {
            // If minimum and maximum values are the same and minimum is odd, it causes
            // error as number of points must be even for both hands exercise
            if (min == max && !min.IsEven())
            {
                errorOccurred = true;
            }
            else if (min != max)
            {
                // If minimum and maximum values are not the same and minimum is odd, it must be
                // increased by one as number of points must be even for both hands exercise
                // If the minimum increased by one is bigger than maximum, it causes error
                if (!min.IsEven())
                {
                    if (min + 1 <= max)
                        min += 1;
                    else
                        errorOccurred = true;
                }
                // If minimum and maximum values are not the same and maximum is odd, it must be
                // increased by one as number of points must be even for both hands exercise
                // If the maximum increased by one is lower than minimum, it causes error
                if (!max.IsEven())
                {
                    if (max - 1 >= min)
                        max -= 1;
                    else
                        errorOccurred = true;
                }
            }
            // If the error did not occur, we divide minimum and maximum values by half as only
            // even values can occur on the sliders
            if (!errorOccurred)
            {
                min /= 2;
                max /= 2;
            }
        }

        return (min, max, errorOccurred);
    }

    /// <summary>
    /// Handles click on Continue button
    /// </summary>
    private void ContinueButtonClicked() {
        // Stores the values in ExerciseInfo object
        exerciseInfo.patientID = patientIDInputField.text;
        exerciseInfo.verticalGameHeight = verticalGamePanel.slider.value / 100f;
        exerciseInfo.selectedHand = selectedHand;
        exerciseInfo.maxHandDistance = maxHandDistancePanel.slider.value / 100f;
        exerciseInfo.fixedExerciseLength = fixedExerciseDistanceToggle.isOn;
        exerciseInfo.exerciseLength = (fixedExerciseDistanceToggle.isOn ? Mathf.Floor(exerciseLengthPanel.slider.value) / 10f : -1f);
        exerciseInfo.numberOfPoints = fixedExerciseDistanceToggle.isOn ? (selectedHand == SelectedHand.BothHands ? (int)numberOfPointsPanel.slider.value * 2 : (int)numberOfPointsPanel.slider.value) : -1;

        // Loads next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Sets value and limits on sliders
    /// </summary>
    private void SetLimitsOnSlider(Slider slider, Limits limits)
    {
        slider.minValue = limits.min;
        slider.maxValue = limits.max;
        slider.value = limits.min;
    }

    /// <summary>
    /// Sets input field text based on slider value
    /// </summary>
    private void SetInputFieldValue(SliderInputFieldTMProPanel panel, int multiplier = 1) {
        panel.inputField.text = ((int)(panel.slider.value * multiplier)).ToString();
    }
}

public struct Limits {
    public float min { get; private set; }
    public float max { get; private set; }

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