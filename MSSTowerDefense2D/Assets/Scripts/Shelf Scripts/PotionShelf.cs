using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionShelf : ShelfScript
{
    public int upgradeLevel = 0;
    List<string> acceptableNames = new List<string>();


    void Start()
    {
        acceptableNames.Add("Love");
        acceptableNames.Add("Haste");
        acceptableNames.Add("Poison");
    }

    void Update()
    {

    }
}
