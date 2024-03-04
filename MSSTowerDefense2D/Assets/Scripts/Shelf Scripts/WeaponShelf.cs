using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShelf : ShelfScript
{
    public int upgradeLevel = 0;
    List<string> acceptableNames = new List<string>();


    void Start()
    {
        acceptableNames.Add("Halbert");
        acceptableNames.Add("Axe");
    }

    void Update()
    {
        
    }
}
