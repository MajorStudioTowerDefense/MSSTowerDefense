using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveLoadSystem : MonoBehaviour
{
    public static saveLoadSystem instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This keeps the object alive across scenes.
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicates that might be created.
        }
    }

    public bool isLoadingGame = false;
}
