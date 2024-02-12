using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GridSystem gridSystem;
    public ShelfPlacementManager shelfPlacementManager;
    public GameObject shelfPrefab;

    public int gridCellLength = 10, gridCellHeight = 10;
    public float gridCellSize = 1f;

    private void Start()
    {
        gridSystem = new GridSystem(gridCellLength, gridCellHeight, gridCellSize, Vector3.zero); // Example parameters
        shelfPlacementManager.gridSystem = gridSystem;
    }

    // Example method to start placing a shelf
    public void StartPlacingShelf()
    {
        shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefab);
    }
}
