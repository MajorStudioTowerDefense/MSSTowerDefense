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

    //t is for table
    public GameObject T1;
    public GameObject T2;
    public GameObject T3;


    public GameObject ToDisable;

 
    private List<GameObject> fruits; 
    private List<GameObject> weapons;
    private List<GameObject> potions;
    private List<GameObject> tables;


    private void Start()
    {
        fruits = new List<GameObject>() { F1, F2, F3};
        weapons = new List<GameObject>() { W1, W2, W3 };
        potions = new List<GameObject>() { P1, P2, P3 };
        tables = new List<GameObject>() { T1, T2, T3 };
        FruitShelvesOn();
        WeaponShelvesOn();
        PotionShelvesOn();
        TableShelvesOn();
        //remove these one level building becomes a thing, will probably need to make structure edits
    }

    private void Update()
    {
        if (GameManager.instance.currentState == GameStates.TUTORIAL)
        {
            StartCoroutine(FindCustomer());
        }
        else if(GameManager.instance.currentState == GameStates.PREP)
        {
            ToDisable.SetActive(true);
        }
        else
        {
            ToDisable.SetActive(false);
        }
    }

    IEnumerator FindCustomer()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject[] tags = GameObject.FindGameObjectsWithTag("Customer");

        if (tags[0] != null)
        {
            ToDisable.SetActive(false);
        }
        else
        {
            StartCoroutine(FindCustomer());
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

    public void TableShelvesOn()
    {
        foreach (GameObject table in tables)
        {
            table.SetActive(true);
        }


    }

    public void Reset()
    {
        //nothing to reset anymore but will leave it here just in case;
    }
}
