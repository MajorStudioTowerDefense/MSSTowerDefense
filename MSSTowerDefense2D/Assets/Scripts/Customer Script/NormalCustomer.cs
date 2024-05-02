using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalCustomer : Bot
{
    public Transform ShopExit;
    private List<GridArea> unvisitedAreas = new List<GridArea>();
    private bool movingToExit = false;
    public Vector3 currentDestination;

    public int areaPartitionSizeX = 2;
    public int areaPartitionSizeY = 2;

    private float stopDuration = 2.0f;
    private bool isWaiting = false;

    [HideInInspector] public Bot bot;

    public AudioClip WalkingHeavy;
    public AudioClip Walking;
    public AudioClip CustomerPissed;
    public AudioClip CustomerHappy;

    public AudioClip thump;

    public AudioSource customerAudioSource;

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
        ShopExit = GameObject.FindGameObjectWithTag("Exit").transform;
        InitializeAreas();
        SetFirstAreaDestination();
        bot = GetComponent<Bot>();

        bot.needs = bot.SelectRandomItems(bot.likedItems, bot.desireAmount);
    }
    void InitializeAreas()
    {
        foreach (Room room in GameManager.instance.rooms)
        {
            int startX = Mathf.FloorToInt(room.transform.position.x);
            int startY = Mathf.FloorToInt(room.transform.position.y);
            int roomWidth = room.GetWidth();
            int roomHeight = room.GetHeight();

            for (int x = startX; x < startX + roomWidth; x += areaPartitionSizeX)
            {
                for (int y = startY; y < startY + roomHeight; y += areaPartitionSizeY)
                {
                    Vector2 centerPosition = Vector2.zero;
                    int validCells = 0;

                    for (int xi = x; xi < x + areaPartitionSizeX && xi < startX + roomWidth; xi++)
                    {
                        for (int yi = y; yi < y + areaPartitionSizeY && yi < startY + roomHeight; yi++)
                        {
                            if (GameManager.instance.CanPlaceShelf(new Vector2Int(xi, yi)))
                            {
                                Vector2 worldPos = GameManager.instance.gridSystem.GetWorldPosition(xi, yi);
                                centerPosition += worldPos;
                                validCells++;
                            }
                        }
                    }

                    if (validCells > 0)
                    {
                        centerPosition /= validCells; // Calculate the center of this sub-area
                        unvisitedAreas.Add(new GridArea($"Area_{x}_{y}", centerPosition));
                    }
                }
            }
        }
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
            customerAudioSource.clip = CustomerHappy;
            customerAudioSource.Play();

            Destroy(gameObject);
        }

        if (patience <= 0)
        {
            customerAudioSource.clip = CustomerPissed;
            customerAudioSource.Play();
            MoveToExit();
            return;
        }

        UpdatePatienceSprite();

        switch (bot.isPurchasing)
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
            AudioManager.instance.PlaySound(CustomerPissed);
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
        if (!bot.isPurchasing)
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
        movingToExit = true;
        destinationSetter.targetPosition = ShopExit.position;
    }
}
