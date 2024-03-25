using UnityEngine;

public class ShelfPlacementManager : MonoBehaviour
{
    public static ShelfPlacementManager instance;
    public GridSystem gridSystem;
    public GameObject[] shelfPrefabs;
    private GameObject currentShelfPrefab;
    private GameObject currentShelfInstance;
    private int currentPrefabIndex = -1;
    public GameObject shelfBeingRepositioned = null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (shelfBeingRepositioned == null && currentShelfInstance == null)
            {
                TrySelectShelfForRepositioning();
            }
            else if (shelfBeingRepositioned != null)
            {
                // Finalize repositioning if within grid
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                int x, y;
                gridSystem.GetXY(mousePos, out x, out y);
                if (IsWithinGrid(x, y))
                {
                    shelfBeingRepositioned = null;
                }
            }
            else if (currentShelfInstance != null)
            {
                // Finalize initial placement if within grid
                FinalizePlacement();
            }
        }

        if (shelfBeingRepositioned == null && currentShelfPrefab != null)
        {
            SnapShelfToGrid();
        }

        if (shelfBeingRepositioned != null)
        {
            RepositionShelf();
        }
    }

    public void PlacingShelf()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentShelfInstance == null && shelfBeingRepositioned == null)
        {
            SelectNextShelfPrefab();
        }
    }

    void SnapShelfToGrid()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Adjust for 2D

        int x, y;
        gridSystem.GetXY(mouseWorldPosition, out x, out y);

        if (IsWithinGrid(x, y))
        {
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);

            if (currentShelfInstance == null)
            {
                currentShelfInstance = Instantiate(currentShelfPrefab, gridPosition, Quaternion.identity);
            }
            else
            {
                currentShelfInstance.transform.position = gridPosition;
            }
        }
    }

    void TrySelectShelfForRepositioning()
    {

        Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<ShelfScript>() != null)
        {
            shelfBeingRepositioned = hit.collider.gameObject;
        }
    }

    private void SelectNextShelfPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex + 1) % shelfPrefabs.Length;
        currentShelfPrefab = shelfPrefabs[currentPrefabIndex];
    }

    public void SetCurrentShelfPrefab(GameObject shelfPrefab)
    {
        // Check if we are currently not in the process of placing or repositioning a shelf
        if (currentShelfInstance == null && shelfBeingRepositioned == null)
        {
            // Assuming GameManager.instance.money and shelfPrefab.GetComponent<ShelfScript>().Cost are valid
            ShelfScript shelfScript = shelfPrefab.GetComponent<ShelfScript>();
            if (shelfScript != null && GameManager.instance.money >= shelfScript.Cost)
            {
                GameManager.instance.money -= shelfScript.Cost;
                currentShelfPrefab = shelfPrefab;
                // Reset the index or manage prefab selection state as needed
                currentPrefabIndex = -1; // Reset or adjust according to your logic
            }
        }
        else
        {
            Debug.Log("Currently placing or repositioning a shelf. Please wait.");
        }
    }


    void RepositionShelf()
    {

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure it's in the 2D plane for a 2D game

        // Convert the mouse position to grid coordinates
        int x, y;
        gridSystem.GetXY(mouseWorldPosition, out x, out y);

        if (IsWithinGrid(x, y))
        {
            // If within the grid, update the shelf's position to the corresponding grid position
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);
            shelfBeingRepositioned.transform.position = gridPosition;
        }
        else
        {
            // Clamp the grid coordinates to ensure they're within the grid bounds
            x = Mathf.Clamp(x, 0, gridSystem.GetWidth() - 1);
            y = Mathf.Clamp(y, 0, gridSystem.GetHeight() - 1);

            // Update the shelf's position to the nearest valid grid position
            Vector3 nearestGridPosition = gridSystem.GetWorldPosition(x, y);
            shelfBeingRepositioned.transform.position = nearestGridPosition;
        }
    }

    private void FinalizePlacement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        int x, y;
        gridSystem.GetXY(mousePos, out x, out y);
        if (IsWithinGrid(x, y))
        {
            currentShelfPrefab = null;
            currentShelfInstance = null;
        }
    }


    private bool IsWithinGrid(int x, int y)
    {
        // Check if the x and y coordinates are within the grid bounds
        return x >= 0 && y >= 0 && x < gridSystem.GetWidth() && y < gridSystem.GetHeight();
    }
}
