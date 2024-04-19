using UnityEngine;

public class ShelfPlacementManager : MonoBehaviour
{
    public static ShelfPlacementManager instance;
    public GridSystem gridSystem;
    public GameObject[] shelfPrefabs;
    private GameObject currentShelfPrefab;
    [SerializeField]private GameObject currentShelfInstance;
    private int currentPrefabIndex = -1;
    public GameObject shelfBeingRepositioned = null;
    private bool[,] shelfPlacementGrid;

    private bool isShelfGridCreated = false;
    public AudioClip ShelfPlaced;

    private int alternativeAreaWidth;
    private int alternativeAreaHeight;
    private int alternativeAreaStartX = 0;
    private int alternativeAreaStartY;

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

    private void Start()
    {
        
    }

    private void Update()
    {

        alternativeAreaWidth = GameManager.instance.alternativeAreaWidth;
        alternativeAreaHeight = GameManager.instance.alternativeAreaHeight;
        alternativeAreaStartY = gridSystem.GetHeight() - alternativeAreaHeight;

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
                AudioManager.instance.PlaySound(ShelfPlaced);

            }
        }

        if (shelfBeingRepositioned == null && currentShelfPrefab != null)
        {
            if (gridSystem != null)
            {
                if (!isShelfGridCreated)
                {
                    shelfPlacementGrid = new bool[gridSystem.GetWidth() + 2, gridSystem.GetHeight() + 1];
                    for (int x = 0; x < gridSystem.GetWidth() + 2; x++)
                    {
                        for (int y = 0; y < gridSystem.GetHeight() + 1; y++)
                        {
                            shelfPlacementGrid[x, y] = false;
                        }
                    }
                    isShelfGridCreated = true;
                }
            }
            else
            {
                Debug.LogError("Grid System is null.");
            }

            SnapShelfToGridAvoidOverlap();
        }

        if (shelfBeingRepositioned != null)
        {
            RepositionShelfAvoidOverlap();
        }
    }

    public void PlacingShelf()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentShelfInstance == null && shelfBeingRepositioned == null)
        {
            SelectNextShelfPrefab();
        }
    }

    public void SetCurrentShelfInstance(GameObject ins)
    {
        currentShelfInstance = ins;
    }

    public void SetCurrentShelfInstanceToNull()
    {
        currentShelfInstance = null;
    }

    public GameObject GetCurrentShelfInstance()
    {
        return currentShelfInstance;
    }

    public GameObject shelfCollectionForReload;
    public void SnapShelfToGridAvoidOverlap()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Adjust for 2D

        int x, y;
        gridSystem.GetXY(mouseWorldPosition, out x, out y);

        if (IsWithinGrid(x, y) && !shelfPlacementGrid[x, y]) // Check if the position is free
        {
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);

            if (currentShelfInstance == null)
            {
                currentShelfInstance = Instantiate(currentShelfPrefab, gridPosition, Quaternion.identity);
                currentShelfInstance.gameObject.transform.SetParent(shelfCollectionForReload.transform);
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
            Debug.Log("Shelf selected for repositioning."+shelfBeingRepositioned.name);
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

    public void RepositionShelfMovedByEmployee(Vector3 shadowPosition, GameObject selectedShelf)
    {
        int x, y;
        gridSystem.GetXY(shadowPosition, out x, out y);

        // First, check if the new position is within the grid and not occupied
        if (IsWithinGrid(x, y) && !shelfPlacementGrid[x, y])
        {
            // Find the previous position of the shelf being repositioned
            int prevX, prevY;
            gridSystem.GetXY(selectedShelf.transform.position, out prevX, out prevY);

            // Update the shelf's position to the corresponding grid position
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);
            selectedShelf.transform.position = gridPosition;

            // Update the shelf placement grid: mark the new position as occupied and the previous one as free
            if (IsWithinGrid(prevX, prevY))
            {
                shelfPlacementGrid[prevX, prevY] = false;
            }
            shelfPlacementGrid[x, y] = true;
        }
    }

    public void RepositionShelfAvoidOverlap()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure it's in the 2D plane for a 2D game

        int x, y;
        gridSystem.GetXY(mouseWorldPosition, out x, out y);

        // First, check if the new position is within the grid and not occupied
        if (IsWithinGrid(x, y) && !shelfPlacementGrid[x, y])
        {
            
            // Find the previous position of the shelf being repositioned
            int prevX, prevY;
            gridSystem.GetXY(shelfBeingRepositioned.transform.position, out prevX, out prevY);

            // Update the shelf's position to the corresponding grid position
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);
            shelfBeingRepositioned.transform.position = gridPosition;

            // Update the shelf placement grid: mark the new position as occupied and the previous one as free
            if (IsWithinGrid(prevX, prevY))
            {
                shelfPlacementGrid[prevX, prevY] = false;
            }
            shelfPlacementGrid[x, y] = true;
        }
    }

    private void FinalizePlacement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        int x, y;
        gridSystem.GetXY(mousePos, out x, out y);
        if (IsWithinGrid(x, y) && !shelfPlacementGrid[x, y]) // Check if the position is free
        {
            GameManager.instance.money -= currentShelfPrefab.GetComponent<ShelfScript>().Cost;
            // Mark the grid position as occupied
            shelfPlacementGrid[x, y] = true;
            currentShelfPrefab = null;
            currentShelfInstance = null;
        }
        else if (currentShelfInstance != null)
        {
            // If placement is not valid, destroy the current instance
            Destroy(currentShelfInstance);
            currentShelfPrefab = null;
            currentShelfInstance = null;
        }
    }

    private bool IsWithinGrid(int x, int y)
    {
        bool isWithinBasicGrid = x >= 2 && y >= 1 && x < gridSystem.GetWidth() + 2 && y < gridSystem.GetHeight() + 1;

        bool isInRestrictedArea = x >= alternativeAreaStartX && x < alternativeAreaStartX + alternativeAreaWidth + 2 &&
                                  y >= alternativeAreaStartY + 1 && y < alternativeAreaStartY + alternativeAreaHeight + 1;

        //Debug.Log("Employee Area Width: " + alternativeAreaWidth);

        return isWithinBasicGrid && !isInRestrictedArea;
    }

    public void PutDownShelfWhenEnterShopState()
    {
        if (currentShelfInstance != null)
        {
            FinalizePlacement();
            AudioManager.instance.PlaySound(ShelfPlaced);
        }
    }
}
