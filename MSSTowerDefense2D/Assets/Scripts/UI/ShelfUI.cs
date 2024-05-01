using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfUI : MonoBehaviour
{
    //f is for fruit
    public GameObject apple;
    public GameObject dragonFruit;
    public GameObject durian;

    //w is for weapon
    public GameObject axe;
    public GameObject sword;
    public GameObject halbert;

    //p is for potion
    public GameObject haste;
    public GameObject poison;
    public GameObject love;

    //t is for table
    public GameObject horn;
    public GameObject crystal;
    public GameObject halo;


    public GameObject ToDisable;

    public DayTrackerScript dayTrackerScript;

    private void Start()
    {
    }

    private void Update()
    {
        if (GameManager.instance.currentState == GameStates.TUTORIAL)
        {
            //StartCoroutine(FindCustomer());
            ToDisable.SetActive(true);
        }
        if(GameManager.instance.currentState == GameStates.PREP)
        {
            ToDisable.SetActive(true);
        }
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            ToDisable.SetActive(false);
        }

        int day = dayTrackerScript.dayTracker;
        //we love hardcoding levels
        if (day < 2)
        {
            apple.SetActive(true);
        }
        else if (day < 4)
        {
            dragonFruit.SetActive(true);
        }
        else if (day < 8)
        {
            durian.SetActive(true);
        }
        else if (day < 10)
        {
            haste.SetActive(true);
        }
        else if (day < 12)
        {
            poison.SetActive(true);
        }
        else if (day < 14)
        {
            love.SetActive(true);
        }
        else if (day < 16)
        {
            axe.SetActive(true);
        }
        else if (day < 18)
        {
            sword.SetActive(true);
        }
        else if (day < 20)
        {
            halbert.SetActive(true);
        }
        else if (day < 22)
        {
            horn.SetActive(true);
            crystal.SetActive(true);
        }
        else
        {
            halo.SetActive(true);
        }

    }
   
}
