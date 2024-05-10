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
        if (day < 1)
        {
            apple.SetActive(true);
        }
        else if (day < 3)
        {
            durian.SetActive(true);
            dragonFruit.SetActive(true);
        }
        else if (day < 7)
        {
            haste.SetActive(true);
            poison.SetActive(true);
            love.SetActive(true);
        }
        else if (day < 10)
        {
            horn.SetActive(true);
            crystal.SetActive(true);
            halo.SetActive(true);
        }
        else
        {
            axe.SetActive(true);
            sword.SetActive(true);
            halbert.SetActive(true);
        }

    }
   
}
