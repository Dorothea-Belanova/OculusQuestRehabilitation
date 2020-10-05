using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ErrorCorrection;
using UnityEngine.SceneManagement;

public class ExerciseSceneControl : MonoBehaviour {

    // Static Variables
    private static Color LEFT_HAND_COLOR = new Color(0.0f, 0.2f, 0.2f);
    private static Color RIGHT_HAND_COLOR = new Color(1.0f, 0.5f, 0.0f);
    private static float RADIUS = 0.03f; // nastavene v unity editore
    private static float CORNER_OFFSET = 0.05f;

    [SerializeField] GameObject leftHand;
    [SerializeField] public GameObject workspace;
    List<Vector3> leftPositions;
    List<Vector3> rightPositions;
    [SerializeField] GameObject initialPositionSphere;
    [SerializeField] TextMeshPro scoreText;
    [SerializeField] TextMeshPro count;
    [SerializeField] GameObject spherePrefab;
    [SerializeField] GameObject cylinderPrefab;
    [SerializeField] GameObject table;
    [SerializeField] public GameObject tableTop;
    [SerializeField] GameObject alarm;

    [HideInInspector] public InitialPositionSphere leftHandInteractiveSphere;
    [HideInInspector] public InitialPositionSphere rightHandInteractiveSphere;

    private Vector3 leftHandInitialPos;
    private Vector3 rightHandInitialPos;
    GameObject leftPositionSphere;
    GameObject rightPositionSphere;

    // POINT RANDOMIZER
    float[] distances;
    bool isFixedExerciseLength;
    int testingPointIndex = -1;

    bool set = false;
    float distance = 0f;
    Vector3 previousHandPosition = Vector3.zero;
    bool alternateHands;

    private bool started = false;
    public bool isLeftActive = false;
    private bool cylinderInScene = false;
    private bool countingDistanceForLeft = true;
    private int numOfPoints = 0;
    private int score = 0;
    private int positionCount = 0;
    private float verticalGameHeight = 0;
    private bool isInitialized = false;
    private bool verticalGame = false;

    private bool isSessionOn = false;
    private float sessionTime = 0f;
    private Coroutine sessionCoroutine;
    private Coroutine gameCoroutine;

    GameObject cylinderInstance;
    GameObject alarmInstance;

    ErrorCorrectionManager errorCorrection;

    [SerializeField] private ExerciseInfo exerciseInfo;

    public void Awake()
    {
        Debug.Log("EXERCISE INFO");
        Debug.Log("name: " + (exerciseInfo.patientID));
        Debug.Log("vertical height: " + (exerciseInfo.verticalGameHeight));
        Debug.Log("max distance: " + (exerciseInfo.maxHandDistance));
        Debug.Log("number of points: " + (exerciseInfo.numberOfPoints));
        Debug.Log("exerciseLength: " + (exerciseInfo.exerciseLength));

        /*exerciseInfo.patientID = "reed";
        exerciseInfo.verticalGameHeight = 0.15f;
        exerciseInfo.selectedHand = SelectedHand.BothHands;
        exerciseInfo.maxHandDistance = 0.35f;
        exerciseInfo.numberOfPoints = 16;
        exerciseInfo.exerciseLength = 4.4f;
        exerciseInfo.fixedExerciseLength = true;*/

        //exerciseInfo = new ExerciseInfo("Dori", 0.2f, SelectedHand.BothHands, 0.4f, true, 4f, 6);
        alternateHands = exerciseInfo.selectedHand == SelectedHand.BothHands;
        if(!alternateHands)
            isLeftActive = exerciseInfo.selectedHand == SelectedHand.LeftHand;
        ChangeTableSize();
        SetPointRandomizer();

        //Debug.Log("length: " + clickClip.length);
    }

    void SetPointRandomizer()
    {
        isFixedExerciseLength = exerciseInfo.fixedExerciseLength;
        if (isFixedExerciseLength)
        {
            var min = 0.2f;
            var max = exerciseInfo.maxHandDistance;
            var length = exerciseInfo.exerciseLength;
            var points = exerciseInfo.numberOfPoints;

            if (exerciseInfo.selectedHand == SelectedHand.BothHands)
            {
                length /= 2;
                points /= 2;
            }

            distances = PointRandomizer.GenerateDistances(min, max, length, points);
            if (distances[points - 1] < min || distances[points - 1] > max)
            {
                distances = PointRandomizer.GenerateDistances(min, max, length, points);
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("starting calibration");
            StartCoroutine(InitialPositionCalibration());
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("starting game");
            gameCoroutine = StartCoroutine(Game());
        }

        if (gameCoroutine != null)
        {
            (var left, var right) = GetHandsPosition();
            var position = isLeftActive ? left : right;
            float correctedDistance = errorCorrection.NewValue(Camera.main.transform.position, position);
            SetCorrectedDistance(correctedDistance);
        }
    }

    private void SetCorrectedDistance(float distance)
    {
        count.text = "Distance: " + Mathf.Round(distance * 100f) / 100f + " m";
    }

    private void CreateCylinder()
    {
        Debug.Log("CREATING CYLINDER");
        if (!verticalGame)
        {
            Vector3 position;

            // FIXED EXERCISE LENGTH
            if (isFixedExerciseLength)
            {
                ++testingPointIndex;
                position = PointRandomizer.GenerateRandomPointPosition(distances[testingPointIndex], GetActiveHandPosition(), tableTop);
                position.y -= 0.02f;
            }
            // RANDOM POINTS GENERATION
            else
            {
                //float minHandDistance = 0.15f;
                //float tableWidth = table.transform.localScale.x / 100;
                //float radius = cylinder.transform.lossyScale.x;
                //float x = Random.Range(-tableWidth / 2 + radius, tableWidth / 2 - radius);
                //float z = Random.Range(radius, applicationControl.rehabilitationInfo.maxHandDistance - minHandDistance - radius);
                position = PointRandomizer.GenerateRandomPointPosition(Random.Range(0.2f, exerciseInfo.maxHandDistance - 0.05f), GetActiveHandPosition(), tableTop);
                //position.y -= 0.02f;
                //position = new Vector3(table.transform.position.x + x, leftHandInitialPos.y - 0.02f, leftHandInitialPos.z + minHandDistance + z);
            }

            cylinderInstance = GameObject.Instantiate(cylinderPrefab, position, Quaternion.identity);
            Debug.Log("CYLINDER CREATED");
        }
        else
        {
            // VERTICAL GAME
            float randomX = Random.Range(0, 0.2f);
            float x;
            float middleX = (leftHandInitialPos.x + rightHandInitialPos.x) / 2f;
            if (isLeftActive)
            {
                x = middleX - randomX;
            }
            else
            {
                x = middleX + randomX;
            }
            float y = Random.Range(verticalGameHeight - 0.1f, verticalGameHeight + 0.1f);
            cylinderInstance = GameObject.Instantiate(cylinderPrefab, new Vector3(x, leftHandInitialPos.y + y, leftHandInitialPos.z + 0.2f), Quaternion.identity);
            cylinderInstance.transform.Rotate(90f, 0, 0);
        }

        cylinderInScene = true;

        if (isLeftActive)
        {
            cylinderInstance.GetComponent<Renderer>().material.color = LEFT_HAND_COLOR;
            cylinderInstance.GetComponent<InteractiveCylinder>().isLeft = true;
        }
        else
        {
            cylinderInstance.GetComponent<Renderer>().material.color = RIGHT_HAND_COLOR;
            cylinderInstance.GetComponent<InteractiveCylinder>().isLeft = false;
        }
    }

    private Vector3 GetActiveHandPosition()
    {
        return (isLeftActive ? leftHandInitialPos : rightHandInitialPos);
    }

    public void CylinderDestroyed(GameObject cylinder)
    {
        cylinderInScene = false;

        if (alternateHands)
        {
            isLeftActive = !isLeftActive;
        }
        else
        {
            isLeftActive = exerciseInfo.selectedHand == SelectedHand.LeftHand ? true : false;
        }

        ++score;
        SetScore();

        /*if(isSessionOn) {
            StopCoroutine(sessionCoroutine);
            sessionCoroutine = null;
            applicationControl.rehabilitationInfo.sessionTime = sessionTime;
            errorCorrection.StopDataCollection(markerFrames);
            SceneLoader.LoadNextScene();
        }*/

        if (exerciseInfo.fixedExerciseLength && score >= exerciseInfo.numberOfPoints)
        {
            errorCorrection.StopDataCollection();
            if(gameCoroutine != null)
            {
                StopCoroutine(gameCoroutine);
                gameCoroutine = null;
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //errorCorrection.StopDataCollection(markerFrames);
            //StartCoroutine(WaitAndGoToNextScene());
        }

        Destroy(cylinder);
    }

    private void SetScore()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private IEnumerator Game()
    {
        if (!alternateHands)
        {
            isLeftActive = exerciseInfo.selectedHand == SelectedHand.LeftHand ? true : false;
        }

        (var left, var right) = GetHandsPosition();
        var position = isLeftActive ? left : right;
        errorCorrection.UpdateMeasurement(position, "L", exerciseInfo);

        //StartCoroutine(HandTranslationCoroutine());
        //StartCoroutine(ControlCues());

        //bool isLeftHandSelected = exerciseInfo.selectedHand == SelectedHand.LeftHand ? true : false;
        for (; ; )
        {
            if (alarmInstance != null)
            {
                yield return null;
            }

            //Debug.Log("hram");
            if (alternateHands)
            {

                //Debug.Log(leftHandInteractiveSphere.IntersectsHand() ? "left intersects" : "left does not intersect");
                //Debug.Log(rightHandInteractiveSphere.IntersectsHand() ? "left intersects" : "left does not intersect");

                if (leftHandInteractiveSphere.IntersectsHand() && rightHandInteractiveSphere.IntersectsHand() && !cylinderInScene)
                {
                    //lastHandPosition = new Vector3();
                    CreateCylinder();
                    Debug.Log("ABOUT TO CREATE A CYLINDER");
                    //lineRenderer.positionCount = 0;
                    //countingDistanceForLeft = !countingDistanceForLeft;
                }
            }
            else
            {
                if (!cylinderInScene && ((leftHandInteractiveSphere.IntersectsHand() && isLeftActive) || (rightHandInteractiveSphere.IntersectsHand() && !isLeftActive)))
                {
                    //lastHandPosition = new Vector3();
                    CreateCylinder();
                    //lineRenderer.positionCount = 0;
                    //countingDistanceForLeft = isLeftHandSelected ? true : false;
                }
            }

            yield return null;
        }
    }

    IEnumerator InitialPositionCalibration()
    {
        yield return new WaitForSeconds(2f);

        do
        {
            yield return new WaitForFixedUpdate();
        } while (!HandsManager.Instance.IsInitialized());

        yield return new WaitForSeconds(1f);

        float time = 0;
        leftPositions = new List<Vector3>();
        rightPositions = new List<Vector3>();

        while (time < 5f)
        {
            time += Time.deltaTime;
            (Vector3 left, Vector3 right) = GetHandsPosition();

            if (HandsManager.Instance.IsInitialized() && !IsNegativeInfinity(left) && !IsNegativeInfinity(right))
            {

                leftPositions.Add(left);
                rightPositions.Add(right);
            }

            //scoreText.text = "Time: " + time.ToString();
            //count.text = "Count: " + rightPositions.Count;
            yield return new WaitForFixedUpdate();
        }

        leftHandInitialPos = GetAverageVector(leftPositions);
        rightHandInitialPos = GetAverageVector(rightPositions);

        float y = (leftHandInitialPos.y + rightHandInitialPos.y) / 2f;
        leftHandInitialPos.y = y;
        rightHandInitialPos.y = y;

        /*leftHandInitialPos = new Vector3(-0.09047802f, -0.1860618f, -0.7799741f);
        rightHandInitialPos = new Vector3(0.1484186f, -0.1860618f, -0.7998919f);

        float y = leftHandInitialPos.y;*/

        if (!leftHandInitialPos.IsNan() && !rightHandInitialPos.IsNan())
        {
            if (leftPositionSphere == null)
            {
                leftPositionSphere = GameObject.Instantiate(initialPositionSphere, new Vector3(leftHandInitialPos.x, y - 0.02f, leftHandInitialPos.z), Quaternion.identity);
                leftPositionSphere.GetComponent<Renderer>().material.color = LEFT_HAND_COLOR;
                leftHandInteractiveSphere = leftPositionSphere.GetComponent<InitialPositionSphere>();
                leftHandInteractiveSphere.isLeft = true;
            }
            else
            {
                leftPositionSphere.transform.position = leftHandInitialPos;
            }

            if (rightPositionSphere == null)
            {
                rightPositionSphere = GameObject.Instantiate(initialPositionSphere, new Vector3(rightHandInitialPos.x, y - 0.02f, rightHandInitialPos.z), Quaternion.identity);
                rightPositionSphere.GetComponent<Renderer>().material.color = RIGHT_HAND_COLOR;
                rightHandInteractiveSphere = rightPositionSphere.GetComponent<InitialPositionSphere>();
                rightHandInteractiveSphere.isLeft = false;
            }
            else
            {
                rightPositionSphere.transform.position = rightHandInitialPos;
            }

            errorCorrection = new ErrorCorrectionManager(leftPositions, rightPositions);

            AdjustWorkspace();
            Debug.Log("starting finished");

            //StartCoroutine(CalculateDistance());

            /*Debug.Log("GAME STARTED");

            Debug.Log("LEFT: ");
            Debug.Log("X: " + leftHandInitialPos.x);
            Debug.Log("Y: " + leftHandInitialPos.y);
            Debug.Log("Z: " + leftHandInitialPos.z);

            Debug.Log("RIGHT: ");
            Debug.Log("X: " + rightHandInitialPos.x);
            Debug.Log("Y: " + rightHandInitialPos.y);
            Debug.Log("Z: " + rightHandInitialPos.z);*/

        }
    }

    IEnumerator CalculateDistance()
    {
        while (true)
        {
            (Vector3 left, Vector3 right) = GetHandsPosition();

            if (previousHandPosition == Vector3.zero)
            {
                previousHandPosition = left;
            }
            else
            {
                distance += Vector3.Distance(previousHandPosition, left);
                previousHandPosition = left;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    (Vector3, Vector3) GetHandsPosition()
    {
        if (!HandsManager.Instance.IsInitialized())
        {
            return (Vector3.negativeInfinity, Vector3.negativeInfinity);
        }

        //bool leftHandIsReliable = Hands.Instance.LeftHand.HandConfidence == Hand.HandTrackingConfidence.High;
        //bool rightHandIsReliable = Hands.Instance.RightHand.HandConfidence == Hand.HandTrackingConfidence.High;

        /*IList<OVRBone> leftHandBones = HandsManager.Instance.LeftHandSkeleton.Bones;
        IList<OVRBone> rightHandBones = Hands.Instance.RightHand.Skeleton.Bones;
        Vector3 leftPosition = (leftHandBones[3].transform.position + leftHandBones[17].transform.position) / 2f;
        Vector3 rightPosition = (rightHandBones[3].transform.position + rightHandBones[17].transform.position) / 2f;*/

        IList<OVRBone> leftHandBones = HandsManager.Instance.LeftHandSkeleton.Bones;
        IList<OVRBone> rightHandBones = HandsManager.Instance.RightHandSkeleton.Bones;
        Vector3 leftPosition = (leftHandBones[1].Transform.position + leftHandBones[10].Transform.position) / 2f;
        Vector3 rightPosition = (rightHandBones[1].Transform.position + rightHandBones[10].Transform.position) / 2f;

        return (leftPosition, rightPosition);
    }

    Vector3 GetAverageVector(List<Vector3> positions)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < positions.Count; i++)
        {
            sum += positions[i];
        }

        return sum / positions.Count;
    }

    bool IsNegativeInfinity(Vector3 position)
    {
        return float.IsNegativeInfinity(position.x) && float.IsNegativeInfinity(position.y) && float.IsNegativeInfinity(position.z);
    }

    private void ChangeTableSize()
    {
        float tableMax = exerciseInfo.maxHandDistance + CORNER_OFFSET * 2f;
        float difference = ((tableMax - workspace.transform.GetChild(0).transform.localScale.z) / 2);
        workspace.transform.GetChild(0).transform.localScale = new Vector3(workspace.transform.GetChild(0).transform.localScale.x, workspace.transform.GetChild(0).transform.localScale.y, tableMax);
        workspace.transform.GetChild(0).transform.Translate(0, 0, difference);
        table.transform.localScale = new Vector3(table.transform.localScale.x, table.transform.localScale.y, tableMax * 100f);
        table.transform.Translate(0, 0, -difference);
    }

    private void AdjustWorkspace()
    {
        float z = ((leftHandInitialPos.z + rightHandInitialPos.z) / 2f) + 0.15f - CORNER_OFFSET;
        float x = (leftHandInitialPos.x + rightHandInitialPos.x) / 2f;
        float y = (leftHandInitialPos.y + rightHandInitialPos.y) / 2f - CORNER_OFFSET / 2f;

        count.text = x.ToString() + "/" + y.ToString() + "/" + z.ToString();

        workspace.transform.position = new Vector3(x, y, z);
    }

    public void StartPressed()
    {
        if (!leftHandInitialPos.IsNan() && !rightHandInitialPos.IsNan() && leftPositionSphere != null && rightPositionSphere != null)
        {
            Debug.Log("GAME STARTED");
            gameCoroutine = StartCoroutine(Game());
        }
    }

    public void CalibrationPressed()
    {
        StartCoroutine(InitialPositionCalibration());
    }

    public void VerticalGameTogglePressed()
    {
        if (leftPositionSphere != null && rightPositionSphere != null)
        {
            //Debug.Log("prechadza");
            // if vertical game is turned on, put it back to horizontal
            Debug.Log("verticalGameHeight: " + verticalGameHeight);
            verticalGameHeight = exerciseInfo.verticalGameHeight;
            if (verticalGame)
            {
                verticalGame = !verticalGame;
                table.SetActive(true);
                tableTop.SetActive(true);
                if (cylinderInstance != null)
                {
                    Object.Destroy(cylinderInstance);
                    cylinderInScene = false;
                }
                //workspace.GetComponent<Renderer>().enabled = true;
                leftHandInteractiveSphere.transform.Translate(new Vector3(0f, -1 * verticalGameHeight, 0f));
                rightHandInteractiveSphere.transform.Translate(new Vector3(0f, -1 * verticalGameHeight, 0f));
            }
            else
            {
                verticalGame = !verticalGame;
                table.SetActive(false);
                tableTop.SetActive(false);
                if (cylinderInstance != null)
                {
                    Object.Destroy(cylinderInstance);
                    cylinderInScene = false;
                }
                //workspace.GetComponent<Renderer>().enabled = false;
                leftHandInteractiveSphere.transform.Translate(new Vector3(0f, verticalGameHeight, 0f));
                rightHandInteractiveSphere.transform.Translate(new Vector3(0f, verticalGameHeight, 0f));
            }
        }
    }

    public void AlarmPressed()
    {
        if (alarmInstance == null)
        {
            if (cylinderInScene)
            {
                cylinderInstance.SetActive(false);
            }

            Vector3 alarmPosition = new Vector3((leftHandInteractiveSphere.transform.position.x + rightHandInteractiveSphere.transform.position.x) / 2f, leftHandInteractiveSphere.transform.position.y + 0.04f, leftHandInteractiveSphere.transform.position.z + 0.15f);

            alarmInstance = GameObject.Instantiate(alarm, alarmPosition, Quaternion.identity);
            alarmInstance.transform.Rotate(180, 0, 0);
            //clockInstance.transform.Translate(0f, -0.04f, 0);
        }

        //InteractiveAlarmButton alarmButton = alarmInstance.transform.Find("group5/botan").GetComponent<InteractiveAlarmButton>();
        //alarmButton.StartRinging();
    }

    public void QuitGamePresssed()
    {
        Debug.Log("QUIT PRESSED");
        if(gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
            gameCoroutine = null;
            errorCorrection.StopDataCollection();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
