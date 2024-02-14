using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeShelf : MonoBehaviour
{
    ShelfScript shelfScript;

    void Start()
    {
        shelfScript = GetComponent<ShelfScript>();
        shelfScript.shelfType = "AxeShelf";
        shelfScript.maxCustomers = 40;
        shelfScript.maxInventory = 120;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }

    
    void Update()
    {
        
    }
}
