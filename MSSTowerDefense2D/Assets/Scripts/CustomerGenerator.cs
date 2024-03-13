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
    private int currentCustomers = 0; // Current number of generated customers

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
        if (customerPrefabs.Length > 0 && spawnPoint != null)
        {
            // Select a random customer prefab
            int index = Random.Range(0, customerPrefabs.Length);
            GameObject customerPrefab = customerPrefabs[index];

            // Instantiate the customer at the spawn point's position
            GameObject cus = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            cus.GetComponent<Bot>().init();
            customersList.Add(cus);

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
