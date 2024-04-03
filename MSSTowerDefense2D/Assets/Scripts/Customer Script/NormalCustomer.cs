using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalCustomer : Bot
{
    public Transform ShopExit;
    private List<GridArea> unvisitedAreas = new List<GridArea>();
    private bool movingToExit = false;

    // Desired partition size
    public int areaPartitionSizeX = 2;
    public int areaPartitionSizeY = 2;

    void Start()
    {
        base.init();
        ShopExit = GameObject.FindGameObjectWithTag("Exit").transform;
        InitializeAreas();
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

        // Initial shuffle
        unvisitedAreas = unvisitedAreas.OrderBy(a => Random.value).ToList();
    }

    protected override void Update()
    {
        base.Update();
        if (!movingToExit)
        {
            if (unvisitedAreas.Count > 0)
            {
                MoveToNextArea();
            }
            else
            {
                MoveToExit();
            }
        }
    }

    void MoveToNextArea()
    {
        if (unvisitedAreas.Count == 0) return;

        // Sort remaining areas by distance to prioritize closer ones
        unvisitedAreas = unvisitedAreas.OrderBy(area =>
            Vector3.Distance(this.transform.position, area.CenterPosition)).ToList();
        GridArea nextArea = unvisitedAreas[0];
        destinationSetter.target.position = nextArea.CenterPosition;
        unvisitedAreas.RemoveAt(0);
    }

    void MoveToExit()
    {
        movingToExit = true;
        destinationSetter.target = ShopExit;
    }
}
