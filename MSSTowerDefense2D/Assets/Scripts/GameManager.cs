using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
    PREP,
    STORE,
    END,
    LOOP
}

public enum WallSide
{
    Top,
    Left,
    Right
}

public class GameManager : MonoBehaviour
{
    private GridSystem gridSystem;
    public ShelfPlacementManager shelfPlacementManager;
    public GameObject[] shelfPrefabs;
    public GameObject cellTilePrefab;

    public GameObject topWallPrefab;
    public GameObject sideWallPrefab;
    public GameObject bottomWallPrefab;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;

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

    [Header("Entrance & Exit Settings")]
    public WallSide entranceSide;
    public WallSide exitSide;
    public int entranceYPosition;
    public int exitYPosition;

    [Header("Game Loop Settings")]
    [SerializeField] private int level = 1;
    [SerializeField] private float difficultyFactor = 1.2f;


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
        InitializeLevel();
        //gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, Vector3.zero);
        //shelfPlacementManager.gridSystem = gridSystem;
        //timer = InitialTime.x * 60 + InitialTime.y;

        //GenerateGrid();
        //GenerateWalls();
    }

    private void Update()
    {
        if (isTimer)
        {
            timer += timeScaleFactor * Time.deltaTime;
            if (timer >= startStoreTime.x * 60 + startStoreTime.y && currentState == GameStates.PREP) currentState = GameStates.STORE;
            if (currentState == GameStates.STORE && timer >= endTime.x * 60 + endTime.y) currentState = GameStates.END;
            if (currentState == GameStates.END) SummaryOfTheDay();
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

    private void InitializeLevel()
    {
        gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, Vector3.zero);
        shelfPlacementManager.gridSystem = gridSystem;
        timer = InitialTime.x * 60 + InitialTime.y;

        GenerateGrid();
        GenerateWalls();
        currentState = GameStates.PREP; 
        isTimer = true; 
    }

    private void SummaryOfTheDay()
    {

    }
    private void StartNextLevel()
    {
        level++; 
        AdjustDifficulty(); 
        
    }

    private void AdjustDifficulty()
    {
        // code here to add difficulties
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

    private void GenerateWalls()
    {
        for (int x = -1; x < gridCellLength + 1; x++)
        {
            for (int y = -1; y < gridCellHeight + 1; y++)
            {
                Vector3 position = gridSystem.GetWorldPosition(x, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
                GameObject prefabToInstantiate = null;

                if (x == -1 && entranceSide == WallSide.Left && entranceYPosition == y)
                    prefabToInstantiate = entrancePrefab;
                else if (x == -1 && exitSide == WallSide.Left && exitYPosition == y)
                    prefabToInstantiate = exitPrefab;
                else if (x == gridCellLength && entranceSide == WallSide.Right && entranceYPosition == y)
                    prefabToInstantiate = entrancePrefab;
                else if (x == gridCellLength && exitSide == WallSide.Right && exitYPosition == y)
                    prefabToInstantiate = exitPrefab;
                else if (y == gridCellHeight && entranceSide == WallSide.Top && entranceYPosition == x)
                    prefabToInstantiate = entrancePrefab;
                else if (y == gridCellHeight && exitSide == WallSide.Top && exitYPosition == x)
                    prefabToInstantiate = exitPrefab;
/*                else if (x == -1 || x == gridCellLength || y == -1 || y == gridCellHeight)
                    prefabToInstantiate = sideWallPrefab;*/

                if (prefabToInstantiate != null)
                    Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
            }
        }

        for (int x = 0; x < gridCellLength; x++)
        {
            Vector3 position = gridSystem.GetWorldPosition(x, gridCellHeight) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
            Instantiate(topWallPrefab, position, Quaternion.identity, transform);
        }

        for (int y = 1; y <= gridCellHeight; y++)
        {
            Vector3 leftPosition = gridSystem.GetWorldPosition(-1, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
            Instantiate(sideWallPrefab, leftPosition, Quaternion.identity, transform);

            Vector3 rightPosition = gridSystem.GetWorldPosition(gridCellLength, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
            Instantiate(sideWallPrefab, rightPosition, Quaternion.identity, transform);
        }

        Vector3 bottomLeft = gridSystem.GetWorldPosition(-1, 0) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
        Instantiate(bottomWallPrefab, bottomLeft, Quaternion.identity, transform);

        Vector3 bottomRight = gridSystem.GetWorldPosition(gridCellLength, 0) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
        Instantiate(bottomWallPrefab, bottomRight, Quaternion.identity, transform);
    }

    public void StartPlacingShelfA() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[1]);
    public void StartPlacingShelfB() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[2]);
    public void StartPlacingTable() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[0]);

    public void AddMoney(float amount) => money += amount;

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
