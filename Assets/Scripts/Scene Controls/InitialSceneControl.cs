using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitialSceneControl: MonoBehaviour {

    [SerializeField] public InputField patientIDInputField;
    [SerializeField] public Slider verticalGameSlider;
    [SerializeField] public InputField verticalGameInputField;
    [SerializeField] public GameObject segmentedControl;
    [SerializeField] public GameObject afterHandSelectionGroup;
    [SerializeField] public GameObject afterHandSelectionLabelGroup;

    [SerializeField] public Slider maxHandDistanceSlider;
    [SerializeField] public InputField maxHandDistanceInputField;

    [SerializeField] public Toggle fixedExerciseDistanceToggle;

    [SerializeField] public GameObject pointsLengthGroup;
    [SerializeField] public GameObject pointsLengthLabelGroup;
    [SerializeField] public Slider exerciseLengthSlider;
    [SerializeField] public InputField exerciseLengthInputField;
    [SerializeField] public Slider numberOfPointsSlider;
    [SerializeField] public InputField numberOfPointsInputField;
    [SerializeField] public Text numberOfPointsErrorText;
    [SerializeField] public Button continueButton;

    [SerializeField] public GameObject keyboardCanvas;

    [SerializeField] private ExerciseInfo exerciseInfo;

    // STATIC VALUES
    static Limits VERTICAL_GAME_HEIGHT_LIMITS = new Limits(15f, 40f); // with step 1 cm
    static Limits MAX_HAND_DISTANCE_LIMITS = new Limits(35f, 85f); // with step 1 cm
    static Limits EXERCISE_LENGTH_LIMITS = new Limits(12f, 100f); // with step 10 cm
    static float MIN_DISTANCE = 20f;
    static float CALCULATION_MARGIN = 7f;
    static int EXERCISE_LENGTH_STEP = 10;

    private bool keyboardOn = false;

    private void Awake() {
        InitializeUIElements();
        AddListeners();

        //FilterTesting.Test();
    }

    /*private void Update() {
        if(patientIDInputField.isFocused && !keyboardOn) {
            keyboardOn = true;
            Debug.Log("keyboard is on");
            //keyboardCanvas.GetComponent<Image>().CrossFadeAlpha(0.1f, 2.0f, false);
            keyboardCanvas.SetActive(true);
            //StartCoroutine(FadeEffect.FadeCanvas(keyboardCanvas, 0f, 1f, 2f));
            //keyboardCanvas.alpha = 1f;
        }
    }*/

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
        SetLimitsOnSlider(verticalGameSlider, VERTICAL_GAME_HEIGHT_LIMITS);
        SetLimitsOnSlider(maxHandDistanceSlider, MAX_HAND_DISTANCE_LIMITS);
        SetLimitsOnSlider(exerciseLengthSlider, EXERCISE_LENGTH_LIMITS);
        SetNumberOfPointsSlider();

        // Seting Input Fields
        SetInputFieldValue(verticalGameSlider, verticalGameInputField);
        SetInputFieldValue(maxHandDistanceSlider, maxHandDistanceInputField);
        SetInputFieldValue(exerciseLengthSlider, exerciseLengthInputField, EXERCISE_LENGTH_STEP);
        SetInputFieldValue(numberOfPointsSlider, numberOfPointsInputField);

        continueButton.GetComponent<Button>().interactable = false;
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
        verticalGameSlider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(verticalGameSlider, verticalGameInputField);
        });

        // Max Hand Distance Slider
        maxHandDistanceSlider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(maxHandDistanceSlider, maxHandDistanceInputField);
            SetNumberOfPointsSlider();
        });

        // Exercise Length Slider
        exerciseLengthSlider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(exerciseLengthSlider, exerciseLengthInputField, EXERCISE_LENGTH_STEP);
            SetNumberOfPointsSlider();
        });

        // Number of Points Slider
        numberOfPointsSlider.onValueChanged.AddListener(delegate {
            SetInputFieldValue(numberOfPointsSlider, numberOfPointsInputField, AreBothHandsSelected() ? 2 : 1);
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
        Debug.Log("margin: " + AVG_MARGIN);
        Debug.Log("length: " + exerciseLengthSlider.value * 10);
        Debug.Log("min: " + (exerciseLengthSlider.value * 10) / (maxHandDistanceSlider.value - AVG_MARGIN));
        Debug.Log("max: " + (exerciseLengthSlider.value * 10) / (MIN_DISTANCE + AVG_MARGIN));*/
        int min = (int)Math.Ceiling((exerciseLengthSlider.value * EXERCISE_LENGTH_STEP) / (maxHandDistanceSlider.value - CALCULATION_MARGIN));
        int max = (int)Math.Floor((exerciseLengthSlider.value * EXERCISE_LENGTH_STEP) / (MIN_DISTANCE + CALCULATION_MARGIN));

        if(min > max) {
            max = min;
        }
        if(min == max) {
            numberOfPointsSlider.enabled = false;
        }
        else {
            numberOfPointsSlider.enabled = true;
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

        SetLimitsOnSlider(numberOfPointsSlider, limits);
        SetInputFieldValue(numberOfPointsSlider, numberOfPointsInputField, (AreBothHandsSelected() && !errorOccurred) ? 2 : 1);
    }

    private void NumberOfPointsErrorOccurred(bool occurred) {
        numberOfPointsSlider.enabled = !occurred;
        numberOfPointsErrorText.gameObject.SetActive(occurred);
        continueButton.GetComponent<Button>().interactable = !occurred;
    }

    void ContinueButtonClicked() {
        SelectedHand selectedHand = (SelectedHand)segmentedControl.GetComponent<SegmentedControl>().GetSelectedIndex();
        bool bothHands = selectedHand == SelectedHand.BothHands;

        exerciseInfo.patientID = patientIDInputField.text;
        exerciseInfo.verticalGameHeight = verticalGameSlider.value / 100;
        exerciseInfo.selectedHand = selectedHand;
        exerciseInfo.maxHandDistance = maxHandDistanceSlider.value / 100;
        exerciseInfo.fixedExerciseLength = fixedExerciseDistanceToggle.isOn;
        exerciseInfo.exerciseLength = (fixedExerciseDistanceToggle.isOn ? Mathf.Floor(exerciseLengthSlider.value) / 10 : -1f);
        // I NEED TO FIX THIS, AS I AM NOT REACTING TO CHANGE OF SELECTED HAND VALUE
        exerciseInfo.numberOfPoints = fixedExerciseDistanceToggle.isOn ? (bothHands ? (int)numberOfPointsSlider.value * 2 : (int)numberOfPointsSlider.value) : -1;
        //exerciseInfo.numberOfPoints = fixedExerciseDistanceToggle.isOn ? (int)numberOfPointsSlider.value * 2 : -1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion

    private void SetInputFieldValue(Slider slider, InputField inputField, int multiplier = 1) {
        //Debug.Log("multiplier: " + multiplier);
        inputField.text = ((int)(slider.value * multiplier)).ToString();
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