using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GridSystem gridSystem;
    public ShelfPlacementManager shelfPlacementManager;
    public GameObject shelfPrefab; // Assign this in the inspector

    private void Start()
    {
        gridSystem = new GridSystem(10, 10, 1f, Vector3.zero); // Example parameters
        shelfPlacementManager.gridSystem = gridSystem;
    }

    // Example method to start placing a shelf
    public void StartPlacingShelf()
    {
        shelfPlacementManager.SetCurrentShelfPrefab(shelfPrefab);
    }
}
