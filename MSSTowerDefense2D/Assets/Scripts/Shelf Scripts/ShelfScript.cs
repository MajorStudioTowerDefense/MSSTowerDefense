using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;
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
    public string shelfTypeNameString = "";
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

    SpriteRenderer spriteRenderer;

    [SerializeField] private float costToBuy;

    public Canvas canvas;

    [Header("Items")]
    public List<Items> itemsCanBeSold;
    public Items sellingItem;
    public shelvesType thisType;
    private int currentStateIndex = -1; // 初始化为-1以确保在开始时更新sprite
    public List<Sprite[]> threeStates; // 假设0=空，1=半满，2=满

    public float Cost { get { return costToBuy; } }

    private TMP_Text loadAmountText;
    [Space(10)]
    public int costToMaintain = 5;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        loadAmount = initalLoadAmount;

        loadAmountText = GetComponentInChildren<TMP_Text>();
        canvas.worldCamera = Camera.main;

        if (loadAmountText == null)
        {
            Debug.LogError("TextMeshPro component not found on the child object!");
        }

        threeStates = new List<Sprite[]>();
        threeStates.Add(ShelfManager.Instance.shelfSpritesEmpty);
        threeStates.Add(ShelfManager.Instance.shelfSpritesHalf);
        threeStates.Add(ShelfManager.Instance.shelfSpritesFull);
        
        
        
        
    }

    void Update()
    {
        showVisibility();
        DetectAndManageCustomers();
        updateSprite();
        if (loadAmountText != null)
        {
            loadAmountText.text = $"Load Amount: {loadAmount}";
        }
    }

    void updateSprite()
    {
        int targetStateIndex;

        if (loadAmount == 0)
        {
            targetStateIndex = 0; 
        }
        else if (loadAmount > 0 && loadAmount < loadAmountMax / 2)
        {
            targetStateIndex = 1; 
        }
        else
        {
            targetStateIndex = 2; 
        }

        // 如果目标sprite索引与当前不同，则更新sprite
        if (targetStateIndex != currentStateIndex)
        {
           if(targetStateIndex == 0)
            {
                spriteRenderer.sprite = threeStates[targetStateIndex][(int)thisType];
                currentStateIndex = targetStateIndex;
            }
            else if(targetStateIndex == 1)
            {
                spriteRenderer.sprite = threeStates[targetStateIndex][(int)sellingItem.GetItem()];
                currentStateIndex = targetStateIndex;
            }
            else
            {
                spriteRenderer.sprite = threeStates[targetStateIndex][(int)sellingItem.GetItem()];
                currentStateIndex = targetStateIndex;
            }
        }
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
            Items wantedItem = sellingItem.GetItem() == item.GetItem() ?  sellingItem:null;
            if (wantedItem != null)
            {
                return wantedItem;
            }
        }
        return null;
    }

    [Header("Purchase")]
    public float purchasePower = 0f;
    
    void Purchase(CustomerData customerData)
    {
        loadAmount--;
        Bot customer = customerData.bot;
        GameManager.instance.AddMoney(ItemManager.Instance.GetPricePerUnit(sellingItem.GetItem()) + purchasePower);
        Debug.Log("Finish purchase!" + customer.selectedItem.GetItem());
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