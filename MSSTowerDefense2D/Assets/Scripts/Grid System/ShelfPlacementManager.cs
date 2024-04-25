using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShelfPlacementManager : MonoBehaviour
{
    public static ShelfPlacementManager instance;
    public GridSystem gridSystem;
    public GameObject[] shelfPrefabs;
    private GameObject currentShelfPrefab;
    [SerializeField] private GameObject currentShelfInstance;
    private int currentPrefabIndex = -1;
    public GameObject shelfBeingRepositioned = null;
    private Dictionary<Vector2Int, bool> shelfPlacementGrid = new Dictionary<Vector2Int, bool>();

    private bool isShelfGridCreated = false;
    public AudioClip ShelfPlaced;
    public int alternativeAreaWidth;
    private int alternativeAreaStartY;
    private int alternativeAreaHeight;
    private int alternativeAreaStartX = 0;

    [SerializeField] PlayerInteraction playerInteraction;


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
        playerInteraction = this.gameObject.GetComponent<PlayerInteraction>();
    }

    bool stageChangedFromPrepToStore = false;
    private void Update()
    {
        alternativeAreaWidth = GameManager.instance.alternativeAreaWidth;
        alternativeAreaHeight = GameManager.instance.alternativeAreaHeight;
        alternativeAreaStartY = 2;


        if (GameManager.instance.currentState != GameStates.PREP && GameManager.instance.currentState != GameStates.TUTORIAL)
        {
            if(GameManager.instance.currentState == GameStates.STORE && !stageChangedFromPrepToStore)
            {
                stageChangedFromPrepToStore = true;
                if (currentShelfInstance != null && shelfBeingRepositioned == null)
                {
                    // Finalize initial placement if within grid
                    FinalizePlacement();
                    AudioManager.instance.PlaySound(ShelfPlaced);
                }
                if (shelfBeingRepositioned != null)
                {
                    RepositionShelfAvoidOverlap(stageChangedFromPrepToStore);
                }

            }
            return;
        }

        if (playerInteraction.getHoldLongEnough() == true && shelfBeingRepositioned == null)
        {
            if (shelfBeingRepositioned == null && currentShelfInstance == null)
            {
                TrySelectShelfForRepositioning();

            }
            
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (currentShelfInstance != null && shelfBeingRepositioned==null)
            {
                // Finalize initial placement if within grid
                FinalizePlacement();
                AudioManager.instance.PlaySound(ShelfPlaced);
            }


        }
        

        if (shelfBeingRepositioned == null && currentShelfPrefab != null)
        {
            shelfPlacementGrid = GameManager.instance.shelfPlacementGrid;

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
        Vector2Int position = new Vector2Int(x, y);
        if (IsWithinGrid(x, y) && GameManager.instance.shelfPlacementGrid.TryGetValue(position, out bool canPlace)) // Check if the position is free
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

        if (playerInteraction.longHoldGameObject!=null && playerInteraction.longHoldGameObject.GetComponent<ShelfScript>() != null)
        {
            shelfBeingRepositioned = playerInteraction.longHoldGameObject;
            setPreviousPosInGrid();
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
        Vector2Int position = new Vector2Int(x, y);
        if (IsWithinGrid(x, y) && GameManager.instance.shelfPlacementGrid.TryGetValue(position, out bool canPlace))
        {
            // Find the previous position of the shelf being repositioned
            int prevX, prevY;
            
            gridSystem.GetXY(selectedShelf.transform.position, out prevX, out prevY);
            Vector2Int prevPosition = new Vector2Int(prevX, prevY);
            // Update the shelf's position to the corresponding grid position
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);
            selectedShelf.transform.position = gridPosition;

            // Update the shelf placement grid: mark the new position as occupied and the previous one as free
            if (IsWithinGrid(prevX, prevY))
            {
                if (GameManager.instance.shelfPlacementGrid.ContainsKey(prevPosition))
                {
                    GameManager.instance.shelfPlacementGrid[prevPosition] = true;
                }
            }
            if (GameManager.instance.shelfPlacementGrid.ContainsKey(prevPosition))
            {
                GameManager.instance.shelfPlacementGrid[prevPosition] = false;
            }
        }
    }

    Vector2Int prevPositionForRepo;
    void setPreviousPosInGrid()
    {
        // Find the previous position of the shelf being repositioned
        int prevX, prevY;
        gridSystem.GetXY(shelfBeingRepositioned.transform.position, out prevX, out prevY);
        prevPositionForRepo = new Vector2Int(prevX, prevY);
    }
    public void RepositionShelfAvoidOverlap()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure it's in the 2D plane for a 2D game

        int x, y;
        gridSystem.GetXY(mouseWorldPosition, out x, out y);
        Vector2Int position = new Vector2Int(x, y);
        // First, check if the new position is within the grid and not occupied
        if (IsWithinGrid(x, y) && GameManager.instance.shelfPlacementGrid.TryGetValue(position, out bool canPlace))
        {
            

            // Update the shelf's position to the corresponding grid position
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);
            shelfBeingRepositioned.transform.position = gridPosition;

            if (Input.GetMouseButtonDown(0))
            {
                // Update the shelf placement grid: mark the new position as occupied and the previous one as free
                if (GameManager.instance.shelfPlacementGrid.ContainsKey(prevPositionForRepo))
                {
                    GameManager.instance.shelfPlacementGrid[prevPositionForRepo] = true; // Mark the previous position as free
                }
                
                GameManager.instance.shelfPlacementGrid[position] = false; // Mark the new position as occupied
                shelfBeingRepositioned = null;
                Debug.Log("Shelf moved from " + prevPositionForRepo + " to " + position);
                AudioManager.instance.PlaySound(ShelfPlaced);
            }


        }
    }

    public void RepositionShelfAvoidOverlap(bool turn)
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure it's in the 2D plane for a 2D game

        int x, y;
        gridSystem.GetXY(mouseWorldPosition, out x, out y);
        Vector2Int position = new Vector2Int(x, y);
        // First, check if the new position is within the grid and not occupied
        if (IsWithinGrid(x, y) && GameManager.instance.shelfPlacementGrid.TryGetValue(position, out bool canPlace))
        {


            // Update the shelf's position to the corresponding grid position
            Vector3 gridPosition = gridSystem.GetWorldPosition(x, y);
            shelfBeingRepositioned.transform.position = gridPosition;

            if (turn)
            {
                // Update the shelf placement grid: mark the new position as occupied and the previous one as free
                if (GameManager.instance.shelfPlacementGrid.ContainsKey(prevPositionForRepo))
                {
                    GameManager.instance.shelfPlacementGrid[prevPositionForRepo] = true; // Mark the previous position as free
                }

                GameManager.instance.shelfPlacementGrid[position] = false; // Mark the new position as occupied
                shelfBeingRepositioned = null;
                Debug.Log("Shelf moved from " + prevPositionForRepo + " to " + position);
                //if (GameManager.instance.shelfPlacementGrid.ContainsKey(prevPosition))
                //{
                //    GameManager.instance.shelfPlacementGrid[prevPosition] = false;
                //}
            }


        }
    }

    private void FinalizePlacement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        int x, y;
        gridSystem.GetXY(mousePos, out x, out y);
        Vector2Int position = new Vector2Int(x, y);
        if (IsWithinGrid(x, y) && GameManager.instance.shelfPlacementGrid.TryGetValue(position, out bool canPlace)) // Check if the position is free
        {
            GameManager.instance.money -= currentShelfPrefab.GetComponent<ShelfScript>().Cost;
            // Mark the grid position as occupied
            if (GameManager.instance.shelfPlacementGrid.ContainsKey(position))
            {
                GameManager.instance.shelfPlacementGrid[position] = false;
            }
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
        Vector2Int position = new Vector2Int(x, y);
        return GameManager.instance.shelfPlacementGrid.TryGetValue(position, out bool canPlace) && canPlace;
    }

}
