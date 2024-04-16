using UnityEngine;

public class ShelfMatcher : MonoBehaviour
{
    private GameObject shelf;
    void Update()
    {
        ShelfScript shelfScript = FindObjectOfType<ShelfScript>();

        if (shelfScript != null)
        {
            shelf = shelfScript.gameObject; 

            transform.position = shelf.transform.position;
            transform.rotation = shelf.transform.rotation;
            transform.localScale = shelf.transform.localScale;
        }
    }
}