using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ErrorCorrection;
using UnityEngine.UI;

public class TestingErrorCorrection : MonoBehaviour
{
    [SerializeField] private ExerciseInfo exerciseInfo;
    [SerializeField] private RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        exerciseInfo.maxHandDistance = 0.35f;
        var data = CSVManager.Load("C:/Users/dortb/Desktop/1_left.csv");

        List<Vector3> handPosition = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(0, 0.1f, 0f)};

        var correction = new ErrorCorrectionManager(handPosition, handPosition);

        for(int i = 1; i < data.Count; ++i)
        {
            float x = float.Parse(data[i][1]);
            float y = float.Parse(data[i][2]);
            float z = float.Parse(data[i][3]);
            Vector3 cameraPosition = new Vector3(x, y, z);

            x = float.Parse(data[i][5]);
            y = float.Parse(data[i][6]);
            z = float.Parse(data[i][7]);

            Vector3 position = new Vector3(x, y, z);

            if (i == 1)
            {
                correction.UpdateMeasurement(cameraPosition, position, "L", exerciseInfo);
            }
            else
            {
                correction.NewValue(cameraPosition, position);
            }
        }
        correction.StopDataCollection();

        Debug.Log("koniec");

        image.texture = exerciseInfo.texture;
    }
}
