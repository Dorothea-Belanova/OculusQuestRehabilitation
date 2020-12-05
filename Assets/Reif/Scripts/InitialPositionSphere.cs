using UnityEngine;
using OculusSampleFramework;
using System.Collections.Generic;

public class InitialPositionSphere : MonoBehaviour {
    private bool isColiding = false;
    public bool isLeft;

    /// <summary>
    /// Handles trigger enter and evalues whether sphere collides with hand
    /// </summary>
    private void OnTriggerEnter(Collider other) {
        isColiding = other.gameObject.transform.parent.name.Contains("Hand") ? !isColiding : isColiding;
    }

    /// <summary>
    /// Handles trigger exits and evalues whether sphere stops colliding with hand
    /// </summary>
    private void OnTriggerExit(Collider other) {
        isColiding = other.gameObject.transform.parent.name.Contains("Hand") ? !isColiding : isColiding;
    }

    /// <summary>
    /// Returns true if sphere collides with corresponding hand
    /// </summary>
    public bool IntersectsHand() {
        if(isLeft && HandsManager.Instance.LeftHand.HandConfidence == OVRHand.TrackingConfidence.High) {
            IList<OVRBone> bones = HandsManager.Instance.LeftHandSkeleton.Bones;
            Vector3 palmPosition = (bones[1].Transform.position + bones[10].Transform.position) / 2f;

            return GetComponent<SphereCollider>().bounds.Contains(palmPosition);
        }
        else if(!isLeft && HandsManager.Instance.RightHand.HandConfidence == OVRHand.TrackingConfidence.High) {
            IList<OVRBone> bones = HandsManager.Instance.RightHandSkeleton.Bones;
            Vector3 palmPosition = (bones[1].Transform.position + bones[10].Transform.position) / 2f;

            return GetComponent<SphereCollider>().bounds.Contains(palmPosition);
        }

        return false;
    }
}