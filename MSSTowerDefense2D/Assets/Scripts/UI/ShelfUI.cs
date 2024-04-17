using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfUI : MonoBehaviour
{
    //f is for fruit
    public GameObject F1;
    public GameObject F2;
    public GameObject F3;

    //w is for weapon
    public GameObject W1;
    public GameObject W2;
    public GameObject W3;

    //p is for potion
    public GameObject P1;
    public GameObject P2;
    public GameObject P3;

    public GameObject ToDisable;

 
    private List<GameObject> fruits; //shut up
    private List<GameObject> weapons;
    private List<GameObject> potions;


    private void Start()
    {
        fruits = new List<GameObject>() { F1, F2, F3};
        weapons = new List<GameObject>() { W1, W2, W3 };
        potions = new List<GameObject>() { P1, P2, P3 };
        FruitShelvesOn();
        WeaponShelvesOn();
        PotionShelvesOn();
        //remove these one level building becomes a thing, will probably need to make structure edits
    }

    private void Update()
    {
        if((GameManager.instance.currentState != GameStates.PREP && GameManager.instance.currentState != GameStates.TUTORIAL && GameManager.instance.previousState != GameStates.PREP))
        {
            ToDisable.SetActive(false);
        }
        else
        {
            ToDisable.SetActive(true);
        }
    }

    public void FruitShelvesOn()
    {
        foreach (GameObject fruit in fruits)
        {
            fruit.SetActive(true);
        }
        
    }

    public void WeaponShelvesOn()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(true);
        }

    }

    public void PotionShelvesOn()
    {
        foreach (GameObject potion in potions)
        {
            potion.SetActive(true);
        }


    }

    public void Reset()
    {
        //nothing to reset anymore but will leave it here just in case;
    }
}
