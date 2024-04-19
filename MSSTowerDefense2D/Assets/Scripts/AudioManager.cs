using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
// Setting up the Audio Sources
    public static AudioManager instance;
    public List<AudioSource> audioSources;
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioClip Ticking;
    public AudioClip StartShift;
    public bool allowCheck;

    //Assigning certain sounds to play for a limited amount of time
    private void Start()
    {
        audioSources = new() {audioSource1, audioSource2};
        //audioSource1.clip = Ticking;
        //audioSource2.clip = StartShift;
        allowCheck = true;
        // Start playing music based on the initial time
        
       
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
    {

        if (GameManager.instance.currentState == GameStates.TUTORIAL)
        {
            StopAllSounds();
            allowCheck = true;
        }
        else if (GameManager.instance.currentState == GameStates.PREP & allowCheck)
        {
            audioSource1.Play();
            allowCheck = false;
            Debug.Log("audio1");
        }
        else if (GameManager.instance.currentState == GameStates.STORE && !allowCheck)
        {
            audioSource2.Play();
            allowCheck = true;
            Debug.Log("audio2");
        }
    }
}