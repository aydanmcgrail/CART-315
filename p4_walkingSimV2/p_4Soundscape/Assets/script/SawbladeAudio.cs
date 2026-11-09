using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SawbladeAudio : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();
    }

    void Start()
    {
        if (audioSource.clip == null)
        {
            Debug.LogError("SawbladeAudio requires an AudioClip on its AudioSource.", this);
            return;
        }

        audioSource.loop = true;
        audioSource.timeSamples = Random.Range(0, audioSource.clip.samples);
        audioSource.Play();
    }
}
