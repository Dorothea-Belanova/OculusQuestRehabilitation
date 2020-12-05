using System.Collections;
using UnityEngine;

public class CylinderClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;

    void Awake()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickClip;
        audioSource.volume = Random.Range(0.8f, 1f);
        audioSource.pitch = Random.Range(0.8f, 1.1f);
        audioSource.Play();

        StartCoroutine(DestroyAfterSoundOff());
    }

    /// <summary>
    /// Destroys itself after the length of click sound passed
    /// </summary>
    IEnumerator DestroyAfterSoundOff()
    {
        yield return new WaitForSeconds(clickClip.length);
        Destroy(gameObject);
    }
}
