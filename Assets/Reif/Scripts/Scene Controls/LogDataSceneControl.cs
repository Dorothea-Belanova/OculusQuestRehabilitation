using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LogDataSceneControl : MonoBehaviour
{
    [SerializeField] private ExerciseInfo exerciseInfo;
    [SerializeField] private TextMeshProUGUI patientIDText;
    [SerializeField] private TextMeshProUGUI startTimeText;
    [SerializeField] private TextMeshProUGUI endTimeText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private RawImage image;

    void Start() => SetUI();

    private void SetUI()
    {
        patientIDText.text = exerciseInfo.patientID;
        startTimeText.text = exerciseInfo.startingTime;
        endTimeText.text = exerciseInfo.endingTime;
        distanceText.text = Mathf.Round(exerciseInfo.correctedDistance * 100f) / 100f + " m";
        image.texture = exerciseInfo.texture;
    }

    public void BackButtonPressed() => SceneManager.LoadScene(0);
}
