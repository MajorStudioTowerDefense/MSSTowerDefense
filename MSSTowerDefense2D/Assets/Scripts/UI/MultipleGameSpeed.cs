using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleGameSpeed : MonoBehaviour
{
    public float gameSpeed = 2f;
    public void SetGameSpeed()
    {
        Time.timeScale = gameSpeed;
    }
}
