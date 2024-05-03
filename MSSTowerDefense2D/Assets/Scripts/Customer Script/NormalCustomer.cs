using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalCustomer : Bot
{
    public Transform ShopExit;
    private List<GridArea> unvisitedAreas = new List<GridArea>();
    private Dictionary<Vector2Int, bool> shelfPlacementGrid = new Dictionary<Vector2Int, bool>();
    private bool movingToExit = false;
    public Vector3 currentDestination;

    public int areaPartitionSizeX = 2;
    public int areaPartitionSizeY = 2;

    private float stopDuration = 2.0f;
    private bool isWaiting = false;

    public bool hasForcedToBuyApple = false;
    public AudioClip WalkingHeavy;
    public AudioClip Walking;
    [SerializeField] private AudioSource customerPlayer;

    public Items apple;

    public AudioClip thump;

    [Header("Patience")]
    public float maxPatience = 30f;
    private float patience;
    public Sprite highPatienceSprite;
    public Sprite mediumPatienceSprite;
    public Sprite lowPatienceSprite;
    public SpriteRenderer patienceSpriteRenderer;

    void Start()
    {
        base.init();
        patience = maxPatience;
        customerPlayer = GetComponent<AudioSource>();
        ShopExit = GameObject.FindGameObjectWithTag("Exit").transform;
        shelfPlacementGrid = GameManager.instance.shelfPlacementGrid;
        InitializeAreas();
        SetFirstAreaDestination();

        needs = SelectRandomItems(likedItems, desireAmount);
    }
    void InitializeAreas()
    {
        // Assuming bounds of the grid are known or calculated:
        int minX = shelfPlacementGrid.Keys.Min(k => k.x);
        int maxX = shelfPlacementGrid.Keys.Max(k => k.x);
        int minY = shelfPlacementGrid.Keys.Min(k => k.y);
        int maxY = shelfPlacementGrid.Keys.Max(k => k.y);

        // Loop through the grid in chunks defined by areaPartitionSizeX and areaPartitionSizeY
        for (int x = minX; x <= maxX; x += areaPartitionSizeX)
        {
            for (int y = minY; y <= maxY; y += areaPartitionSizeY)
            {
                Vector2 centerPosition = Vector2.zero;
                int validCells = 0;

                // Process each cell in the current sub-area
                for (int xi = x; xi < x + areaPartitionSizeX && xi <= maxX; xi++)
                {
                    for (int yi = y; yi < y + areaPartitionSizeY && yi <= maxY; yi++)
                    {
                        Vector2Int pos = new Vector2Int(xi, yi);
                        if (shelfPlacementGrid.ContainsKey(pos) && shelfPlacementGrid[pos])
                        {
                            Vector2 worldPos = GameManager.instance.gridSystem.GetWorldPosition(xi, yi);
                            centerPosition += worldPos;
                            validCells++;
                        }
                    }
                }

                // If there are valid cells, calculate the center and store the area
                if (validCells > 0)
                {
                    centerPosition /= validCells; // Calculate the center of this sub-area
                    unvisitedAreas.Add(new GridArea($"Area_{x}_{y}", centerPosition));
                }
            }
        }

        // Optionally shuffle the list of unvisited areas
        unvisitedAreas = unvisitedAreas.OrderBy(a => Random.value).ToList();
    }


    void SetFirstAreaDestination()
    {
        if (unvisitedAreas.Count > 0)
        {
            unvisitedAreas = unvisitedAreas.OrderBy(a => Random.value).ToList();

            GridArea firstArea = unvisitedAreas[0];
            currentDestination = firstArea.CenterPosition;
            destinationSetter.targetPosition = currentDestination;
            unvisitedAreas.RemoveAt(0);

            MoveToNextArea();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Vector2.Distance(transform.position, ShopExit.position) <= 1.5f)
        {
            Destroy(gameObject);
        }

        if (patience <= 0)
        {
            MoveToExit();
            return;
        }

        UpdatePatienceSprite();

        switch (isPurchasing)
        {
            case false:
                PerformWonderingActions();
                break;
            case true:
                break;
        }

        if (GameManager.instance.currentState == GameStates.END)
        {
            MoveToExit();
            AudioManager.instance.PlaySound(thump);
        }
        //Debug.Log(gameObject.name + " distance from exit: " + distance);

if (GameManager.instance.day == 0 && !hasForcedToBuyApple)
{
        ForceToBuyApple();
}
    }

    void ForceToBuyApple()
    {
        if (!hasForcedToBuyApple)
            {
                needs[0] = apple;
                hasForcedToBuyApple = true;
            }
    }

    void UpdatePatienceSprite()
    {
        float normalizedPatience = patience / maxPatience;

        if (normalizedPatience > 0.66f)  // High patience
        {
            patienceSpriteRenderer.sprite = highPatienceSprite;
        }
        else if (normalizedPatience > 0.33f)  // Medium patience
        {
            patienceSpriteRenderer.sprite = mediumPatienceSprite;
        }
        else  // Low patience
        {
            patienceSpriteRenderer.sprite = lowPatienceSprite;
        }
    }

    void PerformWonderingActions()
    {
        if (!movingToExit && !isWaiting)
        {
            if (unvisitedAreas.Count > 0 && Vector2.Distance(transform.position, currentDestination) < 1f)
            {
                StartCoroutine(WaitAtArea(stopDuration));
            }
            else if (unvisitedAreas.Count == 0)
            {
                MoveToExit();
            }
        }
    }

    IEnumerator WaitAtArea(float duration)
    {
        isWaiting = true;
        yield return new WaitForSeconds(duration);
        isWaiting = false;
        if (!isPurchasing)
        {
            patience -= 10;
        }
        else
        {
            patience += 5;
        }

        if (patience <= 0)
        {
            MoveToExit();
        }
        else
        {
            MoveToNextArea();
        }
    }

    void MoveToNextArea()
    {
        if (!customerPlayer.isPlaying) {customerPlayer.clip = Walking; customerPlayer.Play(); }
        Debug.Log("Moving... Unvisited areas: " + unvisitedAreas.Count);
        if (unvisitedAreas.Count == 0 || isWaiting) return;

        unvisitedAreas = unvisitedAreas.OrderBy(area =>
            Vector2.Distance(this.transform.position, area.CenterPosition)).ToList();
        GridArea nextArea = unvisitedAreas[0];
        currentDestination = nextArea.CenterPosition;
        destinationSetter.targetPosition = currentDestination;
        unvisitedAreas.RemoveAt(0);
    }

    public void SetNextDestination()
    {
        if (unvisitedAreas.Count > 0)
        {
            currentDestination = unvisitedAreas[0].CenterPosition;
        }
        else
        {
            currentDestination = ShopExit.position;
        }
        destinationSetter.targetPosition = currentDestination;
    }

    void MoveToExit()
    {
        if (!customerPlayer.isPlaying) { customerPlayer.clip = Walking; customerPlayer.Play(); }
        movingToExit = true;
        destinationSetter.targetPosition = ShopExit.position;
    }
}
