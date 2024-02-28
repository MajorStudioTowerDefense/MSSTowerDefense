using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GridSystem gridSystem;
    public ShelfPlacementManager shelfPlacementManager;
    public ShelfScript[] shelfPrefabs;
    public GameObject cellTilePrefab; // Reference to the cell tile prefab

    public int gridCellLength = 10, gridCellHeight = 10;
    public float gridCellSize = 1f;

    public float money = 100;

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, Vector3.zero); // Initialize the grid system
        shelfPlacementManager.gridSystem = gridSystem;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridCellLength; x++)
        {
            for (int y = 0; y < gridCellHeight; y++)
            {
                // Calculate the world position for the current grid cell
                Vector3 cellPosition = gridSystem.GetWorldPosition(x, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
                // Instantiate the cell tile prefab at the calculated position
                Instantiate(cellTilePrefab, cellPosition, Quaternion.identity, transform);
            }
        }
    }

    // Example method to start placing a shelf
    public void StartPlacingShelfA()
    {
        shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[1]);
    }

    public void StartPlacingShelfB()
    {
        shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[2]);
    }

    public void StartPlacingTable()
    {
        shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[0]);
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }
}
