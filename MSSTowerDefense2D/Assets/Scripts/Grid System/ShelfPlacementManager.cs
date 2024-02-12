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
        PlacingShelf();

        if (currentShelfPrefab != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f; // Ensure it's in the 2D plane
            int x, y;
            gridSystem.GetXY(mouseWorldPosition, out x, out y);

            if (IsWithinGrid(x, y))
            {
                Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);

                if (currentShelfInstance == null)
                {
                    // Instantiate a new shelf instance at the grid position
                    currentShelfInstance = Instantiate(currentShelfPrefab, gridPosition, Quaternion.identity);
                }
                else
                {
                    // Move the current shelf instance with the mouse within the grid
                    currentShelfInstance.transform.position = gridPosition;
                }
            }
            else if (currentShelfInstance != null)
            {
                // Destroy the shelf instance if it's outside the grid
                Destroy(currentShelfInstance);
                currentShelfInstance = null;
            }

            // Rotate the current shelf instance
            if (currentShelfInstance != null && Input.GetKeyDown(KeyCode.R))
            {
                currentShelfInstance.transform.Rotate(0, 0, 90);
            }

            // Finalize shelf placement
            if (Input.GetMouseButtonDown(0) && IsWithinGrid(x, y))
            {
                // Lock the shelf in place and prepare for the next shelf selection
                currentShelfInstance = null;
                currentShelfPrefab = null; // Reset currentShelfPrefab if you want to force reselection for each placement
            }
        }
    }

    public void PlacingShelf()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentShelfInstance == null)
        {
            SelectNextShelfPrefab();
        }
    }

    private void SelectNextShelfPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex + 1) % shelfPrefabs.Length;
        currentShelfPrefab = shelfPrefabs[currentPrefabIndex];
    }

    public void SetCurrentShelfPrefab(GameObject shelfPrefab)
    {
        currentShelfPrefab = shelfPrefab;
    }

    private bool IsWithinGrid(int x, int y)
    {
        // Check if the x and y coordinates are within the grid bounds
        return x >= 0 && y >= 0 && x < gridSystem.GetWidth() && y < gridSystem.GetHeight();
    }
}
