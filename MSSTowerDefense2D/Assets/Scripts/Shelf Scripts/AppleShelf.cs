using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShelf : MonoBehaviour
{
    ShelfScript shelfScript;

    void Start()
    {
        shelfScript = GetComponent<ShelfScript>();
        shelfScript.shelfType = "AppleShelf";
        shelfScript.maxCustomers = 3;
        shelfScript.maxInventory = 10;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }

    
    void Update()
    {
        
    }
}
