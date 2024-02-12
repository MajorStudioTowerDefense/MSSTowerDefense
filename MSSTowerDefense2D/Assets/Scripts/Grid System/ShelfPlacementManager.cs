using UnityEngine;

public class ShelfPlacementManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public GameObject[] shelfPrefabs; // Array to hold shelf prefabs
    private GameObject currentShelfPrefab;
    private GameObject currentShelfInstance;
    private int currentPrefabIndex = -1; // Initialize to -1 to indicate no prefab is selected

    private void Update()
    {
        // Select shelf prefab with spacebar only if there is no current shelf being placed
        if (Input.GetKeyDown(KeyCode.Space) && currentShelfInstance == null)
        {
            SelectNextShelfPrefab();
        }

        if (currentShelfPrefab != null && currentShelfInstance == null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f; // Ensure it's in the 2D plane
            int x, y;
            gridSystem.GetXY(mouseWorldPosition, out x, out y);
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);

            // Instantiate a new shelf instance at the mouse position
            currentShelfInstance = Instantiate(currentShelfPrefab, gridPosition, Quaternion.identity);
        }

        if (currentShelfInstance != null)
        {
            // Move the current shelf instance with the mouse
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f; // Ensure it's in the 2D plane
            int x, y;
            gridSystem.GetXY(mouseWorldPosition, out x, out y);
            currentShelfInstance.transform.position = gridSystem.GetWorldPosition(x, y);

            // Rotate the current shelf instance
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentShelfInstance.transform.Rotate(0, 0, 90);
            }

            // Finalize shelf placement
            if (Input.GetMouseButtonDown(0))
            {
                // Lock the shelf in place and prepare for the next shelf selection
                currentShelfInstance = null;
                currentShelfPrefab = null; // Reset currentShelfPrefab if you want to force reselection for each placement
            }
        }
    }

    private void SelectNextShelfPrefab()
    {
        // Increment the currentPrefabIndex, looping back to 0 if it exceeds the array bounds
        currentPrefabIndex = (currentPrefabIndex + 1) % shelfPrefabs.Length;

        // Update the currentShelfPrefab with the next prefab in the array
        currentShelfPrefab = shelfPrefabs[currentPrefabIndex];
    }

    // This method might still be useful if you need to set the prefab in some other way or trigger selection from UI
    public void SetCurrentShelfPrefab(GameObject shelfPrefab)
    {
        currentShelfPrefab = shelfPrefab;
    }
}
