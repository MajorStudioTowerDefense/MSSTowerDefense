using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    public MultipleGameSpeed multipleGameSpeed;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
{
    if(Input.GetKeyDown(KeyCode.Alpha1))
    {
        multipleGameSpeed.gameSpeed = 1;
        multipleGameSpeed.SetGameSpeed();
    }

    if(Input.GetKeyDown(KeyCode.Alpha2))
    {
        multipleGameSpeed.gameSpeed = 2;
        multipleGameSpeed.SetGameSpeed();
    }

    if(Input.GetKeyDown(KeyCode.Alpha3))
    {
        multipleGameSpeed.gameSpeed = 4;
        multipleGameSpeed.SetGameSpeed();
    }

    if(Input.GetKeyDown(KeyCode.BackQuote))
    {
        if (!isPaused)
        {
            multipleGameSpeed.gameSpeed = 0;
        multipleGameSpeed.SetGameSpeed();
            isPaused = true;
        }
        else
        {
            multipleGameSpeed.gameSpeed = 1;
        multipleGameSpeed.SetGameSpeed();
            isPaused = false;
        }
    }
}
}
