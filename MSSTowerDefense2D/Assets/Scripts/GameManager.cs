using UnityEngine;

public enum GameStates
{
    PREP,
    STORE,
    END
}

public class GameManager : MonoBehaviour
{
    private GridSystem gridSystem;
    public ShelfPlacementManager shelfPlacementManager;
    public GameObject[] shelfPrefabs;
    public GameObject cellTilePrefab; // Reference to the cell tile prefab

    // Wall prefabs
    public GameObject topWallPrefab;
    public GameObject sideWallPrefab;
    public GameObject bottomWallPrefab;

    public int gridCellLength = 10, gridCellHeight = 10;
    public float gridCellSize = 1f;

    public float money = 100;

    public static GameManager instance { get; private set; }

    public GameStates currentState;

    public Transform exit;

    [Header("Clock Settings")]
    [HideInInspector] public float timer;
    private bool isTimer;
    [SerializeField] private float timeScaleFactor;
    [SerializeField] private Vector2 startStoreTime;
    [SerializeField] private Vector2 endTime;
    [SerializeField] private Vector2 InitialTime;

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
        timer = InitialTime.x * 60 + InitialTime.y;

        GenerateGrid();
        GenerateWalls(); // Generate walls after the grid
    }

    private void Update()
    {
        if (isTimer)
        {
            timer += timeScaleFactor * Time.deltaTime;
            if (timer >= startStoreTime.x * 60 + startStoreTime.y && currentState == GameStates.PREP) currentState = GameStates.STORE;
            if (currentState == GameStates.STORE && timer >= endTime.x * 60 + endTime.y) currentState = GameStates.END;
        }

        switch (currentState)
        {
            case GameStates.PREP:
                isTimer = true;
                break;
            case GameStates.STORE:
                isTimer = true;
                break;
            case GameStates.END:
                isTimer = false;
                break;
        }
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridCellLength; x++)
        {
            for (int y = 0; y < gridCellHeight; y++)
            {
                Vector3 cellPosition = gridSystem.GetWorldPosition(x, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
                Instantiate(cellTilePrefab, cellPosition, Quaternion.identity, transform);
            }
        }
    }

    // New method for generating walls
    private void GenerateWalls()
    {
        // Top walls
        for (int x = -1; x < gridCellLength + 1; x++)
        {
            Vector3 position = gridSystem.GetWorldPosition(x, gridCellHeight) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
            Instantiate(topWallPrefab, position, Quaternion.identity, transform);
        }

        // Side walls (excluding the bottom-most tile on each side)
        for (int y = 1; y < gridCellHeight; y++)
        {
            Vector3 leftPosition = gridSystem.GetWorldPosition(-1, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
            Instantiate(sideWallPrefab, leftPosition, Quaternion.identity, transform);

            Vector3 rightPosition = gridSystem.GetWorldPosition(gridCellLength, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
            Instantiate(sideWallPrefab, rightPosition, Quaternion.identity, transform);
        }

        // Bottom walls (for two bottom tiles of the left and right sides)
        Vector3 bottomLeft = gridSystem.GetWorldPosition(-1, 0) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
        Instantiate(bottomWallPrefab, bottomLeft, Quaternion.identity, transform);

        Vector3 bottomRight = gridSystem.GetWorldPosition(gridCellLength, 0) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
        Instantiate(bottomWallPrefab, bottomRight, Quaternion.identity, transform);
    }

    public void StartPlacingShelfA() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[1]);
    public void StartPlacingShelfB() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[2]);
    public void StartPlacingTable() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[0]);

    public void AddMoney(float amount) => money += amount;
}
