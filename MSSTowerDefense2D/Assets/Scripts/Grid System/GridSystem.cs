using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;

public class GridSystem
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int roomCount;
    public List<GridArea> areas = new List<GridArea>();

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public int GetWidth() => width;
    public int GetHeight() => height;

    public void AddArea(GridArea area)
    {
        areas.Add(area);
    }
}

public class GridArea
{
    public string Name;
    public Vector3 CenterPosition; // Center position of the area for distance calculations

    public GridArea(string name, Vector3 centerPosition)
    {
        Name = name;
        CenterPosition = centerPosition;
    }
}