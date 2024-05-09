using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
// Setting up the Audio Sources
    public static AudioManager instance;
    public List<AudioSource> audioSources;
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public Slider slider;
    

    public bool allowCheck;
    private bool hasPlayedShiftSound = false;
    
public AudioClip spawnSound;


    // This function is called whenever an NPC is spawned
    public void PlaySpawnSound()
    {
        // Play the spawn sound only once
        if (!hasPlayedShiftSound)
        {
            // Check if the spawn sound is assigned
            if (spawnSound == null)
            {
                Debug.LogError("Spawn sound is not assigned to the AudioManager!");
                return;
            }
        if(hasPlayedShiftSound == true)
            // Play the spawn sound
            GetComponent<AudioSource>().PlayOneShot(spawnSound);
            hasPlayedShiftSound = false;
           
        }
    }
    //Assigning certain sounds to play for a limited amount of time
    private void Start()
    {   
        audioSource1.volume = 0.7f;
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
    public void PlaySound(AudioClip clip)
    {
        foreach (var source in audioSources)
        {
            if (!source.isPlaying)
            {
                slider = FindObjectOfType<Slider>();
                source.clip = clip;
                source.volume = slider.value;
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
    public void SetVolume()
    {
        slider = FindObjectOfType<Slider>();
        foreach (var source in audioSources)
        {
            Debug.Log(source.name);
            source.volume = slider.value;
        }
    }
   

    void Update()
    {

        if (GameManager.instance.currentState == GameStates.TUTORIAL)
        {
            StopAllSounds();
            allowCheck = true;
        }
        else if (GameManager.instance.currentState == GameStates.STORE && !allowCheck)
        {
            audioSource2.Play();
            allowCheck = true;
            Debug.Log("audio2");
        }
    }

    public void PlayShiftAudio()
    {
        audioSource2.clip = spawnSound;
        audioSource2.Play();
    }
}