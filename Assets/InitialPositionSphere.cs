using UnityEngine;
using OculusSampleFramework;
using System.Collections.Generic;

public class InitialPositionSphere: MonoBehaviour {
    private bool isColiding = false;
    public bool isLeft;
    private ExerciseSceneControl sceneControl;

    private void Awake() {
        sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<ExerciseSceneControl>();
    }

    private void OnTriggerEnter(Collider other) {
        isColiding = other.gameObject.transform.parent.name.Contains("Hand") ? !isColiding : isColiding;

        if(isColiding) {
            /*Debug.Log("hand entered " + other.name);
            Debug.Log("PARENT: " + other.gameObject.transform.parent.name);*/
        }
    }

    private void OnTriggerExit(Collider other) {
        isColiding = other.gameObject.transform.parent.name.Contains("Hand") ? !isColiding : isColiding;

        if(isColiding) {
            /*Debug.Log("hand exited");
            Debug.Log("PARENT: " + other.gameObject.transform.parent.name);*/
        }
    }

    public bool IsColiding() {
        return isColiding;
    }

    public bool IntersectsHand() {
        if(isLeft && HandsManager.Instance.LeftHand.HandConfidence == OVRHand.TrackingConfidence.High) {
            IList<OVRBone> bones = HandsManager.Instance.LeftHandSkeleton.Bones;
            Vector3 palmPosition = (bones[1].Transform.position + bones[10].Transform.position) / 2f;

            bool interacts = this.GetComponent<SphereCollider>().bounds.Contains(palmPosition);
            /*if(interacts) {
                Debug.Log("LAVAAAA");
            }*/
            return this.GetComponent<SphereCollider>().bounds.Contains(palmPosition);
        }
        else if(!isLeft && HandsManager.Instance.RightHand.HandConfidence == OVRHand.TrackingConfidence.High) {
            IList<OVRBone> bones = HandsManager.Instance.RightHandSkeleton.Bones;
            Vector3 palmPosition = (bones[1].Transform.position + bones[10].Transform.position) / 2f;

            bool interacts = this.GetComponent<SphereCollider>().bounds.Contains(palmPosition);
            /*if(interacts) {
                Debug.Log("PRAVAAAAA");
            }*/
            return this.GetComponent<SphereCollider>().bounds.Contains(palmPosition);
        }
        return false;
    }
}