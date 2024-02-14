using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShelfScript : MonoBehaviour
{
    public string shelfType = "";
    public int maxCustomers;
    private int currentCustomers = 0;
    private List<GameObject> currentCustomersPeople = null;
    public int maxInventory;
    public int currentInventory = 0;
    private List<GameObject> currentInventoryItems = null;
    private bool placed = false;
    private bool allgood = true;
    private ShelfUIScript shelfUIScript;
    public int shelfLength;
    public GameObject gridBlockMain;
    public List<GameObject> gridBlockArray;
    private GameObject gridBlockLeft;
    private GameObject gridBlockRight;
    private GameObject gridBlockAbove;
    private GameObject gridBlockBelow;

    [SerializeField] private float cost;
    public float Cost { get { return cost; } }


    //private void OnMouseDown()
    //{
    //    if (shelfUIScript = null)
    //    {
    //        shelfUIScript.currentShelf = gameObject;
    //    }
    //    else if ((shelfUIScript.currentShelf = gameObject) && verifyPlacement() == true)
    //    {
    //        shelfUIScript.currentShelf = null;
    //    }
    //    else if ((shelfUIScript.currentShelf = gameObject) && verifyPlacement() == false)
    //    {
    //        shelfUIScript.currentShelf = null;
    //        deleteShelf();
    //    }
    //}


    private void sellInventory(GameObject item, GameObject customer)
    {
        if (currentInventory == 2)
        {
            restockAlert();
        }
        else if (currentInventory > 2)
        {
            StopCoroutine(flash());
        }
        if (currentInventory > 0)
        {
            currentInventoryItems.Remove(item);
            currentInventoryItems.RemoveAll(x => x == null);
            currentInventory = currentInventoryItems.Count;
            makeCustomerLeave(customer);
        }
        if (currentInventory == 0)
        {
            StopCoroutine(flash());
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.color = new Color(0, 0, 0);
        }
    }

    private void restockAlert()
    {
        StartCoroutine(flash());
    }

    IEnumerator flash()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(1, 0, 0);
        yield return new WaitForSeconds(0.5f);
        renderer.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(flash());
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
            currentCustomers = currentCustomersPeople.Count;
        }
    }

    public void rotateRight()
    {
        placed = false;
        transform.Rotate(0, 0, -90);
        if (verifyPlacement() == true)
        {
            placed = true;
        }
    }

    public void rotateLeft()
    {
        placed = false;
        transform.Rotate(0, 0, 90);
        if (verifyPlacement() == true)
        {
            placed = true;
        }
    }

    public bool verifyPlacement()
    {
        if (Mathf.Round(transform.position.z/180) - transform.position.z/180 == 0)
        {
            foreach(GameObject gridBlock in gridBlockArray)
            {
                if (gridBlockMain.transform.position.x + 1 == gridBlock.transform.position.x)
                {
                    gridBlockRight = gridBlock;
                }
                if (gridBlockMain.transform.position.x - 1 == gridBlock.transform.position.x)
                {
                    gridBlockLeft = gridBlock;
                }
            }
            //gridBlockLeft = null;
            //gridBlockRight = null;
            return true;

        }
        else
        {
            foreach (GameObject gridBlock in gridBlockArray)
            {
                if (gridBlockMain.transform.position.y + 1 == gridBlock.transform.position.x)
                {
                    gridBlockAbove = gridBlock;
                }
                if (gridBlockMain.transform.position.y - 1 == gridBlock.transform.position.x)
                {
                    gridBlockAbove = gridBlock;
                }
            }
            // if gridBlock to the obove is open and exists && if gridblock to the below is open and exists
            //return true;
            //else
            //return false;
            return true;
        }
    }

    public void deleteShelf()
    {
        Destroy(gameObject);
    }
   
}
