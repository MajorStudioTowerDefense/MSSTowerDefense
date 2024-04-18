using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameStates
{
    PREP,
    STORE,
    END,
    LOOP
}

public class GameManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public ShelfPlacementManager shelfPlacementManager;
    public GameObject[] shelfPrefabs;
    public GameObject cellTilePrefab;

    public GameObject topWallPrefab;
    public GameObject sideWallPrefab;
    public GameObject bottomWallPrefab;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;
    public GameObject alternativeCellTilePrefab;

    [Header("Level Initialization Perameters")]
    public int gridCellLength = 15;
    public int gridCellHeight = 11;
    public float gridCellSize = 1f;
    public int alternativeAreaWidth = 3;
    public int alternativeAreaHeight = 3;
    public float money = 100;
    public float yesterdayMoney = 0;

    [Header("Environment")]
    public PlantProbability[] plantProbabilities;
    public int outdoorGridWidth = 40;
    public int outdoorGridHeight = 40;
    public float plantSpacing = 2.0f;

    [System.Serializable]
    public class PlantProbability
    {
        public GameObject plantPrefab;
        public float probability;  // Probability of this plant being spawned
    }

    public static GameManager instance { get; private set; }
    public GameStates currentState;

    public Transform exit;

    [Header("Revenue Data")]
    public float revenue;
    public float total;
    public float rent;

    private int day = 0;

    [Header("Clock Settings")]
    [HideInInspector] public float timer;
    private bool isTimer;
    [SerializeField] private float timeScaleFactor;
    [SerializeField] private Vector2 startStoreTime;
    [SerializeField] private Vector2 endTime;
    [SerializeField] private Vector2 InitialTime;
    [SerializeField] private Room roomPrefab;
    List<Room> rooms;

    [Header("Game Loop Settings")]
    [SerializeField] private int level = 1;
    [SerializeField] private float difficultyFactor = 1.2f;

    [Header("Tutorials")]
    public GameStates previousState;

    [Header("Map Layouts")]
    [SerializeField] private TextAsset[] layouts;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this.gameObject);

        DontDestroyOnLoad(this);
        rooms = new List<Room>();
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
        if (Input.GetKeyDown(KeyCode.Space)) GenerateWalls();

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
        yesterdayMoney = money;
        gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, Vector3.zero);
        shelfPlacementManager.gridSystem = gridSystem;
        timer = InitialTime.x * 60 + InitialTime.y;

        GenerateWalls();
        currentState = GameStates.PREP;
        isTimer = true;
    }

    private void ReInitLevel()
    {
        day++;
        if (day % 6 == 0) GenerateWalls();
        CustomerGenerator[] customerGenerators = FindObjectsOfType<CustomerGenerator>();
        if (customerGenerators.Length > 0)
        {
            for (int i = customerGenerators[0].customersList.Count - 1; i >= 0; i--)
            {
                GameObject customer = customerGenerators[0].customersList[i];
                if (customer != null)
                {
                    Debug.Log("Delete customers");
                    customerGenerators[0].customersList.RemoveAt(i);
                    customerGenerators[0].currentCustomers--;
                    Destroy(customer);
                }
            }

            float newCustomersAmount = 0;
            newCustomersAmount = (float)customerGenerators[0].maxCustomers * difficultyFactor;
            customerGenerators[0].maxCustomers = Mathf.RoundToInt(newCustomersAmount);
        }

        yesterdayMoney = money;
        timer = InitialTime.x * 60 + InitialTime.y;

        currentState = GameStates.PREP;
        isTimer = true;

        ShelfScript[] shelfScripts = FindObjectsOfType<ShelfScript>();
        foreach (var shelfScript in shelfScripts)
        {
            shelfScript.loadAmount = shelfScript.initalLoadAmount;
        }
    }

    public GameObject summaryPanel;
    public GameObject upgradePanel;
    public TextMeshProUGUI[] summaryText;

    private void SummaryOfTheDay()
    {
        ShelfScript[] shelfScripts = FindObjectsOfType<ShelfScript>();
        NormalEmployee[] employees = FindObjectsOfType<NormalEmployee>();
        int shelfCost = 0;
        int wageCost = 0;
        foreach (var shelfScript in shelfScripts)
        {
            shelfCost += shelfScript.costToMaintain;
        }
        foreach (var employee in employees)
        {
            wageCost += 20;
        }
        summaryPanel.SetActive(true);
        revenue = money - yesterdayMoney;
        rent = (gridCellHeight - 1) * (gridCellLength - 1) * 7;
        total = revenue - shelfCost - wageCost;
        money += total;
        summaryText[0].text = "Revenue Gained " + revenue + "\nSupplies for shelves: " + shelfCost + "\nEmployee Wages: " + wageCost + "\nTotal: " + total + "\n\nEST. RENT DUE SUNDAY: " + rent;
    }

    public void confirmSummary()
    {
        summaryPanel.SetActive(false);
        upgradePanel.SetActive(true);
    }

    public void confirmUpgrade()
    {
        upgradePanel.SetActive(false);
        StartNextLevel();
    }
    private void StartNextLevel()
    {
        level++;
        AdjustDifficulty();
        ReInitLevel();

    }

    private void AdjustDifficulty()
    {
        
    }

    /*
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
    */

    private void GenerateWalls()
    {
        Room currentRoom;
        List<List<string>> room;
        room = CSVReader.Read(layouts[Random.Range(0, layouts.Length - 1)]);

        if (rooms.Count == 0) currentRoom = Instantiate<Room>(roomPrefab, Vector3.zero, Quaternion.identity);
        else
        {
            currentRoom = Instantiate<Room>(roomPrefab);
            Room selectedRoom = rooms[Random.Range(0, rooms.Count)];
            currentRoom.Init(selectedRoom.AddRoom(currentRoom));
        }
        rooms.Add(currentRoom);
        gridSystem.GetXY(currentRoom.roomPos, out int initX, out int initY);
        for (int y = 0; y < room.Count; y++)
        {
            for (int x = 0; x < room[y].Count; x++)
            {
                Vector3 position = gridSystem.GetWorldPosition(x+initX, initY-y) + new Vector3(gridCellSize, gridCellSize);
                GameObject prefabToInstantiate = null;
                Transform wallParent = null;
                switch (room[y][x])
                {
                    case "t":
                        prefabToInstantiate = cellTilePrefab;
                        break;
                    case "i":
                        prefabToInstantiate = entrancePrefab;
                        if(x == initX) wallParent = currentRoom.walls[2].transform;
                        else if (y>initY) wallParent = currentRoom.walls[0].transform;
                        break;
                    case "o":
                        prefabToInstantiate = exitPrefab;
                        if (x > initX) wallParent = currentRoom.walls[2].transform;
                        else if (y == initY) wallParent = currentRoom.walls[0].transform;
                        break;
                    case "tw":
                        wallParent = currentRoom.walls[0].transform;
                        prefabToInstantiate = topWallPrefab;
                        break;
                    case "lw":
                        wallParent = currentRoom.walls[2].transform;
                        prefabToInstantiate = sideWallPrefab;
                        break;
                    case "rw":
                        wallParent = currentRoom.walls[3].transform;
                        prefabToInstantiate = sideWallPrefab;
                        break;
                    case "bw":
                        wallParent = currentRoom.walls[1].transform;
                        prefabToInstantiate = bottomWallPrefab;
                        break;
                    case "wh":
                        prefabToInstantiate = alternativeCellTilePrefab;
                        break;
                    case "br":
                        prefabToInstantiate = bottomWallPrefab;
                        break;

                }

                if (prefabToInstantiate != null)
                    Instantiate(prefabToInstantiate, position, Quaternion.identity, transform).transform.parent = wallParent;
            }
        }
    }

    void GenerateOutdoorPlants()
    {
        float halfWidth = outdoorGridWidth * plantSpacing * 0.5f;
        float halfHeight = outdoorGridHeight * plantSpacing * 0.5f;
        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

        for (int x = 0; x < outdoorGridWidth; x++)
        {
            for (int y = 0; y < outdoorGridHeight; y++)
            {
                Vector3 position = new Vector3((x * plantSpacing) - halfWidth, (y * plantSpacing) - halfHeight, 0);
                Vector2 flatPosition = new Vector2(position.x, position.y);

                foreach (var plantProbability in plantProbabilities)
                {
                    if (Random.Range(0f, 1f) < plantProbability.probability && !occupiedPositions.Contains(flatPosition))
                    {
                        Instantiate(plantProbability.plantPrefab, position, Quaternion.identity);
                        occupiedPositions.Add(flatPosition);
                        break; 
                    }
                }
            }
        }
    }

    public void placingApple() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[1]);
    public void placingDurian() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[2]);
    public void placingDragonFruit() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[3]);
    public void placingHalbert() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[4]);
    public void placingAxe() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[5]);
    public void placingSword() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[6]);
    public void placingLove() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[7]);
    public void placingHaste() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[8]);
    public void placingPoison() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[9]);
    
    public void StartPlacingTable() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[0]);

    public void AddMoney(float amount) => money += amount;

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
