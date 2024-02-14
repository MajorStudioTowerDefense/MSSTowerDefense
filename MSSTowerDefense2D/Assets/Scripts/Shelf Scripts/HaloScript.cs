using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloScript : MonoBehaviour
{
    ShelfScript shelfScript;

    void Start()
    {
        shelfScript = GetComponent<ShelfScript>();
        shelfScript.shelfType = "AxeShelf";
        shelfScript.maxCustomers = 1;
        shelfScript.maxInventory = 3;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }

    
    void Update()
    {
        
    }
}
