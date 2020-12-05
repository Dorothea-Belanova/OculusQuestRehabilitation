using System.Collections;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    private ExerciseSceneControl sceneControl;
    private AudioSource audioSource;
    private bool isRinging = false;

    void Awake()
    {
        sceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<ExerciseSceneControl>();
        audioSource = this.GetComponent<AudioSource>();

        StartCoroutine(Ring());
    }

    /// <summary>
    /// Handles ringing at a random time
    /// </summary>
    private IEnumerator Ring()
    {
        float wait = Random.Range(1f, 5f);
        yield return new WaitForSeconds(wait);

        audioSource.Play();
        isRinging = true;
    }

    /// <summary>
    /// Handles alarm button pressed action
    /// </summary>
    public void AlarmButtonPressed()
    {
        if (isRinging)
        {
            audioSource.Stop();
            isRinging = false;
            StartCoroutine(Destroy());
        }
    }

    /// <summary>
    /// Handles destroying of an alarm
    /// </summary>
    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.5f);
        sceneControl.OnAlarmDestroyed();
        Destroy(gameObject);
    }
}
