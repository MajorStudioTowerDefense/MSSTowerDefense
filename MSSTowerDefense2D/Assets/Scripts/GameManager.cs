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
    TUTORIAL
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
    public int gridCellLength = 10;
    public int gridCellHeight = 10;
    public float gridCellSize = 1f;
    public int alternativeAreaWidth = 3;
    public int alternativeAreaHeight = 2;

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

    private List<List<string>> room;

    public static GameManager instance { get; private set; }
    public GameStates currentState;

    public Transform exit;

    [Header("Revenue Data")]
    public float revenue;
    public float total;
    public float rent;
    public float money = 100;
    public float yesterdayMoney = 0;

    [Header("Clock Settings")]
    [HideInInspector] public float timer;
    [HideInInspector]public bool isTimer;
    [SerializeField] private float timeScaleFactor;
    [SerializeField] private Vector2 startStoreTime;
    [SerializeField] private Vector2 endTime;
    [SerializeField] private Vector2 InitialTime;

    [Header("Game Loop Settings")]
    public int level = 1;
    [SerializeField] private float difficultyFactor = 1.2f;

    [Header("Tutorials")]
    public GameStates previousState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this.gameObject);
        room = CSVReader.Read("LevelEditor");

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        
        
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
        
        if (isTimer)
        {
            timer += timeScaleFactor * Time.deltaTime;
            if (timer >= startStoreTime.x * 60 + startStoreTime.y && currentState == GameStates.PREP)
            {
                shelfPlacementManager.PutDownShelfWhenEnterShopState();
                currentState = GameStates.STORE;
            }
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
        }

    }

    private void InitializeLevel()
    {
        yesterdayMoney = money;
        gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, Vector3.zero);
        shelfPlacementManager.SetGridSystem(gridSystem);
        timer = InitialTime.x * 60 + InitialTime.y;

        GenerateWalls();
        GenerateOutdoorPlants();

        currentState = GameStates.PREP;
        isTimer = true;
    }

    private void ReInitLevel()
    {
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

    /*
    private void GenerateGrid()
    {
        for (int x = 0; x < gridCellLength; x++)
        {
            for (int y = 0; y < gridCellHeight; y++)
            {
                Vector3 cellPosition = gridSystem.GetWorldPosition(x, y) + new Vector3(gridCellSize, gridCellSize) * 0.5f;
                GameObject prefabToUse = cellTilePrefab;

                if (x < alternativeAreaWidth && y >= gridCellHeight - alternativeAreaHeight)
                {
                    prefabToUse = alternativeCellTilePrefab;
                }

                Instantiate(prefabToUse, cellPosition, Quaternion.identity, transform);
            }
        }
    }
    */

    private void GenerateWalls()
    {
        for (int y = 0; y < room.Count; y++)
        {
            for (int x = 0; x < room[y].Count; x++)
            {
                Vector3 position = gridSystem.GetWorldPosition(x, -y+9) + new Vector3(gridCellSize, gridCellSize);
                GameObject prefabToInstantiate = null;

                switch (room[y][x])
                {
                    case "t":
                        prefabToInstantiate = cellTilePrefab;
                        break;
                    case "i":
                        prefabToInstantiate = entrancePrefab;
                        break;
                    case "o":
                        prefabToInstantiate = exitPrefab;
                        break;
                    case "tw":
                        prefabToInstantiate = topWallPrefab;
                        break;
                    case "sw":
                        prefabToInstantiate = sideWallPrefab;
                        break;
                    case "bw":
                        prefabToInstantiate = bottomWallPrefab;
                        break;
                    case "wh":
                        prefabToInstantiate = alternativeCellTilePrefab;
                        break;

                }

                if (prefabToInstantiate != null)
                    Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
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
    }

    public void TutorialEnds()
    {
        currentState = previousState;
    }

    public GameObject employeeArea;

}
