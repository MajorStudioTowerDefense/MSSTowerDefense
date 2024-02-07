using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShelfScript : MonoBehaviour
{
    private int maxCustomers;
    private int currentCustomers;
    private List<GameObject> currentCustomersPeople;
    private int maxInventory;
    private int currentInventory;
    private List<GameObject> currentInventoryItems;
    private bool placed = false;
    private bool allgood = true;

    private void sellInventory(GameObject item, GameObject customer)
    {
        if (currentInventory == 1)
        {
            restockAlert();
        }
        if (currentInventory > 0)
        {
            currentInventoryItems.Remove(item);
            currentInventoryItems.RemoveAll(x => x == null);
            //currentInventory -= 1;
            currentInventory = currentInventoryItems.Count;
            makeCustomerLeave(customer);
        }
    }

    private void restockAlert()
    {
        //TODO flash red, squish/stretch, look into juice effects of towers dying
    }

    public void restock(List<GameObject> newItems)
    {
        foreach (GameObject item in newItems)
        {
            if (currentInventory != maxInventory)
            {
                currentInventoryItems.Add(item);
            }
        }
    }

    public void drawInCustomer(GameObject customer)
    {
        if (currentCustomers < maxCustomers)
        {
            currentCustomersPeople.Add(customer);
        }
    }

    private void makeCustomerLeave(GameObject customer)
    {
        if (currentCustomers > 0)
        {
            currentCustomersPeople.Remove(customer);
            currentCustomersPeople.RemoveAll(x => x == null);
            //currentCustomers -= 1;
            currentCustomers = currentCustomersPeople.Count;
        }
    }

    public void rotateRight()
    {
        placed = false;
        transform.Rotate(0, 90, 0);
        if (verifyPlacement() == true)
        {
            placed = true;
        }
    }

    public void rotateLeft()
    {
        placed = false;
        transform.Rotate(0, -90, 0);
        if (verifyPlacement() == true)
        {
            placed = true;
        }
    }

    private bool verifyPlacement()
    {
        if (allgood) //TODO change this lol
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void deleteShelf()
    {
        Destroy(gameObject);
    }
   
}
