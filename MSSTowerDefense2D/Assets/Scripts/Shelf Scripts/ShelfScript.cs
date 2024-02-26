using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using Pathfinding;
using System.Linq;

public class CustomerData
{
    public AIDestinationSetter aiDestinationSetter;
    public Transform originalDestination;
    public float timeAtShelf = 0f;
    public CustomerAI customerAI;

    public CustomerData(AIDestinationSetter ai, Transform originalDestination, CustomerAI customerAI)
    {
        this.aiDestinationSetter = ai;
        this.originalDestination = originalDestination;
        this.customerAI = customerAI;
        this.timeAtShelf = 0f;
    }
}

public class ShelfScript : MonoBehaviour
{
    public string shelfType = "";
    public int maxCustomers;
    private List<CustomerData> currentCustomersData = new List<CustomerData>();
    private List<AIDestinationSetter> removedCustomers = new List<AIDestinationSetter>();
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

    public float detectionRadius = 1f;
    public float customerStayDuration = 5f;

    public string targetObjectName;

    [SerializeField] private float cost;
    [SerializeField] private goods sellingGoods;
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

    // void Start()
    // {
    //     CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        
    //     circleCollider.isTrigger = true;
        
    //     circleCollider.radius = shelfDetectionRange;
    // }

    void Update()
    {
         DetectAndManageCustomers();
    }


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

    void DetectAndManageCustomers()
    {
        AIDestinationSetter[] allAIDestinationSetters = FindObjectsOfType<AIDestinationSetter>();
        foreach (var aiDestinationSetter in allAIDestinationSetters)
        {
            // Skip if this customer has already been removed
            if (removedCustomers.Contains(aiDestinationSetter)) continue;

            float distance = Vector3.Distance(transform.position, aiDestinationSetter.transform.position);
            var existingCustomerData = currentCustomersData.FirstOrDefault(c => c.aiDestinationSetter == aiDestinationSetter);

            // Check for new customer within range and max capacity not exceeded
            if (distance <= detectionRadius && currentCustomersData.Count < maxCustomers)
            {
                aiDestinationSetter.target = transform;
                if (existingCustomerData == null) // New customer detected
                {
                    CustomerAI customerAI = aiDestinationSetter.gameObject.GetComponent<CustomerAI>();
                    if (customerAI != null)
                    {
                        GameObject shopExit = GameObject.Find("TestShopExit");
                        if (shopExit != null)
                        {
                            Transform originalDestination = shopExit.transform;
                            var newCustomerData = new CustomerData(aiDestinationSetter, originalDestination, customerAI);
                            if (distance <= 1f)
                            {
                                currentCustomersData.Add(newCustomerData);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("TestShopExit object not found in the scene.");
                        }
                    }
                }
            }
            // else if (existingCustomerData != null)
            // {
            //     RemoveCustomer(existingCustomerData);
            // }

            // Update and manage customers' stay duration
            foreach (var customer in currentCustomersData.ToList())
            {
                customer.timeAtShelf += Time.deltaTime;
                if (customer.timeAtShelf >= customerStayDuration || customer.customerAI.item.GetItem() != sellingGoods)
                {
                    RemoveCustomer(customer);
                }
            }
        }
    }

    void RemoveCustomer(CustomerData customerData)
    {
       if(customerData.customerAI.item.GetItem() == sellingGoods) GameManager.instance.AddMoney(customerData.customerAI.budget);
        customerData.aiDestinationSetter.target = customerData.originalDestination;
        currentCustomersData.Remove(customerData);
        // Add the AIDestinationSetter to the removedCustomers list
        removedCustomers.Add(customerData.aiDestinationSetter);
    }
}
