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


    public int gridCellLength = 10, gridCellHeight = 10;
    public float gridCellSize = 1f;

    public float money = 100;

    public static GameManager instance { get; private set; }

    public GameStates currentState;

    [Header("Clock Settings")]
    [HideInInspector]public float timer;
    private bool isTimer;
    [SerializeField] float timeScaleFactor;
    [SerializeField] Vector2 startStoreTime;
    [SerializeField] Vector2 endTime;
    [SerializeField] Vector2 InitialTime;

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
    }

    private void Update()
    {
        if (isTimer)
        {
            timer += timeScaleFactor * Time.deltaTime;
            Debug.Log(timer);
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
