using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionShelf : ShelfScript
{
    enum UpgradeLevel
    {
        Low,
        Medium,
        High
    }
    List<string> acceptableNames = new List<string>();
    List<GameObject> acceptableItems = new List<GameObject>();
    ShelfScript shelfScript;
    private UpgradeLevel level;

    void Start()
    {
        acceptableNames.Add("Love");
        acceptableNames.Add("Haste");
        acceptableNames.Add("Poison");
        shelfScript = GetComponent<ShelfScript>();
        level = UpgradeLevel.Low;
    }

    void Update()
    {
        switch (level)
        {
            case UpgradeLevel.Low:
                shelfScript.maxInventory = 20;
                shelfScript.shelfLength = 1;
                shelfScript.maxCustomers = 2;
                break;
            case UpgradeLevel.Medium:
                shelfScript.maxInventory = 25;
                shelfScript.shelfLength = 2;
                shelfScript.maxCustomers = 4;
                break;
            case UpgradeLevel.High:
                shelfScript.maxInventory = 30;
                shelfScript.shelfLength = 3;
                shelfScript.maxCustomers = 6;
                break;
        }
    }

    public void upgradeShelf()
    {
        if (level == UpgradeLevel.Low)
        {
            level = UpgradeLevel.Medium;
        }
        else if (level == UpgradeLevel.Medium)
        {
            level = UpgradeLevel.High;
        }
        else
        {
            Debug.Log("not possible");
        }
    }

    public void restock(GameObject[] items)
    {
        foreach (GameObject good in items)
        {
            foreach (String name in acceptableNames)
            {
                if (good.name == name)
                {
                    acceptableItems.Add(good);
                    break;
                }
            }

            shelfScript.restock(acceptableItems);
        }
        acceptableItems.Clear();
    }
}