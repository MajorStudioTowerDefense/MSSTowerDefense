using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
// Setting up the Audio Sources
    public static AudioManager instance;

    public AudioSource[] audioSources;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public float duration = 21f;
    public AudioClip Ticking;
    public AudioClip StartShift;
    
    //Assigning certain sounds to play for a limited amount of time
    private void Start()
    {
        audioSource.clip = Ticking;
        audioSource2.clip = StartShift;


        // Calls up Stopping Coroutine
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
    //Stops the ticking from playing after 20 seconds
 IEnumerator StopSoundAfterDelay()
    {
        // Wait for 'duration' seconds
        yield return new WaitForSeconds(duration);

        // Stop the sound
        audioSource.Stop();

        //start the whistle
        StartCoroutine(PlaySoundAfterDelay(20f));

    }
    //Play the whistle after 20 seconds
    IEnumerator PlaySoundAfterDelay(float delay)
    {   
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(Ticking, transform.position);

    }

}
