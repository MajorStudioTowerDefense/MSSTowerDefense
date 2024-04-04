using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeShelf : MonoBehaviour
{
    ShelfScript shelfScript;
    //test
    void Start()
    {
        shelfScript = GetComponent<ShelfScript>();
        shelfScript.shelfTypeNameString = "AxeShelf";
        shelfScript.maxCustomers = 40;
        shelfScript.maxInventory = 120;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }

    
    void Update()
    {
        
    }
}
