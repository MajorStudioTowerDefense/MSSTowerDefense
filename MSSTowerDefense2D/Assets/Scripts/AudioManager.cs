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
    private ClockDisplayUI timerScript;

    //Assigning certain sounds to play for a limited amount of time
    private void Start()
    {
        timerScript = GameObject.Find("ClockNumbers").GetComponent<ClockDisplayUI>(); // Grabs the ClockUI script from the "ClockNumbers" GameObect
        audioSource.clip = Ticking;
        audioSource2.clip = StartShift;
        // Start playing music based on the initial time
        UpdateMusic();
       
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
   

    void Update()
    {   //tie real time to ambiance
        
        if (timerScript.hours == 7 && timerScript.minutes == 0){
            audioSource.Play();
        }
        if (timerScript.hours == 9 && timerScript.minutes == 0)
        {
            audioSource2.Play();

        } 
        
        

}
}