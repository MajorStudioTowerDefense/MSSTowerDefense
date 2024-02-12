using UnityEngine;

public class GridSystem
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        // Adjust origin position to center the grid
        this.originPosition = originPosition - new Vector3(width * cellSize, height * cellSize, 0) * 0.5f;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        // Calculate world position with the adjusted origin
        return new Vector3(x + 0.5f, y + 0.5f) * cellSize + originPosition;
    }

    // Converts world position to grid coordinates
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
