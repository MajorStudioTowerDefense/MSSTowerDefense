using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfUI : MonoBehaviour
{
    public GameObject FruitShelves;
    public GameObject WeaponShelves;
    public GameObject Other;

    public GameObject FruitTab;
    public GameObject WeaponTab;
    public GameObject OtherTab;
    public GameObject ToDisable;

    private List<GameObject> tabs;
    private List<GameObject> tabButtons;


    private void Start()
    {
        tabs = new List<GameObject>() { FruitShelves, WeaponShelves, Other};
        tabButtons = new List<GameObject>() { FruitTab, WeaponTab, OtherTab };
        FruitShelvesOn();
    }

    private void Update()
    {
        if(GameManager.instance.currentState != 0)
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
        Reset();
        FruitShelves.SetActive(true);
        FruitTab.GetComponent<Image>().color = Color.white;
    }

    public void WeaponShelvesOn()
    {
        Reset();
        WeaponShelves.SetActive(true);
        WeaponTab.GetComponent<Image>().color = Color.white;

    }

    public void OtherOn()
    {
        Reset();
        Other.SetActive(true);
        OtherTab.GetComponent<Image>().color = Color.white;


    }

    public void Reset()
    {
        foreach (GameObject tab in tabs)
        {
            tab.SetActive(false);
        }
        foreach (GameObject tabButton in tabButtons)
        {
            tabButton.GetComponent<Image>().color = Color.grey;
        }
    }
}
