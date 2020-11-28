using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRandomizerTesting : MonoBehaviour
{
    [SerializeField] private ExerciseInfo exerciseInfo;

    public void Awake()
    {
        exerciseInfo.patientID = "reed";
        exerciseInfo.verticalGameHeight = 0.15f;
        exerciseInfo.selectedHand = SelectedHand.LeftHand;
        exerciseInfo.maxHandDistance = 0.62f;
        exerciseInfo.numberOfPoints = 5;
        exerciseInfo.exerciseLength = 1.4f;
        exerciseInfo.fixedExerciseLength = true;

        SetPointRandomizer();
    }

    void SetPointRandomizer()
    {
        var isFixedExerciseLength = exerciseInfo.fixedExerciseLength;
        float[] distances;

        if (isFixedExerciseLength)
        {
            var min = 0.25f;
            var max = exerciseInfo.maxHandDistance;
            var length = exerciseInfo.exerciseLength;
            var points = exerciseInfo.numberOfPoints;

            if (exerciseInfo.selectedHand == SelectedHand.BothHands)
            {
                length /= 2;
                points /= 2;
            }


            Debug.Log("PRUSER");
            distances = PointRandomizer.GenerateDistances(min, max, length, points);

            for (int i = 0; i < distances.Length; ++i)
            {
                Debug.Log(i + ": " + distances[i]);
            }

            if (exerciseInfo.selectedHand == SelectedHand.BothHands)
            {
                var newDistances = new float[points * 2];
                for (int i = 0; i < points; ++i)
                {
                    Debug.Log("i: " + i);
                    newDistances[i * 2] = distances[i];
                    newDistances[i * 2 + 1] = distances[i];
                    Debug.Log("managed");
                }
                distances = newDistances;
            }
        }
    }
}
