using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuableShelf : MonoBehaviour
{
    public int upgradeLevel = 0;
    List<string> acceptableNames = new List<string>();


    void Start()
    {
        acceptableNames.Add("Fairydust");
        acceptableNames.Add("Halo");
        acceptableNames.Add("Crystals");
    }

    void Update()
    {

    }
}
