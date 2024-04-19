using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
// Setting up the Audio Sources
    public static AudioManager instance;
    public GameManager gameManager; 
    public AudioSource[] audioSources;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip Ticking;
    public AudioClip StartShift;
    public GameObject ambiHalt;
    public GameObject abmiResume;

    //Assigning certain sounds to play for a limited amount of time
    private void Start()
    {
        ambiHalt = gameManager.TutorialStarts(); // Grabs the tutorial starting method from the game manager script
        abmiResume = gameManager.TutorialEnds(); //Grabs the tutorial ending method from the game manager script
        audioSource.clip = Ticking;
        audioSource2.clip = StartShift;
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
        
        if (!ambiHalt){
            audioSource.Play();
        }
        if (abmiResume)
        {
            audioSource2.Play();

        } 
        
        

}
}