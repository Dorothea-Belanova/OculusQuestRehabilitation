using UnityEngine;

[CreateAssetMenu(menuName = "Reif Custom Data/Exercise Info")]
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

    public void Reset()
    {
        patientID = "";
        verticalGameHeight = 0;
        selectedHand = SelectedHand.None;
        maxHandDistance = 0;
        fixedExerciseLength = false;
        exerciseLength = 0;
        numberOfPoints = 0;
        startingTime = "";
        endingTime = "";
        correctedDistance = 0;
        texture = null;
    }
}