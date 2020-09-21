using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LogDataSceneControl : MonoBehaviour
{
    [SerializeField] private ExerciseInfo exerciseInfo;
    [SerializeField] private TextMeshProUGUI patientIDText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private RawImage image;

    void Start()
    {
        SetUI();
    }

    private void SetUI()
    {
        patientIDText.text = exerciseInfo.patientID;
        distanceText.text = Mathf.Round(exerciseInfo.correctedDistance * 100f) / 100f + " m";

        //image.GetComponent<RectTransform>().sizeDelta = new Vector2(exerciseInfo.texture.width, exerciseInfo.texture.height);
        image.texture = exerciseInfo.texture;
    }

    public void BackButtonPressed()
    {
        Debug.Log("KLIK NA ZACIATOK");
        SceneManager.LoadScene(0);
    }
}
