using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] audioSources;
    public AudioSource audioSource;
    public AudioSource audioSource2;


    public float duration = 60f;
    public AudioClip Ticking;
    public AudioClip TempAmb; 
    private void Start()
    {
        audioSource.clip = Ticking;
        audioSource2.clip = TempAmb;


        StartCoroutine(StopSoundAfterDelay());

    }
    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Initialize audio sources
        audioSources = GetComponents<AudioSource>();
    }

    // Play a single audio clip
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        foreach (var source in audioSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.volume = volume;
                source.Play();
                return;
            }
        }
    }

    // Stop all audio sources
    public void StopAllSounds()
    {
        foreach (var source in audioSources)
        {
            source.Stop();
        }
    }

    // Pause all audio sources
    public void PauseAllSounds()
    {
        foreach (var source in audioSources)
        {
            source.Pause();
        }
    }

    // Adjust the volume of all audio sources
    public void SetVolume(float volume)
    {
        foreach (var source in audioSources)
        {
            source.volume = volume;
        }
    }
 IEnumerator StopSoundAfterDelay()
    {
        // Wait for 'duration' seconds
        yield return new WaitForSeconds(60);

        // Stop the sound
        audioSource.Stop();
        StartCoroutine(StartSoundAfterDelay());

    }

 IEnumerator StartSoundAfterDelay()
    {
        // Wait for 'duration' seconds
        yield return new WaitForSeconds(60);

        // Stop the sound
        audioSource2.Play();
    }
}