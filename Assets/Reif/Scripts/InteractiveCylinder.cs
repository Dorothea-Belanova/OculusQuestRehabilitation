using UnityEngine;

public class InteractiveCylinder: MonoBehaviour {

    [SerializeField] private GameObject cylinderClickSound;
    [SerializeField] private ExerciseInfo exerciseInfo;
    private ExerciseSceneControl sceneControl;
    public bool isLeft = false;

    void Awake() {
        sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<ExerciseSceneControl>();
    }

    /// <summary>
    /// Evaluates whether on trigger enter was caused by corresponding hand
    /// </summary>
    public void OnTriggerEnter(Collider other) {
        OVRSkeleton.SkeletonType skeletonType = other.gameObject.transform.parent.parent.parent.gameObject.GetComponent<OVRSkeleton>().GetSkeletonType();

        if (exerciseInfo.selectedHand == SelectedHand.BothHands)
        {
            if ((isLeft && skeletonType == OVRSkeleton.SkeletonType.HandLeft && sceneControl.rightHandInteractiveSphere.IntersectsHand()) ||
                (!isLeft && skeletonType == OVRSkeleton.SkeletonType.HandRight && sceneControl.leftHandInteractiveSphere.IntersectsHand()))
                DestroyCylinder();
        }
        else
        {
            bool isLeftHandSelected = exerciseInfo.selectedHand == SelectedHand.LeftHand ? true : false;
            if ((isLeftHandSelected && skeletonType == OVRSkeleton.SkeletonType.HandLeft) || (!isLeftHandSelected && skeletonType == OVRSkeleton.SkeletonType.HandRight))
                DestroyCylinder();
        }
    }

    /// <summary>
    /// Handles destroying cylinder action
    /// </summary>
    private void DestroyCylinder() {
        GameObject.Instantiate(cylinderClickSound, transform.position, Quaternion.identity);
        sceneControl.CylinderDestroyed(gameObject);
    }
}