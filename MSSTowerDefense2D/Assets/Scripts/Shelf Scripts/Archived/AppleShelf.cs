using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShelf : MonoBehaviour
{
    ShelfScript shelfScript;

    void Start()
    {
        shelfScript = GetComponent<ShelfScript>();
        shelfScript.shelfTypeNameString = "AppleShelf";
        shelfScript.maxCustomers = 10;
        shelfScript.maxInventory = 50;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }

    
    void Update()
    {
        
    }
}
