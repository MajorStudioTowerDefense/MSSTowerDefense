using UnityEngine;
using Pathfinding;

public class CustomerAI : MonoBehaviour
{
    public float budget = 10f;
    public Items item;
    public bool isPurchasing = false;
    [SerializeField] private GameObject desireVFX;

    private void Update()
    {
        desireVFX.SetActive(isPurchasing);
    }
}