using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    public Sprite[] shelfSpritesFull;
    public Sprite[] shelfSpritesHalf;
    public Sprite[] shelfSpritesEmpty;
    public static ShelfManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetShelfSpriteFull(int index)
    {
        return shelfSpritesFull[index];
    }

    public Sprite GetShelfSpriteHalf(int index)
    {
        return shelfSpritesHalf[index];
    }

    public Sprite GetShelfSpriteEmpty(int index)
    {
        return shelfSpritesEmpty[index];
    }
}
