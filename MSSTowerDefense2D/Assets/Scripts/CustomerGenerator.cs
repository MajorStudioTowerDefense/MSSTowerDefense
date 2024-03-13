using System.Collections.Generic;
using UnityEngine;

public class CustomerGenerator : MonoBehaviour
{
    public GameObject[] customerPrefabs; // Array to hold different customer prefabs
    public Transform spawnPoint; // The GameObject's Transform where customers will be spawned
    public float minGenerateDelay = 1f; // Minimum delay between generating customers
    public float maxGenerateDelay = 3f; // Maximum delay between generating customers
    public int maxCustomers = 10; // Maximum number of customers allowed

    private float nextGenerateTime = 0f; // When the next customer should be generated
    [HideInInspector] public int currentCustomers = 0; // Current number of generated customers

    private bool isShopOpened = false;

    public List<GameObject> customersList = new List<GameObject>();

    void Update()
    {
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            // Check if it's time to generate a new customer and if the current number of customers is below the maximum
            if (Time.time >= nextGenerateTime && currentCustomers < maxCustomers)
            {
                GenerateCustomer();
                // Schedule the next generation time by adding a random delay
                nextGenerateTime = Time.time + Random.Range(minGenerateDelay, maxGenerateDelay);
            }
        }
    }

    void GenerateCustomer()
    {
        if (customerPrefabs.Length > 1 && spawnPoint != null) // Ensure there are at least 2 customer prefabs
        {
            // Generate a random number between 0.0 and 1.0
            float chance = Random.Range(0.0f, 1.0f);
            GameObject customerPrefab;

            if (customerPrefabs.Length > 1)
            {
                // 80% chance for customerPrefabs[0], 20% chance for customerPrefabs[1]
                if (chance < 0.8f) // 0.0 to 0.79... is 80% of the range
                {
                    customerPrefab = customerPrefabs[0];
                }
                else // 0.8 to 1.0 is the remaining 20%
                {
                    customerPrefab = customerPrefabs[1];
                }
            }
            else
            {
                customerPrefab = customerPrefabs[0];
            }

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
