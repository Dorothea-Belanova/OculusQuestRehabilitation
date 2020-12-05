using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ErrorCorrection;
using UnityEngine.SceneManagement;

public class ExerciseSceneControl : MonoBehaviour {

    [SerializeField] private GameObject workspace;
    [SerializeField] private GameObject initialPositionSpherePrefab;
    [SerializeField] private TextMeshPro scoreValue;
    [SerializeField] private TextMeshPro distanceValue;
    [SerializeField] private GameObject cylinderPrefab;
    [SerializeField] private GameObject table;
    [SerializeField] public  GameObject tableTop;
    [SerializeField] private GameObject alarmPrefab;
    [SerializeField] private ExerciseInfo exerciseInfo;

    [HideInInspector] public InitialPositionSphere leftHandInteractiveSphere;
    [HideInInspector] public InitialPositionSphere rightHandInteractiveSphere;

    private Vector3 leftHandInitialPos;
    private Vector3 rightHandInitialPos;
    private GameObject leftPositionSphere;
    private GameObject rightPositionSphere;

    // POINT RANDOMIZER
    private float[] distances;

    private bool alternateHands;
    private bool startedComputingDistance = false;
    private bool isLeftActive = false;
    private bool isLeftComputing = false;
    private bool isCylinderInScene = false;
    private int score = 0;
    private float verticalGameHeight = 0;
    private bool verticalGame = false;

    private Coroutine gameCoroutine;

    private GameObject cylinderInstance;
    private GameObject alarmInstance;

    private ErrorCorrectionManager errorCorrection;

    public void Awake()
    {
        alternateHands = exerciseInfo.selectedHand == SelectedHand.BothHands;
        if(!alternateHands)
            isLeftActive = exerciseInfo.selectedHand == SelectedHand.LeftHand;
        ChangeTableSize();
        SetPointRandomizer();
    }

    /// <summary>
    /// Sets point randomizer and fills calculated distances into an array
    /// </summary>
    void SetPointRandomizer()
    {
        if (exerciseInfo.fixedExerciseLength)
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

            int trialIndex = -1;
            do
            {
                ++trialIndex;
                distances = PointRandomizer.GenerateDistances(min, max, length, points);
            } while (distances == null && trialIndex < 10);

            if(distances == null && trialIndex >= 10)
            {
                for (int i = 0; i < points; ++i)
                    distances[i] = length / (float)points;
            }

            if (exerciseInfo.selectedHand == SelectedHand.BothHands)
            {
                var newDistances = new float[points * 2];
                for (int i = 0; i < points; ++i)
                {
                    newDistances[i * 2] = distances[i];
                    newDistances[i * 2 + 1] = distances[i];
                } 
                distances = newDistances;
            }
        }
    }

    /// <summary>
    /// Handles calculation of corrected distance
    /// </summary>
    public void Update()
    {
        if (gameCoroutine != null && startedComputingDistance)
        {
            (var left, var right) = GetHandsPosition();
            var position = isLeftComputing ? left : right;
            float correctedDistance = errorCorrection.NewValue(Camera.main.transform.position, position, isLeftComputing ? "L" : "R");
            SetCorrectedDistance(correctedDistance);
        }       
    }

    /// <summary>
    /// Changes corrected distance value on a panel
    /// </summary>
    private void SetCorrectedDistance(float distance)
    {
        distanceValue.text = Mathf.Round(distance * 100f) / 100f + " m";
    }

    /// <summary>
    /// Handles cylinder creation
    /// </summary>
    private void CreateCylinder()
    {
        // If patient chose fixed length exercise and already collected selected number of points,
        // it stops the game
        if (exerciseInfo.fixedExerciseLength && score >= exerciseInfo.numberOfPoints)
        {
            errorCorrection.StopDataCollection();

            if (gameCoroutine != null)
            {
                StopCoroutine(gameCoroutine);
                gameCoroutine = null;
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        // If this is the first time to create a cylinder, it starts calculation of distance
        if (!startedComputingDistance)
        {
            isLeftComputing = isLeftActive;

            (var left, var right) = GetHandsPosition();
            var position = isLeftComputing ? left : right;
            errorCorrection.UpdateMeasurement(position, isLeftComputing ? "L" : "R", exerciseInfo);
            startedComputingDistance = true;
        }
        else if(alternateHands)
            isLeftComputing = !isLeftComputing;

        // PART CREATING CYLINDER
        if (!verticalGame)
        {
            Vector3 position;

            // FIXED EXERCISE LENGTH
            if (exerciseInfo.fixedExerciseLength)
            {
                position = PointRandomizer.GenerateRandomPointPosition(distances[score], GetActiveHandInitialPosition(), tableTop);
                position.y -= 0.02f;
            }
            // RANDOM POINTS GENERATION
            else
            {
                position = PointRandomizer.GenerateRandomPointPosition(Random.Range(0.25f, exerciseInfo.maxHandDistance - 0.05f), GetActiveHandInitialPosition(), tableTop);
            }

            cylinderInstance = GameObject.Instantiate(cylinderPrefab, position, Quaternion.identity);
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

        isCylinderInScene = true;

        if (isLeftActive)
        {
            cylinderInstance.GetComponent<Renderer>().material.color = Constants.LEFT_HAND_COLOR;
            cylinderInstance.GetComponent<InteractiveCylinder>().isLeft = true;
        }
        else
        {
            cylinderInstance.GetComponent<Renderer>().material.color = Constants.RIGHT_HAND_COLOR;
            cylinderInstance.GetComponent<InteractiveCylinder>().isLeft = false;
        }
    }

    /// <summary>
    /// Returns initial position of an active hand
    /// </summary>
    private Vector3 GetActiveHandInitialPosition()
    {
        return (isLeftActive ? leftHandInitialPos : rightHandInitialPos);
    }

    /// <summary>
    /// Handles cylinder being destroyed in a scene
    /// </summary>
    public void CylinderDestroyed(GameObject cylinder)
    {
        isCylinderInScene = false;

        if (alternateHands)
            isLeftActive = !isLeftActive;
        else
            isLeftActive = exerciseInfo.selectedHand == SelectedHand.LeftHand ? true : false;

        ++score;
        SetScore();

        Destroy(cylinder);
    }

    /// <summary>
    /// Changes score value on a panel
    /// </summary>
    private void SetScore() => scoreValue.text = score.ToString();

    /// <summary>
    /// Handles game actions
    /// </summary>
    private IEnumerator Game()
    {
        if (!alternateHands)
            isLeftActive = exerciseInfo.selectedHand == SelectedHand.LeftHand ? true : false;

        for (; ; )
        {
            if (alarmInstance != null)
                yield return null;

            if (alternateHands)
            {
                if (leftHandInteractiveSphere.IntersectsHand() && rightHandInteractiveSphere.IntersectsHand() && !isCylinderInScene)
                    CreateCylinder();
            }
            else
            {
                if (!isCylinderInScene && ((leftHandInteractiveSphere.IntersectsHand() && isLeftActive) || (rightHandInteractiveSphere.IntersectsHand() && !isLeftActive)))
                    CreateCylinder();
            }

            yield return null;
        }
    }

    /// <summary>
    /// Handles initial position calibration
    /// </summary>
    IEnumerator InitialPositionCalibration()
    {
        yield return new WaitForSeconds(2f);

        // Waints until both hands are initialized
        do
        {
            yield return new WaitForFixedUpdate();
        } while (!HandsManager.Instance.IsInitialized());

        float time = 0;
        var leftInitialPositions = new List<Vector3>();
        var rightInitialPositions = new List<Vector3>();

        // Collects hand positions over a period of 5 seconds
        while (time < 5f)
        {
            time += Time.deltaTime;
            (Vector3 left, Vector3 right) = GetHandsPosition();

            if (HandsManager.Instance.IsInitialized() && !left.IsNegativeInfinity() && !left.IsNegativeInfinity())
            {
                leftInitialPositions.Add(left);
                rightInitialPositions.Add(right);
            }

            yield return new WaitForFixedUpdate();
        }

        // Calculates initial positions as an average of initial positions
        leftHandInitialPos = GetAverageVector(leftInitialPositions);
        rightHandInitialPos = GetAverageVector(rightInitialPositions);

        float y = (leftHandInitialPos.y + rightHandInitialPos.y) / 2f;
        leftHandInitialPos.y = y;
        rightHandInitialPos.y = y;

        if (!leftHandInitialPos.IsNan() && !rightHandInitialPos.IsNan())
        {
            if (leftPositionSphere == null)
            {
                leftPositionSphere = GameObject.Instantiate(initialPositionSpherePrefab, new Vector3(leftHandInitialPos.x, y - 0.02f, leftHandInitialPos.z), Quaternion.identity);
                leftPositionSphere.GetComponent<Renderer>().material.color = Constants.LEFT_HAND_COLOR;
                leftHandInteractiveSphere = leftPositionSphere.GetComponent<InitialPositionSphere>();
                leftHandInteractiveSphere.isLeft = true;
            }
            else
            {
                leftPositionSphere.transform.position = leftHandInitialPos;
            }

            if (rightPositionSphere == null)
            {
                rightPositionSphere = GameObject.Instantiate(initialPositionSpherePrefab, new Vector3(rightHandInitialPos.x, y - 0.02f, rightHandInitialPos.z), Quaternion.identity);
                rightPositionSphere.GetComponent<Renderer>().material.color = Constants.RIGHT_HAND_COLOR;
                rightHandInteractiveSphere = rightPositionSphere.GetComponent<InitialPositionSphere>();
                rightHandInteractiveSphere.isLeft = false;
            }
            else
            {
                rightPositionSphere.transform.position = rightHandInitialPos;
            }

            // Sets error correction
            errorCorrection = new ErrorCorrectionManager(leftInitialPositions, rightInitialPositions);

            AdjustWorkspace();
        }
    }

    /// <summary>
    /// Returns tuple of left and right hands positions
    /// </summary>
    (Vector3, Vector3) GetHandsPosition()
    {
        if (!HandsManager.Instance.IsInitialized())
            return (Vector3.negativeInfinity, Vector3.negativeInfinity);

        IList<OVRBone> leftHandBones = HandsManager.Instance.LeftHandSkeleton.Bones;
        IList<OVRBone> rightHandBones = HandsManager.Instance.RightHandSkeleton.Bones;

        // First and tenth bone are used for calculation of palm position
        Vector3 leftPosition = (leftHandBones[1].Transform.position + leftHandBones[10].Transform.position) / 2f;
        Vector3 rightPosition = (rightHandBones[1].Transform.position + rightHandBones[10].Transform.position) / 2f;

        return (leftPosition, rightPosition);
    }

    /// <summary>
    /// Gets average vector value from a list of positions
    /// </summary>
    Vector3 GetAverageVector(List<Vector3> positions)
    {
        Vector3 sum = Vector3.zero;

        for (int i = 0; i < positions.Count; i++)
            sum += positions[i];

        return sum / positions.Count;
    }

    /// <summary>
    /// Changes table size based on max hand distance position
    /// </summary>
    private void ChangeTableSize()
    {
        float tableMax = exerciseInfo.maxHandDistance + Constants.CORNER_OFFSET * 2f;
        float difference = ((tableMax - workspace.transform.GetChild(0).transform.localScale.z) / 2);
        workspace.transform.GetChild(0).transform.localScale = new Vector3(workspace.transform.GetChild(0).transform.localScale.x, workspace.transform.GetChild(0).transform.localScale.y, tableMax);
        workspace.transform.GetChild(0).transform.Translate(0, 0, difference);
        table.transform.localScale = new Vector3(table.transform.localScale.x, table.transform.localScale.y, tableMax * 100f);
        table.transform.Translate(0, 0, -difference);
    }

    /// <summary>
    /// Adjusts workspace position according to the initial hand position
    /// </summary>
    private void AdjustWorkspace()
    {
        float z = ((leftHandInitialPos.z + rightHandInitialPos.z) / 2f) + 0.15f - Constants.CORNER_OFFSET;
        float x = (leftHandInitialPos.x + rightHandInitialPos.x) / 2f;
        float y = (leftHandInitialPos.y + rightHandInitialPos.y) / 2f - Constants.CORNER_OFFSET / 2f;

        workspace.transform.position = new Vector3(x, y, z);
    }

    /// <summary>
    /// Handles Start button pressed
    /// </summary>
    public void StartPressed()
    {
        if (!leftHandInitialPos.IsNan() && !rightHandInitialPos.IsNan() && leftPositionSphere != null && rightPositionSphere != null)
            gameCoroutine = StartCoroutine(Game());
    }

    /// <summary>
    /// Handles Calibration button pressed
    /// </summary>
    public void CalibrationPressed() => StartCoroutine(InitialPositionCalibration());

    /// <summary>
    /// Handles Vertical Game Toggle button pressed
    /// </summary>
    public void VerticalGameTogglePressed()
    {
        if (leftPositionSphere != null && rightPositionSphere != null)
        {
            verticalGameHeight = exerciseInfo.verticalGameHeight;

            // if vertical game is turned on, it sets horizontal game
            if (verticalGame)
            {
                verticalGame = !verticalGame;

                table.SetActive(true);
                tableTop.SetActive(true);

                if (cylinderInstance != null)
                {
                    Object.Destroy(cylinderInstance);
                    isCylinderInScene = false;
                }

                // Raises the position of initial position spheres
                leftHandInteractiveSphere.transform.Translate(new Vector3(0f, -1 * verticalGameHeight, 0f));
                rightHandInteractiveSphere.transform.Translate(new Vector3(0f, -1 * verticalGameHeight, 0f));
            }
            // if vertical game is not turned on, it sets it
            else
            {
                verticalGame = !verticalGame;

                table.SetActive(false);
                tableTop.SetActive(false);

                if (cylinderInstance != null)
                {
                    Object.Destroy(cylinderInstance);
                    isCylinderInScene = false;
                }

                // Puts the position of initial position spheres on the table
                leftHandInteractiveSphere.transform.Translate(new Vector3(0f, verticalGameHeight, 0f));
                rightHandInteractiveSphere.transform.Translate(new Vector3(0f, verticalGameHeight, 0f));
            }
        }
    }

    /// <summary>
    /// Handles Alarm button pressed
    /// </summary>
    public void AlarmPressed()
    {
        if (alarmInstance == null)
        {
            if (isCylinderInScene)
            {
                cylinderInstance.SetActive(false);
                isCylinderInScene = false;
            }

            Vector3 alarmPosition = new Vector3(
                (leftHandInteractiveSphere.transform.position.x + rightHandInteractiveSphere.transform.position.x) / 2f,
                leftHandInteractiveSphere.transform.position.y + 0.04f,
                leftHandInteractiveSphere.transform.position.z + 0.15f);

            alarmInstance = GameObject.Instantiate(alarmPrefab, alarmPosition, Quaternion.identity);
            alarmInstance.transform.Rotate(180, 0, 0);
        }
    }

    /// <summary>
    /// Handles alarm destroyed action
    /// </summary>
    public void OnAlarmDestroyed() => alarmInstance = null;

    /// <summary>
    /// Handles Quit Game button pressed
    /// </summary>
    public void QuitGamePresssed()
    {
        // If game started, it stops calculation of distance and loads Log scene
        if(gameCoroutine != null && startedComputingDistance)
        {
            StopCoroutine(gameCoroutine);
            gameCoroutine = null;
            errorCorrection.StopDataCollection();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        // If game have not started, it returns to Initial scene
        else
            SceneManager.LoadScene(0);
    }
}
