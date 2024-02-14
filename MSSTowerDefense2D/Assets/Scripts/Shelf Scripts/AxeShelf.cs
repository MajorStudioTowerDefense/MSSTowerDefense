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
        shelfScript.maxCustomers = 4;
        shelfScript.maxInventory = 12;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }

    
    void Update()
    {
        
    }
}
