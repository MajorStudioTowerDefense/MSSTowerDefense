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
    LOOP,
    TUTORIAL,
    LOSE
}

public class GameManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public ShelfPlacementManager shelfPlacementManager;
    public Dictionary<Vector2Int, bool> shelfPlacementGrid = new Dictionary<Vector2Int, bool>();
    public GameObject[] shelfPrefabs;
    public GameObject cellTilePrefab;

    public GameObject topWallPrefab;
    public GameObject sideWallPrefab;
    public GameObject bottomWallPrefab;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;
    public GameObject alternativeCellTilePrefab;
    

    [Header("Level Initialization Perameters")]
    public int gridCellLength = 10;
    public int gridCellHeight = 10;
    public float gridCellSize = 1f;
    public int alternativeAreaWidth = 3;
    public int alternativeAreaHeight = 2;
    public AstarPath aStar;

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
    public float money = 100;
    public float yesterdayMoney = 0;

    private int day = 0;

    [Header("Clock Settings")]
    [HideInInspector] public float timer;
    [HideInInspector]public bool isTimer;
    [SerializeField] private float timeScaleFactor;
    [SerializeField] private Vector2 startStoreTime;
    [SerializeField] private Vector2 endTime;
    [SerializeField] private Vector2 InitialTime;
    [SerializeField] private Room roomPrefab;
    [HideInInspector] public List<Room> rooms;

    [Header("Game Loop Settings")]
    public int level = 1;
    [SerializeField] private float difficultyFactor = 1.2f;
    public GameObject LosePanel;

    [Header("Tutorials")]
    public GameStates previousState;

    [Header("Map Layouts")]
    [SerializeField] private TextAsset initLayout;
    [SerializeField] private TextAsset[] layouts;
    private bool isRoomAdded = false;
    private CameraController cameraController;
    private float initialCameraMaxZoom;
    private float initialMoveLimit;

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
        cameraController = FindFirstObjectByType<CameraController>();
        initialCameraMaxZoom = cameraController.maxZoom;
        initialMoveLimit = cameraController.moveLimit.x;
        
        if (GameObject.Find("saveLoad") && saveLoadSystem.instance.isLoadingGame && ES3.KeyExists("money"))
        {

            money = ES3.Load("money", money);
            
            employeeArea = ES3.Load("Employees", employeeArea);
            
            shelfPlacementManager.shelfCollectionForReload = ES3.Load("Shelves", shelfPlacementManager.shelfCollectionForReload);
            
        }
        InitializeLevel();
        //gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, Vector3.zero);
        //shelfPlacementManager.gridSystem = gridSystem;
        //timer = InitialTime.x * 60 + InitialTime.y;

        //GenerateGrid();
        //GenerateWalls();\


        

        FindObjectOfType<AstarPath>().Scan();
        
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) GenerateWalls();

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
            case GameStates.TUTORIAL:
                isTimer = false;
                break;
            case GameStates.LOSE:
                isTimer = false;
                break;
        }
    }

    private void InitializeLevel()
    {
        yesterdayMoney = money;
        gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, new Vector3(-5,5,0));
        shelfPlacementManager.gridSystem = gridSystem;
        timer = InitialTime.x * 60 + InitialTime.y;

        GenerateWalls();
        GenerateOutdoorPlants();

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
            newCustomersAmount = (float)customerGenerators[0].maxCustomers + 5;
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
        if (money <= 0)
        {
            LosePanel.SetActive(true);
            currentState = GameStates.LOSE;
            return;
        }
        upgradePanel.SetActive(true);
    }

    public void confirmUpgrade()
    {
        upgradePanel.SetActive(false);
        StartNextLevel();
    }
    private void StartNextLevel()
    {
        Debug.Log("Next level start");
        level++;
        ReInitLevel();
        GameObject.Find("TheBar").GetComponent<TimeBarUI>().startCo();
    }

    public void ConfirmLose()
    {
        Destroy(this.gameObject);
        Destroy(GameObject.Find("AudioManager"));
        Destroy(GameObject.Find("Pointer"));
        SceneManager.LoadScene("NewStartScene");
    }

    private void UpdateShelfPlacement(Vector2Int gridPosition, bool canPlaceShelf)
    {
        shelfPlacementGrid[gridPosition] = canPlaceShelf;
    }

    public bool CanPlaceShelf(Vector2Int gridPosition)
    {
        return shelfPlacementGrid.TryGetValue(gridPosition, out bool canPlace) && canPlace;
    }

    private void GenerateWalls()
    {
        if (isRoomAdded)
        {
            cameraController.maxZoom += initialCameraMaxZoom / 2;
            cameraController.moveLimit.x += initialMoveLimit;
            cameraController.moveLimit.y += initialMoveLimit;
        }
        else
        {
            isRoomAdded = true;
        }

        Room currentRoom;
        List<List<string>> roomLayout = CSVReader.Read(initLayout);

        if (rooms.Count == 0) 
        {
            currentRoom = Instantiate<Room>(roomPrefab, gridSystem.originPosition, Quaternion.identity);
            currentRoom.Init(gridSystem.originPosition, roomLayout);
        }
        else
        {

            roomLayout = CSVReader.Read(layouts[Random.Range(0, layouts.Length - 1)]);
            currentRoom = Instantiate<Room>(roomPrefab);
            Room selectedRoom = rooms[rooms.Count-1];
            Vector3 newPosition = selectedRoom.AddRoom(currentRoom);
            currentRoom.Init(newPosition, roomLayout);
            CloseExits(rooms[rooms.Count - 1].exitPos);
        }
        rooms.Add(currentRoom);

        int roomWidth = currentRoom.GetWidth();
        int roomHeight = currentRoom.GetHeight();

        gridSystem.GetXY(currentRoom.roomPos, out int initX, out int initY);
        for (int y = 0; y < roomLayout.Count; y++)
        {
            for (int x = 0; x < roomLayout[y].Count; x++)
            {
                Vector3 position = gridSystem.GetWorldPosition(x + initX, initY - y) * gridCellSize;
                GameObject prefabToInstantiate = null;
                Transform wallParent = null;
                switch (roomLayout[y][x])
                {
                    case "t":
                        prefabToInstantiate = cellTilePrefab;
                        UpdateShelfPlacement(new Vector2Int(x + initX, initY - y), true);
                        break;
                    case "i":
                        if(rooms.Count > 1)
                        {
                            wallParent = currentRoom.walls[2].transform;
                            prefabToInstantiate = sideWallPrefab;
                            UpdateShelfPlacement(new Vector2Int(x + initX, initY - y), false);
                            break;
                        }
                        goto case "lw";
                    case "lw":
                        wallParent = currentRoom.walls[2].transform;
                        goto default;
                    case "o":
                        wallParent = currentRoom.walls[3].transform;
                        currentRoom.exitPos.Add(position);
                        goto default;
                    case "rw":
                        wallParent = currentRoom.walls[3].transform;
                        goto default;
                    case "tw":
                        wallParent = currentRoom.walls[0].transform;
                        goto default;
                    case "bw":
                        wallParent = currentRoom.walls[1].transform;
                        goto default;
                    case "wh":
                    case "br":
                    default:
                        prefabToInstantiate = GetPrefabForIdentifier(roomLayout[y][x], x, initX, y, initY, currentRoom);
                        UpdateShelfPlacement(new Vector2Int(x + initX, initY - y), false);
                        break;
                }

                if (prefabToInstantiate != null)
                {
                    GameObject createdObject = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
                    if (wallParent != null) createdObject.transform.parent = wallParent;
                }
            }
        }

        aStar.Scan();
    }

    public void CloseExits(List<Vector2> exits)
    {
        foreach (Vector2 exit in exits)
        {
            Collider2D[] hitBoxs = Physics2D.OverlapPointAll(exit);
            foreach (Collider2D hit in hitBoxs)
            {
                if(hit.CompareTag("Exit")) Destroy(hit.gameObject);
            }
            Instantiate(sideWallPrefab, exit, Quaternion.identity);
        }
    }


    private GameObject GetPrefabForIdentifier(string identifier, int x, int initX, int y, int initY, Room currentRoom)
    {
        // Logic to determine the correct prefab and possibly the wallParent
        // For example:
        switch (identifier)
        {
            case "i":
                return entrancePrefab;
            case "o":
                return exitPrefab;
            case "tw":
                return topWallPrefab;
            case "lw":
                return sideWallPrefab;
            case "rw":
                return sideWallPrefab;
            case "bw":
                return bottomWallPrefab;
            case "wh":
                return alternativeCellTilePrefab;
            case "br":
                return bottomWallPrefab;
        }
        return null;
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
    public void placingCrystal() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[12]);
    public void placingUnicorn() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[10]);
    public void placingHalo() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[11]);



    public void StartPlacingTable() => shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefabs[0]);

    public void AddMoney(float amount) => money += amount;

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TutorialStarts()
    {
        previousState = currentState;
        currentState = GameStates.TUTORIAL;
        //DisableUIInteraction();
    }

    public void TutorialEnds()
    {
        currentState = previousState;
    }

    public GameObject employeeArea;

public int GetGridWidth()
{
    if (shelfPlacementGrid.Count == 0) return 0;
    int minX = int.MaxValue;
    int maxX = int.MinValue;

    foreach (var key in shelfPlacementGrid.Keys)
    {
        if (key.x < minX) minX = key.x;
        if (key.x > maxX) maxX = key.x;
    }

    return maxX - minX + 1; 
}

public int GetGridHeight()
{
    if (shelfPlacementGrid.Count == 0) return 0;
    int minY = int.MaxValue;
    int maxY = int.MinValue;

    foreach (var key in shelfPlacementGrid.Keys)
    {
        if (key.y < minY) minY = key.y;
        if (key.y > maxY) maxY = key.y;
    }

    return maxY - minY + 1;
}


}
