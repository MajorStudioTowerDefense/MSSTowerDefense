using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using Pathfinding;
using System.Linq;
using UnityEditor.SearchService;
using TMPro;

public class CustomerData
{
    public AIDestinationSetter aiDestinationSetter;
    public NormalCustomer normalCustomer;
    public float timeAtShelf = 0f;
    public Bot bot;

    public CustomerData(AIDestinationSetter ai, NormalCustomer normalCustomer, Bot bot)
    {
        this.aiDestinationSetter = ai;
        this.normalCustomer = normalCustomer;
        this.bot = bot;
        this.timeAtShelf = 0f;
    }
}

public class ShelfScript : MonoBehaviour
{
    public string shelfType = "";
    public int maxCustomers;
    private List<CustomerData> currentCustomersData = new List<CustomerData>();
    private int currentCustomers = 0;
    private List<GameObject> currentCustomersPeople = null;
    public int maxInventory;
    public int currentInventory = 0;
    private List<GameObject> currentInventoryItems = new List<GameObject>();
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

    public float visibility = 1f;
    public float purchaseRadius = 1f;
    public float customerStayDuration = 5f;

    public int loadAmount = 0;
    public int initalLoadAmount = 3;
    public int loadAmountMax = 6;
    public bool loadAllowed = true;

    public string targetObjectName;

    [SerializeField] private float cost;
    public List<Items> sellingItems;
    public float Cost { get { return cost; } }

    private TMP_Text loadAmountText;

    public int maintainingCost = 5;


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

        loadAmount = initalLoadAmount;

        loadAmountText = GetComponentInChildren<TMP_Text>();


        if (loadAmountText == null)
        {
            Debug.LogError("TextMeshPro component not found on the child object!");
        }
    }

    void Update()
    {
        showVisibility();
        DetectAndManageCustomers();

        if (loadAmountText != null)
        {
            loadAmountText.text = $"Load Amount: {loadAmount}";
        }
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
            //currentInventoryItems.RemoveAll(x => x == null);
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
    /*
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

            if (bot != null && distance <= visibility)
            {
                Debug.Log("Buying!");
                bool itemMatch = IsSellingItem(bot.item);
                var existingCustomerData = currentCustomersData.FirstOrDefault(c => c.aiDestinationSetter == aiDestinationSetter);

                // If within range and item matches, and not already being processed
                if (itemMatch && existingCustomerData == null && currentCustomersData.Count < maxCustomers && !bot.isPurchasing)
                {
                    Debug.Log("Comming!");
                    aiDestinationSetter.target = transform;
                    Transform originalDestination = shopExit.transform;

                    if (distance <= purchaseRadius)
                    {
                        if (loadAmount > 0)
                        {
                            Debug.Log("Start Purchase!");
                            bot.isPurchasing = true;
                            var newCustomerData = new CustomerData(aiDestinationSetter, originalDestination, bot);
                            currentCustomersData.Add(newCustomerData); // Add customer for processing
                        }
                        else
                        {
                            removedCustomers.Add(aiDestinationSetter);
                            aiDestinationSetter.target = originalDestination;
                        }
                    }
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
            if (loadAmount <= 0)
            {
                RemoveCustomer(customer);
            }

            customer.timeAtShelf += Time.deltaTime;
            if (customer.timeAtShelf >= customerStayDuration && loadAmount > 0)
            {
                Purchase(customer);
                Debug.Log("Customer leaves after buying or waiting.");
            }
        }
    }*/



    void DetectAndManageCustomers()
    {
        if (GameManager.instance.currentState != GameStates.STORE)
        {
            currentCustomersData.Clear();
            return;
        }
        Transform shopExit = GameObject.FindWithTag("Exit").transform;
        if (shopExit != null)
        {
            Transform originalDestination = shopExit;
        }
        else
        {
            Debug.LogWarning("TestShopExit object not found in the scene.");
        }

        Collider2D[] encounteredCustomers = Physics2D.OverlapCircleAll(transform.position, visibility, 7);

        foreach (Collider2D customer in encounteredCustomers)
        {
            Bot bot;
            AIDestinationSetter ai = customer.GetComponent<AIDestinationSetter>();
            if (ai == null || currentCustomersData.FirstOrDefault(c => c.aiDestinationSetter == ai) != null) continue;
            bot = customer.gameObject.GetComponent<Bot>();
            if (bot != null) { bot.selectedItem = IsSellingItem(bot.needs); }
            NormalCustomer normalCustomer = customer.GetComponent<NormalCustomer>();
            if (bot.selectedItem != null && currentCustomersData.Count < maxCustomers && !bot.isPurchasing)
            {
                if (loadAmount > 0)
                {
                    Debug.Log("While purchasing " + bot.selectedItem);
                    ai.targetPosition = transform.position;
                    bot.isPurchasing = true;
                }
                else
                {
                    bot.isPurchasing = false;
                }
            }

            if (Vector2.Distance(customer.transform.position, transform.position) < purchaseRadius && bot.isPurchasing)
            {
                Debug.Log("Start Purchase!");
                var newCustomerData = new CustomerData(ai, normalCustomer, bot);
                currentCustomersData.Add(newCustomerData);
            }

        }
        Debug.Log(currentCustomersData.ToList().Count);
        // Update time at shelf for customers and remove after duration
        foreach (var customer in currentCustomersData.ToList())
        {
            if (loadAmount <= 0)
            {
                RemoveCustomer(customer);
            }
            customer.timeAtShelf += Time.deltaTime;
            if (customer.timeAtShelf >= customerStayDuration && loadAmount > 0)
            {
                Purchase(customer);
            }
        }
    }


    private Items IsSellingItem(List<Items> customerNeeds)
    {
        foreach (Items item in customerNeeds)
        {
            Items wantedItem = sellingItems.FirstOrDefault(offeredItem => offeredItem.name == item.name);
            if (wantedItem != null)
            {
                return wantedItem;
            }
        }
        return null;
    }


    void Purchase(CustomerData customerData)
    {
        loadAmount--;
        Bot customer = customerData.bot;
        GameManager.instance.AddMoney(customer.botBudget);
        Debug.Log("Finish purchase!" + customer.selectedItem);
        customer.needs.Remove(customer.selectedItem);
        customer.selectedItem = null;
        RemoveCustomer(customerData);
    }

    void RemoveCustomer(CustomerData customerData)
    {
        customerData.bot.isPurchasing = false;
        //customerData.aiDestinationSetter.target = customerData.originalDestination;
        customerData.normalCustomer.SetNextDestination();
        currentCustomersData.Remove(customerData);
    }

    public Transform visibilityIndicator;

    void showVisibility()
    {
        float scaleValue = visibility;
        if (visibilityIndicator != null)
            visibilityIndicator.localScale = new Vector3(scaleValue, scaleValue, 1);
    }
}