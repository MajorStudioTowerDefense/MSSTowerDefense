using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloScript : MonoBehaviour
{
    ShelfScript shelfScript;

    void Start()
    {
        shelfScript = GetComponent<ShelfScript>();
        shelfScript.shelfTypeNameString = "HaloScript";
        shelfScript.maxCustomers = 10;
        shelfScript.maxInventory = 30;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }
    
    void Update()
    {
        
    }
}
