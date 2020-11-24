using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    private AudioSource audioSource;
    private bool ringing = false;

    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();

        StartCoroutine(Ring());
    }

    private IEnumerator Ring()
    {
        float wait = Random.Range(1f, 5f);
        yield return new WaitForSeconds(wait);

        audioSource.Play();
        ringing = true;
    }

    public void AlarmButtonPressed()
    {
        Debug.Log("stlacila som");
        if (ringing)
        {
            audioSource.Stop();
            ringing = false;
            StartCoroutine(Destroy());
        }
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
