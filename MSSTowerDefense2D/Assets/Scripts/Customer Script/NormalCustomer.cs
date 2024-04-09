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

    private Bot bot;

    public AudioClip WalkingHeavy;
    public AudioClip Walking;

    public AudioClip thump;

    void Start()
    {
        base.init();
        ShopExit = GameObject.FindGameObjectWithTag("Exit").transform;
        InitializeAreas();
        SetFirstAreaDestination();
        bot = GetComponent<Bot>();
        AudioManager.instance.PlaySound(WalkingHeavy);
    }

    void InitializeAreas()
    {
        GridSystem gridSystem = GameManager.instance.gridSystem;
        int partitionsX = gridSystem.GetWidth() / areaPartitionSizeX;
        int partitionsY = gridSystem.GetHeight() / areaPartitionSizeY;

        for (int i = 0; i < partitionsX; i++)
        {
            for (int j = 0; j < partitionsY; j++)
            {
                Vector3 centerPosition = gridSystem.GetWorldPosition(i * areaPartitionSizeX + areaPartitionSizeX / 2, j * areaPartitionSizeY + areaPartitionSizeY / 2);
                unvisitedAreas.Add(new GridArea($"Area_{i}_{j}", centerPosition));
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

        Debug.Log("Count: " + unvisitedAreas.Count);
    }

    void PerformWonderingActions()
    {
        if (!movingToExit && !isWaiting)
        {
            if (unvisitedAreas.Count > 0 && Vector3.Distance(transform.position, currentDestination) < 1f)
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
        MoveToNextArea();
    }

    void MoveToNextArea()
    {
        if (unvisitedAreas.Count == 0 || isWaiting) return;

        unvisitedAreas = unvisitedAreas.OrderBy(area =>
            Vector3.Distance(this.transform.position, area.CenterPosition)).ToList();
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
