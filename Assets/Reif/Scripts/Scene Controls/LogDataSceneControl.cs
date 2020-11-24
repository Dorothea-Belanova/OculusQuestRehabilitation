using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class LogDataSceneControl : MonoBehaviour
{
    [SerializeField] private ExerciseInfo exerciseInfo;
    [SerializeField] private TextMeshProUGUI patientIDText;
    [SerializeField] private TextMeshProUGUI startTimeText;
    [SerializeField] private TextMeshProUGUI endTimeText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private RawImage image;

    void Start()
    {
        SetUI();
        //TryCSV();
        var data = CSVManager.Load("test");
        Debug.Log(data[0][7] + ": " + data[1][7]);
    }

    private void SetUI()
    {
        patientIDText.text = exerciseInfo.patientID;
        startTimeText.text = exerciseInfo.startingTime;
        endTimeText.text = exerciseInfo.endingTime;
        distanceText.text = Mathf.Round(exerciseInfo.correctedDistance * 100f) / 100f + " m";

        //image.GetComponent<RectTransform>().sizeDelta = new Vector2(exerciseInfo.texture.width, exerciseInfo.texture.height);
        image.texture = exerciseInfo.texture;
    }

    private void TryCSV()
    {
        List<string[]> data = new List<string[]>();

        string[] paths = BetterStreamingAssets.GetFiles("Data", "test.csv", SearchOption.AllDirectories);
        if (paths.Length > 0)
            Debug.Log("NASLO");

        string[] reader = BetterStreamingAssets.ReadAllLines(paths[0]);


            foreach(var line in reader)
            {
                var values = line.Split(',');

                string[] dataTemp = new string[values.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    dataTemp[i] = values[i];
                }
                data.Add(dataTemp);
            }

        Debug.Log("START TIME: " + data[0][7]);
    }

    public void BackButtonPressed()
    {
        Debug.Log("KLIK NA ZACIATOK");
        SceneManager.LoadScene(0);
    }
}
