using System.Collections.Generic;
using UnityEngine;

namespace ErrorCorrection
{
    public class ErrorCorrectionManager
    {
        private Vector3 previousHandPosition;
        private Vector3 previousClusterCenter;
        private Vector3 previousCameraPosition;
        private Vector3 initialHandPosition;

        private List<string[]> data;
        private List<Vector3> positions;
        private Vector2 dimensions;

        private List<int> xImage = new List<int>();
        private List<int> yImage = new List<int>();
        private Texture2D texture;

        private LowPassFilter lowPassFilter;
        private MedianFilter medianFilter;
        private RadiusClustering finalRadiusClustering;

        private float correctedDistance = 0f;
        private float notCorrectedDistance = 0f;
        private string previousActiveHand;
        private bool handChanged = false;

        private ExerciseSceneControl sceneControl;
        private ExerciseInfo exerciseInfo;

        private Vector3 leftInitial;
        private Vector3 rightInitial;

        /// <param name="leftHandInitialPositions">List of left hand positions during calibration</param>
        /// <param name="righttHandInitialPositions">List of right hand positions during calibration</param>
        public ErrorCorrectionManager(List<Vector3> leftHandInitialPositions, List<Vector3> rightHandInitialPositions)
        {
            sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<ExerciseSceneControl>();

            // Initialization of Filters
            lowPassFilter = new LowPassFilter(LowPassApproach.CombinedPass);
            medianFilter = new MedianFilter(3);
            finalRadiusClustering = new RadiusClustering(leftHandInitialPositions, rightHandInitialPositions);

            positions = new List<Vector3>();
        }

        /// <summary>
        /// Starts initialization of parameters
        /// </summary>
        public void UpdateMeasurement(Vector3 relativePosition, string hand, ExerciseInfo exerciseInfo)
        {
            // Data Collection Initialization
            if (data == null)
            {
                // Camera
                previousCameraPosition = Camera.main.transform.position;

                initialHandPosition = relativePosition;
                previousClusterCenter = relativePosition;
                previousHandPosition = relativePosition;
                positions.Add(previousHandPosition);
                previousActiveHand = hand;

                StartDataCollection(relativePosition, hand, exerciseInfo);
            }
        }

        /// <summary>
        /// Handles starting data collection
        /// </summary>
        private void StartDataCollection(Vector3 position, string hand, ExerciseInfo exerciseInfo)
        {
            this.exerciseInfo = exerciseInfo;
            exerciseInfo.startingTime = Dater.GetTime();

            data = new List<string[]>();
            string[] dataTemp = new string[9];
            dataTemp[0] = "Frame";
            dataTemp[1] = "x";
            dataTemp[2] = "y";
            dataTemp[3] = "z";
            dataTemp[4] = "Hand";
            dataTemp[5] = "Not corrected";
            dataTemp[6] = "Total Distance";
            dataTemp[7] = "Start Time";
            dataTemp[8] = "End Time";
            data.Add(dataTemp);

            // Saving first input
            dataTemp = new string[9];
            dataTemp[0] = data.Count.ToString();
            dataTemp[1] = position.x.ToString();
            dataTemp[2] = position.y.ToString();
            dataTemp[3] = position.z.ToString();
            dataTemp[4] = hand;
            dataTemp[5] = "0";
            dataTemp[6] = "0";
            dataTemp[7] = Dater.GetTime();
            dataTemp[8] = "";
            data.Add(dataTemp);

            dimensions = new Vector2(400, Mathf.FloorToInt((exerciseInfo.maxHandDistance + 0.1f) * 1000));

            float x = Mathf.Abs(position.x - sceneControl.tableTop.transform.position.x + dimensions.x / 2000f);
            float y = Mathf.Abs(position.z - sceneControl.tableTop.transform.position.z + dimensions.y / 2000f);

            xImage.Add((int)(x * 1000f));
            yImage.Add((int)(y * 1000f));

            texture = new Texture2D((int)dimensions.x, (int)dimensions.y);

            for (int i = 0; i < texture.height; i++)
            {
                for (int j = 0; j < texture.width; j++)
                {
                    Color color = Color.white;
                    texture.SetPixel(j, i, color);
                }
            }
        }

        /// <summary>
        /// Handles stopping data collection
        /// </summary>
        public void StopDataCollection()
        {
            // Adds cluster center to all the frames where the position was clustered into current cluster center
            Vector3 center = finalRadiusClustering.GetClusteredCenter();
            for (int i = data.Count - 2; i > 0; --i)
            {
                if (data[i][1] == "")
                {
                    data[i][1] = center.x.ToString();
                    data[i][2] = center.y.ToString();
                    data[i][3] = center.z.ToString();
                }
            }

            Vector3 currentHandPosition = center;
            currentHandPosition.y = 0;
            correctedDistance += Vector3.Distance(previousHandPosition, currentHandPosition);

            float x = Mathf.Abs(center.x - sceneControl.tableTop.transform.position.x + dimensions.x / 2000f);
            float y = Mathf.Abs(center.z - sceneControl.tableTop.transform.position.z + dimensions.y / 2000f);
            xImage.Add((int)(x * 1000));
            yImage.Add((int)(y * 1000));

            // Finishing time
            data[1][8] = Dater.GetTime();
            exerciseInfo.endingTime = Dater.GetTime();

            // LOG IMAGE
            texture.SetPixel(xImage[xImage.Count - 1], yImage[yImage.Count - 1], Color.black);
            texture.Apply();
            exerciseInfo.texture = texture;
            exerciseInfo.correctedDistance = correctedDistance;

            string fileName = exerciseInfo.patientID + "_" + Dater.GetDate() + "_" + exerciseInfo.startingTime;
            CSVManager.Save(data, fileName);
        }

        /// <summary>
        /// Handles new hand position entering error correction
        /// </summary>
        public float NewValue(Vector3 cameraPosition, Vector3 handPosition, string activeHand)
        {
            float distance = Vector3.Distance(cameraPosition, handPosition);

            // HAND ROTATION
            Vector3 currentHandPos = handPosition;
            notCorrectedDistance += Vector3.Distance(previousHandPosition, currentHandPos);

            Vector3 finalPosition;

            // HAND IS THE SAME
            if(previousActiveHand.Equals(activeHand))
            {
                positions.Add(currentHandPos);

                // FIXING THE VALUE - 1. filtering, 2. radius clustering
                Vector3 lowPassFiltrated;
                if (handChanged)
                    lowPassFiltrated = lowPassFilter.Filtrate(positions, positions[positions.Count - 2]);
                else
                    lowPassFiltrated = lowPassFilter.Filtrate(positions);

                finalPosition = finalRadiusClustering.UpdateMeasurement(currentHandPos, distance);

                if (finalPosition != new Vector3())
                {
                    if (handChanged)
                    {
                        previousClusterCenter = finalPosition;
                        handChanged = false;
                    }
                    else
                    {
                        correctedDistance += Vector3.Distance(previousClusterCenter, finalPosition);
                        previousClusterCenter = finalPosition;
                    }
                }
            }
            // HAND CHANGED
            else
            {
                positions.Clear();
                positions.Add(currentHandPos);

                finalPosition = finalRadiusClustering.UpdateMeasurement(currentHandPos, distance);

                handChanged = true;

                if (finalPosition != new Vector3())
                {
                    correctedDistance += Vector3.Distance(previousClusterCenter, finalPosition);
                }
            }

            previousActiveHand = activeHand;

            string[] dataTemp = new string[7];
            dataTemp[0] = data.Count.ToString();
            dataTemp[1] = currentHandPos.x.ToString();
            dataTemp[2] = currentHandPos.y.ToString();
            dataTemp[3] = currentHandPos.z.ToString();
            dataTemp[4] = "";
            dataTemp[5] = notCorrectedDistance.ToString();
            dataTemp[6] = correctedDistance.ToString();
            data.Add(dataTemp);

            float x = Mathf.Abs(handPosition.x - sceneControl.tableTop.transform.position.x + dimensions.x / 2000f);
            float y = Mathf.Abs(handPosition.z - sceneControl.tableTop.transform.position.z + dimensions.y / 2000f);

            xImage.Add((int)(x * 1000));
            yImage.Add((int)(y * 1000));
            texture.SetPixel(xImage[xImage.Count - 1], yImage[yImage.Count - 1], Color.black);

            previousCameraPosition = cameraPosition;
            previousHandPosition = handPosition;

            return correctedDistance;
        }
    }
}
