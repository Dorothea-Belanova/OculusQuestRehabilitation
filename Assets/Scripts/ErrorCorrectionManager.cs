using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using System;

namespace ErrorCorrection
{
    public class ErrorCorrectionManager
    {
        private Vector3 previousHandPosition;
        private Vector3 previousClusterCenter;
        private Vector3 previousCameraPosition;
        private Vector3 initialHandPosition;

        List<string[]> data;
        List<Vector3> positions;
        Vector2 dimensions;

        List<int> xImage = new List<int>();
        List<int> yImage = new List<int>();
        Texture2D texture;

        LowPassFilter lowPassFilter;
        MedianFilter medianFilter;
        RadiusClustering finalRadiusClustering;

        float correctedDistance = 0f;
        float notCorrectedDistance = 0f;
        Coroutine dataCollectionCoroutine;

        ExerciseSceneControl sceneControl;
        ExerciseInfo exerciseInfo;

        Vector3 leftInitial;
        Vector3 rightInitial;

        /// <param name="leftHandInitialPositions">List of left hand positions during calibration.</param>
        /// <param name="righttHandInitialPositions">List of right hand positions during calibration.</param>
        public ErrorCorrectionManager(List<Vector3> leftHandInitialPositions, List<Vector3> rightHandInitialPositions)
        {
            sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<ExerciseSceneControl>();

            // Initialization of Filters
            lowPassFilter = new LowPassFilter(LowPassApproach.CombinedPass);
            medianFilter = new MedianFilter(3);
            finalRadiusClustering = new RadiusClustering(leftHandInitialPositions, rightHandInitialPositions);

            positions = new List<Vector3>();
        }

        // Relative position: 
        //float x = Mathf.Abs(logDataPreviousHandPositon.x - sceneControl.workspace.transform.position.x + 0.2f);
        //float y = Mathf.Abs(logDataPreviousHandPositon.z - sceneControl.workspace.transform.position.z + 0.15f);

        // dat ukladanie obrázka do exercise scene manager
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

                StartDataCollection(relativePosition, hand, exerciseInfo);
            }
        }

        public void UpdateMeasurement(Vector3 cameraPosition, Vector3 handPosition, string hand, ExerciseInfo exerciseInfo)
        {
            // Data Collection Initialization
            if (data == null)
            {
                // Camera
                previousCameraPosition = cameraPosition;

                initialHandPosition = handPosition;
                previousClusterCenter = handPosition;
                previousHandPosition = handPosition;
                positions.Add(previousHandPosition);

                StartDataCollection(handPosition, hand, exerciseInfo);
            }
        }

        #region Data Collection Initialization

        private void StartDataCollection(Vector3 position, string hand, ExerciseInfo exerciseInfo)
        {
            this.exerciseInfo = exerciseInfo;

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

            Debug.Log("VYPOCITANE: " + Mathf.FloorToInt((exerciseInfo.maxHandDistance + 0.1f) * 1000));
            dimensions = new Vector2(400, Mathf.FloorToInt((exerciseInfo.maxHandDistance + 0.1f) * 1000));

            float x = Mathf.Abs(position.x - sceneControl.tableTop.transform.position.x + dimensions.x / 2000f);
            float y = Mathf.Abs(position.z - sceneControl.tableTop.transform.position.z + dimensions.y / 2000f);
            //float x = Mathf.Abs(position.x);
            //float y = Mathf.Abs(position.z);
            //float x = Mathf.Abs(position.x - dimensions.x / 2);
            //float y = Mathf.Abs(position.z - dimensions.y / 2);
            Debug.Log("position x:" + position.x);
            Debug.Log("position y:" + position.z);
            Debug.Log("scene position x:" + sceneControl.tableTop.transform.position.x);
            Debug.Log("scene position y:" + sceneControl.tableTop.transform.position.z);
            Debug.Log("dimension x:" + dimensions.x / 200f);
            Debug.Log("dimension y:" + dimensions.y / 200f);
            xImage.Add((int)(x * 1000));
            yImage.Add((int)(y * 1000f));

            // Preparing for image creation
            texture = new Texture2D((int)dimensions.x, (int)dimensions.y);
            //texture.GetComponent<Renderer>().material.mainTexture = texture;

            for (int i = 0; i < texture.height; i++)
            {
                for (int j = 0; j < texture.width; j++)
                {
                    Color color = Color.white;
                    texture.SetPixel(j, i, color);
                }
            }

            Debug.Log("TUNAK");
        }

        #endregion

        public void StopDataCollection()
        {
            // adding cluster center
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

            Debug.Log("ukladam");
            //CSVManager manager = new CSVManager();
            //manager.Save(data);

            // LOG DATA
            Vector3 currentHandPosition = center;
            currentHandPosition.y = 0;
            correctedDistance += Vector3.Distance(previousHandPosition, currentHandPosition);
            Debug.Log("CORRECTED DISTANCE: " + correctedDistance);

            // FINISH
            //float x = Mathf.Abs(currentHandPosition.x - sceneControl.workspace.transform.position.x + dimensions.x / 2);
            //float y = Mathf.Abs(currentHandPosition.z - sceneControl.workspace.transform.position.z + dimensions.y / 2);
            //float x = Mathf.Abs(currentHandPosition.x + dimensions.x / 2);
            //float y = Mathf.Abs(currentHandPosition.z + dimensions.y / 2);
            //float x = Mathf.Abs(currentHandPosition.x - dimensions.x / 2);
            //float y = Mathf.Abs(currentHandPosition.z + dimensions.y / 2);

            float x = Mathf.Abs(center.x - sceneControl.tableTop.transform.position.x + dimensions.x / 2000f);
            float y = Mathf.Abs(center.z - sceneControl.tableTop.transform.position.z + dimensions.y / 2000f);
            xImage.Add((int)(x * 1000));
            yImage.Add((int)(y * 1000));

            // Finishing time
            data[1][7] = Dater.GetTime();
            exerciseInfo.endingTime = Dater.GetTime();

            // SAVING DATA
            Debug.Log("KONCIM LOG DATA");
            //manager.Save(data);

            // LOG IMAGE
            texture.SetPixel(xImage[xImage.Count - 1], yImage[yImage.Count - 1], Color.black);
            texture.Apply();
            exerciseInfo.texture = texture;
            exerciseInfo.correctedDistance = correctedDistance;

            // Encode texture into PNG
            //byte[] bytes = texture.EncodeToPNG();
            //Object.Destroy(texture);

            // saving
            // FINISH
            //System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/Patients Data/" + sceneControl.applicationControl.rehabilitationInfo.patientID + "_" + Dater.GetDate() + ".png", bytes);
            //System.IO.File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", bytes);
            //System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/SavedScreen.png", bytes);
        }

        public float NewValue(Vector3 cameraPosition, Vector3 handPosition)
        {
            float distance = Vector3.Distance(cameraPosition, handPosition);

            // HAND ROTATION
            Vector3 currentHandPos = handPosition;
            positions.Add(currentHandPos);

            // LMC DISTANCE
            //Debug.Log("NOT CORRECTED");
            //Debug.Log("Previous hand position: " + previousHandPosition);
            //Debug.Log("current hand position: " + previousHandPosition);
            notCorrectedDistance += Vector3.Distance(previousHandPosition, currentHandPos);

            // FIXING THE VALUE - 1. filtering, 2. functioning, 3. radius clustering
            Vector3 lowPassFiltrated = lowPassFilter.Filtrate(positions);
            Vector3 localHandPos = lowPassFiltrated - initialHandPosition;
            Vector3 diff = lowPassFiltrated - previousHandPosition;

            

            Vector3 final = finalRadiusClustering.UpdateMeasurement(lowPassFiltrated, distance);
            if (final != new Vector3())
            {
                /*Debug.Log("CORRECTED");
                Debug.Log("PREVIOUS HAND POSITION: " + previousHandPosition);
                Debug.Log("PREVIOUS CLUSTER CENTRE: " + previousClusterCenter);
                Debug.Log("RADIUS CLUSTERED: " + final);*/
                correctedDistance += Vector3.Distance(previousClusterCenter, final);
                previousClusterCenter = final;
            }

            string[] dataTemp2 = new string[7];
            dataTemp2[0] = data.Count.ToString();
            dataTemp2[1] = currentHandPos.x.ToString();
            dataTemp2[2] = currentHandPos.y.ToString();
            dataTemp2[3] = currentHandPos.z.ToString();
            dataTemp2[4] = "";
            dataTemp2[5] = notCorrectedDistance.ToString();
            dataTemp2[6] = correctedDistance.ToString();
            data.Add(dataTemp2);

            float x = Mathf.Abs(handPosition.x - sceneControl.tableTop.transform.position.x + dimensions.x / 2000f);
            float y = Mathf.Abs(handPosition.z - sceneControl.tableTop.transform.position.z + dimensions.y / 2000f);
            //float x = Mathf.Abs(handPosition.x - dimensions.x / 2);
            //float y = Mathf.Abs(handPosition.z - dimensions.y / 2);
            //float x = Mathf.Abs(handPosition.x + dimensions.x / 2);
            //float y = Mathf.Abs(handPosition.z + dimensions.y / 2);
            Debug.Log("position x:" + handPosition.x);
            Debug.Log("position y:" + handPosition.z);
            Debug.Log("scene position x:" + sceneControl.tableTop.transform.position.x);
            Debug.Log("scene position y:" + sceneControl.tableTop.transform.position.z);
            Debug.Log("dimension x:" + dimensions.x / 200f);
            Debug.Log("dimension y:" + dimensions.y / 200f);
            xImage.Add((int)(x * 1000));
            yImage.Add((int)(y * 1000));
            texture.SetPixel(xImage[xImage.Count - 1], yImage[yImage.Count - 1], Color.black);

            //Debug.Log("not corrected distance: " + notCorrectedDistance);
            //Debug.Log("corrected distance: " + correctedDistance);

            // camera
            previousCameraPosition = cameraPosition;

            // left hand
            previousHandPosition = handPosition;

            return correctedDistance;
        }

        IEnumerator CollectData()
        {
            do
            {
                yield return new WaitForEndOfFrame();

                Debug.Log("v data collection");
                IList<OVRBone> bones;
                OVRHand hand;
                if (sceneControl.isLeftActive)
                {
                    bones = HandsManager.Instance.LeftHandSkeleton.Bones;
                    hand = HandsManager.Instance.LeftHand;
                }
                else
                {
                    bones = HandsManager.Instance.RightHandSkeleton.Bones;
                    hand = HandsManager.Instance.RightHand;
                }
                if (!hand.IsTracked)
                {
                    yield return null;
                }

                Debug.Log("tady");

                Vector3 position = (bones[1].Transform.position + bones[10].Transform.position) / 2f;
                positions.Add(position);

                float cameraDistance = Vector3.Distance(Camera.main.transform.position, position);

                notCorrectedDistance += Vector3.Distance(previousHandPosition, position);

                // FIXING THE VALUE - 1. filtering, 2. radius clustering
                Vector3 lowPassFiltered = lowPassFilter.Filtrate(positions);

                Vector3 final = finalRadiusClustering.UpdateMeasurement(lowPassFiltered, cameraDistance);
                if (final != new Vector3())
                {
                    correctedDistance += Vector3.Distance(previousClusterCenter, final);
                    previousClusterCenter = final;

                    // LOG DATA
                    // FINISH
                    /*float x = Mathf.Abs(final.x - sceneControl.workspace.transform.position.x + 0.2f);
                    float y = Mathf.Abs(final.z - sceneControl.workspace.transform.position.z + 0.15f);
                    xImage.Add((int)(x * 1000));
                    yImage.Add((int)(y * 1000));*/

                    // IMAGE
                    // FINISH
                    //texture.SetPixel(xImage[xImage.Count - 1], yImage[yImage.Count - 1], Color.black);
                }

                string[] dataTemp2 = new string[6];
                dataTemp2[0] = data.Count.ToString();
                // FINISH
                /*dataTemp2[1] = (final.x - sceneControl.workspace.transform.position.x - 0.2f).ToString();
                dataTemp2[2] = (final.y - sceneControl.workspace.transform.position.y - 0.15f).ToString();
                dataTemp2[3] = (final.z - sceneControl.workspace.transform.position.z - 0.15f).ToString();*/
                dataTemp2[1] = (final.x).ToString();
                dataTemp2[2] = (final.y).ToString();
                dataTemp2[3] = (final.z).ToString();
                dataTemp2[4] = sceneControl.isLeftActive ? "L" : "R";
                dataTemp2[5] = notCorrectedDistance.ToString();
                dataTemp2[6] = correctedDistance.ToString();
                data.Add(dataTemp2);

                // FINISH
                /*sceneControl.leftHandDistanceText.text = "LMC Dist: " + ((Mathf.Round(LMCdist * 100)) / 100.0).ToString() + " m";
                sceneControl.SetDistance(((Mathf.Round(fixedDist * 100)) / 100.0).ToString());*/
                //sceneControl.rightHandDistanceText.text = "Distance: " + ((Mathf.Round(fixedDist * 100)) / 100.0).ToString() + " m";
                Debug.Log("not corrected distance: " + notCorrectedDistance);
                Debug.Log("corrected distance: " + correctedDistance);

                // camera
                previousCameraPosition = Camera.main.transform.position;

                // left hand
                previousHandPosition = position;

                //Debug.Log("beriem data z frame");
            } while (true);
        }
    }
}
