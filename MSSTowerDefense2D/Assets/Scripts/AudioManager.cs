using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private List<AudioSource> sources;
    public bool allowCheck;
    private bool hasPlayedShiftSound = false;

    public AudioClip spawnSound;
    public float volume = 0.7f;

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
            if (hasPlayedShiftSound == true)
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
        StartCoroutine(FindAudios());

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


    void Update()
    {

        /*
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
        }*/
    }

    public void PlayShiftAudio()
    {
        audioSource2.clip = spawnSound;
        audioSource2.Play();
    }

    public void OnVolumeChange()
    {
        volume = slider.value;
        foreach (var source in sources)
        {
            
            source.volume = volume;
        }
    }

    IEnumerator FindAudios()
    {
        while (true)
        {
            Debug.Log("Looking for AudioSources");
            yield return new WaitForSeconds(1);
            sources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
            Debug.Log("Found " + sources.Count + " AudioSources");
            // 可以在这里调用一个方法来处理找到的AudioSource列表，例如更新音量
            UpdateAllVolumes(volume);
        }
    }

    public void UpdateAllVolumes(float volume)
    {
        
        foreach (var src in sources)
        {
            if (src != null)
            {
                src.volume = volume;
            }
        }
    }
}