using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using Pathfinding;
using System.Linq;
using UnityEditor.SearchService;

public class CustomerData
{
    public AIDestinationSetter aiDestinationSetter;
    public Transform originalDestination;
    public float timeAtShelf = 0f;
    public Bot bot;

    public CustomerData(AIDestinationSetter ai, Transform originalDestination, Bot bot)
    {
        this.aiDestinationSetter = ai;
        this.originalDestination = originalDestination;
        this.bot = bot;
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
    public List<Items> sellingItems;
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

    void Start()
    {       
    //     CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();

    //     circleCollider.isTrigger = true;

    //     circleCollider.radius = shelfDetectionRange;
    }

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
        if (Mathf.Round(transform.position.z / 180) - transform.position.z / 180 == 0)
        {
            foreach (GameObject gridBlock in gridBlockArray)
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
        GameObject shopExit = GameObject.Find("TestShopExit");
        if (shopExit != null)
        {
            Transform originalDestination = shopExit.transform;
        }
        else
        {
            Debug.LogWarning("TestShopExit object not found in the scene.");
        }

        foreach (var aiDestinationSetter in allAIDestinationSetters)
        {
            // Skip customers that have been removed
            if (removedCustomers.Contains(aiDestinationSetter)) continue;

            float distance = Vector3.Distance(transform.position, aiDestinationSetter.transform.position);
            Bot bot = aiDestinationSetter.gameObject.GetComponent<Bot>();

            if (bot != null && distance <= detectionRadius)
            {
                Debug.Log("Buying!");
                bool itemMatch = IsSellingItem(bot.item);
                var existingCustomerData = currentCustomersData.FirstOrDefault(c => c.aiDestinationSetter == aiDestinationSetter);

                // If within range and item matches, and not already being processed
                if (itemMatch && existingCustomerData == null && currentCustomersData.Count < maxCustomers && !bot.isPurchasing)
                {
                    Debug.Log("Comming!");
                    aiDestinationSetter.target = transform;
                    bot.isPurchasing = true;

                    Transform originalDestination = shopExit.transform;
                    var newCustomerData = new CustomerData(aiDestinationSetter, originalDestination, bot);
                    currentCustomersData.Add(newCustomerData); // Add customer for processing
                }
                // If the item doesn't match or max capacity reached, and the customer isn't already being processed
                else
                {
                    removedCustomers.Add(aiDestinationSetter);
                }
            }
        }

        // Update time at shelf for customers and remove after duration
        foreach (var customer in currentCustomersData.ToList())
        {
            customer.timeAtShelf += Time.deltaTime;
            if (customer.timeAtShelf >= customerStayDuration)
            {
                Purchase(customer);
                Debug.Log("Customer leaves after buying or waiting.");
            }
        }
    }


    private bool IsSellingItem(Items customerItem)
    {
        return sellingItems.Any(item => item.name == customerItem.name);
    }

    void Purchase(CustomerData customerData)
    {
        GameManager.instance.AddMoney(customerData.bot.botBudget);
        customerData.aiDestinationSetter.target = customerData.originalDestination;
        RemoveCustomer(customerData);
    }

    void RemoveCustomer(CustomerData customerData)
    {
        customerData.bot.isPurchasing = false;
        currentCustomersData.Remove(customerData);
        // Add the AIDestinationSetter to the removedCustomers list
        removedCustomers.Add(customerData.aiDestinationSetter);
    }
}
