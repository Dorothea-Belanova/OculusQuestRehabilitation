using UnityEngine;
using OculusSampleFramework;

public class InteractiveCylinder: MonoBehaviour {

    [SerializeField] GameObject cylinderClickSound;
    [SerializeField] ExerciseInfo exerciseInfo;
    ExerciseSceneControl sceneControl;
    public bool isLeft = false;

    void Awake() {
        sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<ExerciseSceneControl>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroyCylinder();
        }
    }

    public void OnTriggerEnter(Collider other) {
        //string parent = other.gameObject.transform.parent.name;
        //string parentparent = other.gameObject.transform.parent.parent.name;
        OVRSkeleton.SkeletonType skeletonType = other.gameObject.transform.parent.parent.parent.gameObject.GetComponent<OVRSkeleton>().GetSkeletonType();

        Debug.Log("triggered");
        if (exerciseInfo.selectedHand == SelectedHand.BothHands)
        {
            Debug.Log("both hands");
            if ((isLeft && skeletonType == OVRSkeleton.SkeletonType.HandLeft && sceneControl.rightHandInteractiveSphere.IntersectsHand()) ||
                (!isLeft && skeletonType == OVRSkeleton.SkeletonType.HandRight && sceneControl.leftHandInteractiveSphere.IntersectsHand()))
            {
                DestroyCylinder();
            }


            //if (((isLeft && parent.Contains("HandLeft"))) || (!sceneControl.isLeftActive && parent.Contains("HandRight"))) &&
            //    ((sceneControl.isLeftActive && sceneControl.rightHandInteractiveSphere.IntersectsHand()) || (!sceneControl.isLeftActive && sceneControl.leftHandInteractiveSphere.IntersectsHand())))
           // {
            //    DestroyCylinder();
            //}
        }
        else
        {
            bool isLeftHandSelected = exerciseInfo.selectedHand == SelectedHand.LeftHand ? true : false;
            if ((isLeftHandSelected && skeletonType == OVRSkeleton.SkeletonType.HandLeft) || (!isLeftHandSelected && skeletonType == OVRSkeleton.SkeletonType.HandRight))
            {
                DestroyCylinder();
            }
        }



        //Debug.Log("TRIGGERED BY: " + other.gameObject.name);
        //Debug.Log("TRIGGERED BY: " + other.gameObject.tag);

        //Debug.Log("PARENT: " + other.gameObject.transform.parent.name);
        //Debug.Log("ROOT: " + other.gameObject.transform.root.gameObject.name);
        
        //gameObject.SetActive(false);
    }

    private void DestroyCylinder() {
        GameObject.Instantiate(cylinderClickSound, transform.position, Quaternion.identity);

        sceneControl.CylinderDestroyed(gameObject);
    }
}