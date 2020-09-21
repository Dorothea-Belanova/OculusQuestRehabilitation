using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ExerciseInfo")]
public class ExerciseInfo : ScriptableObject
{
    public string patientID;
    public float verticalGameHeight;
    public SelectedHand selectedHand;
    public float maxHandDistance;
    public bool fixedExerciseLength;
    public float exerciseLength;
    public int numberOfPoints;
    public string startingTime;
    public string endingTime;
    public float correctedDistance;
    public Texture2D texture;
}