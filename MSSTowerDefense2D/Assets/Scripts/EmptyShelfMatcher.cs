using UnityEngine;

public class EmptyShelfMatcher : MonoBehaviour
{
    private GameObject shelfObject;
    public AudioClip RestockShelf;
 void Start ()
 {
     AudioManager.instance.PlaySound(RestockShelf);
     Debug.Log("Play sound.");
 }
    void Update()
    {
        ShelfScript[] shelves = FindObjectsOfType<ShelfScript>();
        foreach (ShelfScript shelf in shelves)
        {
            if (shelf.loadAmount == 0)
            {
            shelfObject = shelf.gameObject; 
            
            transform.position = shelfObject.transform.position;
            transform.rotation = shelfObject.transform.rotation;
            transform.localScale = shelfObject.transform.localScale;
           

        }
    }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}