using UnityEngine;

public class EmptyShelfItemMatcher : MonoBehaviour
{
    private Transform shelfItemObject;

    void Update()
    {
        ShelfScript[] shelves = FindObjectsOfType<ShelfScript>();
        foreach (ShelfScript shelf in shelves)
        {
            if (shelf.loadAmount == 0)
            {
                shelfItemObject = FindDeepChild(shelf.gameObject.transform, "ItemIndicatorPanel");

                transform.position = shelfItemObject.position;
                transform.rotation = shelfItemObject.rotation;
                transform.localScale = shelfItemObject.localScale;
            }
        }
    }

    public Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform found = FindDeepChild(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}