using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderClickSound : MonoBehaviour
{
    [SerializeField] AudioClip clickClip;

    void Awake()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickClip;
        audioSource.volume = Random.Range(0.8f, 1f);
        audioSource.pitch = Random.Range(0.8f, 1.1f);
        audioSource.Play();

        StartCoroutine(DestroyAfterSoundOff());
    }

    IEnumerator DestroyAfterSoundOff()
    {
        float time = 0;

        do
        {
            yield return null;
            time += Time.deltaTime;
        } while (time < clickClip.length);

        Destroy(gameObject);
    }
}
