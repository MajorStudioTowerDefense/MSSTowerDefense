using System.Collections.Generic;
using UnityEngine;

public class PredeterminedCustomerGenerator : MonoBehaviour
{
    public GameObject[] customerPrefabs; // Array to hold different customer prefabs
    public Transform spawnPoint; // The GameObject's Transform where customers will be spawned
    private float minGenerateDelay = 1f; // Minimum delay between generating customers
    private float maxGenerateDelay = 3f; // Maximum delay between generating customers
    public int maxCustomers = 10; // Maximum number of customers allowed 

    private float nextGenerateTime = 0f; // When the next customer should be generated
    [HideInInspector] public int currentCustomers = 0; // Current number of generated customers

    private bool isShopOpened = false;
    private bool allowNewList = true; //bool to only generate list once;

    public int numHalfling, numRhino, numElf; //number of objects upcoming in predList
    private IncomingCustomers incCustomersScript;

    public List<GameObject> customersList = new List<GameObject>();
    public List<GameObject> predeterminedCustomersList = new List<GameObject>();


    void Update()
    {
        incCustomersScript = GameObject.Find("Customer Info").GetComponent<IncomingCustomers>();

        maxGenerateDelay = 120 / maxCustomers;
        minGenerateDelay = maxGenerateDelay / 2;


        if (GameManager.instance.currentState == GameStates.PREP && allowNewList == true)
        {
            numHalfling = 0;
            numRhino = 0;
            numElf = 0;
            GenerateCustomers();
            allowNewList = false;
            incCustomersScript.halfling.text = numHalfling.ToString();
            incCustomersScript.rhino.text = numRhino.ToString();
            incCustomersScript.elf.text = numElf.ToString();
        }
        else if (GameManager.instance.currentState == GameStates.END)
        {
            allowNewList = true;
        }


        if (GameManager.instance.currentState == GameStates.STORE)
        {
            // Check if it's time to generate a new customer and if the current number of customers is below the maximum
            if (Time.time >= nextGenerateTime && currentCustomers < maxCustomers)
            {
                SpawnCustomer();
                // Schedule the next generation time by adding a random delay
                nextGenerateTime = Time.time + Random.Range(minGenerateDelay, maxGenerateDelay);
            }
        }
    }

    public void GenerateCustomers()
    {
        for (int i = 0; i < maxCustomers; i++)
        {
            // Generate a random number between 0.0 and 1.0
            float chance = Random.Range(0.0f, 1.0f);
            GameObject customerPrefab;

            if (customerPrefabs.Length > 1)
            {
                // 60% chance for customerPrefabs[0], 20% chance for customerPrefabs[1], 20% chance for customerPrefabs[2]
                if (chance < 0.6f) // 0.0 to 0.59... is 60% of the range
                {
                    customerPrefab = customerPrefabs[0];
                    predeterminedCustomersList.Add(customerPrefab);
                    numHalfling++;
                }
                else if (chance < 0.8f) //some more for the middle one cuz I added the third final customer sry if i leave the rest of the commented numbers wrong
                {
                    customerPrefab = customerPrefabs[1];
                    predeterminedCustomersList.Add(customerPrefab);
                    numRhino++;
                }
                else // 0.8 to 1.0 is the remaining 20%
                {
                    customerPrefab = customerPrefabs[2];
                    predeterminedCustomersList.Add(customerPrefab);
                    numElf++;
                }
            }
            else
            {
                customerPrefab = customerPrefabs[0];
                predeterminedCustomersList.Add(customerPrefab);
                numHalfling++;
            }
        }
       
    }

    void SpawnCustomer()
    {
        if (customerPrefabs.Length > 1 && spawnPoint != null && predeterminedCustomersList.Count != 0)
        {
            GameObject customerPrefab = predeterminedCustomersList[predeterminedCustomersList.Count-1];
            predeterminedCustomersList.RemoveAt(predeterminedCustomersList.Count - 1);

            // Instantiate the selected customer at the spawn point's position
            GameObject cus = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            cus.GetComponent<Bot>().init(); // Initialize the bot component
            customersList.Add(cus); // Add the customer to the list

            // Increment the current customer count
            currentCustomers++;
        }
        
    }


    // Optional: A method to decrement the current customer count, called when a customer exits or is removed
    public void CustomerExited()
    {
        if (currentCustomers > 0)
        {
            currentCustomers--;
        }
    }

    public void OpenShop()
    {
        if (!isShopOpened)
        {
            isShopOpened = true;
        }
    }
}