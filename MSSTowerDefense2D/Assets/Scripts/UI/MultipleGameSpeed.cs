using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultipleGameSpeed : MonoBehaviour
{
    public float gameSpeed = 2f;
    public AudioSource audioSource;
    public AudioClip Pause_Resume;
    public void SetGameSpeed()
    {
        Time.timeScale = gameSpeed;
        audioSource.clip = Pause_Resume;
        audioSource.Play();


    }
}
