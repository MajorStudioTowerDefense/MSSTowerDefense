using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceTimer : MonoBehaviour
{
    public AudioSource audioSource2;
    public AudioClip TempAmb; 

    // Start is called before the first frame update
    void Start()
    {
        audioSource2.clip = TempAmb;
        StartCoroutine(StartSoundAfterDelay());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Starts the Ambiance
 IEnumerator StartSoundAfterDelay()
    {
        // Wait for 'duration' seconds
        yield return new WaitForSeconds(60);

        // Start the sound
        audioSource2.Play();
    }
}
