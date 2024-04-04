using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    ShelfScript shelfScript;

    void Start()
    {
        shelfScript = GetComponent<ShelfScript>();
        shelfScript.shelfTypeNameString = "CrystalScript";
        shelfScript.maxCustomers = 30;
        shelfScript.maxInventory = 100;
        shelfScript.currentInventory = shelfScript.maxInventory;
    }
    
    void Update()
    {
        
    }
}
