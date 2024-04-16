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
    public AudioClip Ticking;
    public AudioClip StartShift;
    private ClockDisplay ClockDisplayUI;

    //Assigning certain sounds to play for a limited amount of time
    private void Start()
    {
        ClockDisplayUI = FindObjectOfType<ClockNumbers>(); // Find the TimerScript component in the scene
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
    //Stops the ticking from playing after 60 seconds
 IEnumerator StopSoundAfterDelay()
    {
        // Wait for 'duration' seconds
        yield return null;
        //yield return new WaitForSeconds(ClockDisplayUI.currentTime = 18);

        // Stop the sound
        audioSource.Stop();

        //start the whistle
        StartCoroutine(StartSoundAfterDelay());


    }

    IEnumerator StartSoundAfterDelay()
    {   
        // Wait for 'duration' seconds
        yield return new WaitForSeconds(2f);
        
        //start sound
        //audioSource2.Start();

    }

    void Update()
    {
         if (timerScript != null)
        {
            float timerValue = timerScript.GetTimer(); // Access the timer value from TimerScript
            
        }
    }

}
