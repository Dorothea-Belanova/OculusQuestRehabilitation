using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HandData : MonoBehaviour
{
    [SerializeField] GameObject sphere;

    // Update is called once per frame
    /*void Update()
    {
        if(!Hands.Instance.IsInitialized()) {
            return;
        }

        bool leftHandIsReliable = Hands.Instance.LeftHand.HandConfidence == Hand.HandTrackingConfidence.High;
        bool rightHandIsReliable = Hands.Instance.RightHand.HandConfidence == Hand.HandTrackingConfidence.High;

        IList<Transform> leftHandBones = Hands.Instance.LeftHand.Skeleton.Bones;
        sphere.transform.position = (leftHandBones[3].transform.position + leftHandBones[17].transform.position) / 2;

        //OVRPlugin.BoneId.Hand_ForearmStub
    }*/
}
