using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodShelf : ShelfScript
{

    public int upgradeLevel = 0;
    List<string> acceptableNames = new List<string>();


    void Start()
    {
        acceptableNames.Add("Apple");
        acceptableNames.Add("Pear");
    }

    void Update()
    {

    }
}
