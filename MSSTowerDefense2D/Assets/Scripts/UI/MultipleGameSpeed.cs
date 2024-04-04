using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultipleGameSpeed : MonoBehaviour
{
    public float gameSpeed = 2f;
    public AudioSource[] audioSource;
    public void SetGameSpeed()
    {
        Time.timeScale = gameSpeed;
        foreach (AudioSource audio in audioSource)
        {
            audio.pitch = gameSpeed;
        }
        
    }
}
