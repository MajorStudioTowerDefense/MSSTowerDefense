using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public Sprite[] shelfItemSprites;

    // To Store the cost price of each item
    private Dictionary<goods, float> costEntireShelfForGoods = new Dictionary<goods, float>();

    // To Store the selling price of each item
    private Dictionary<goods, float> pricePerUnitForGoods = new Dictionary<goods, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // initialize cost price
            SetCostEntireShelf(goods.Apple, 3f); 
            SetCostEntireShelf(goods.DragonFruit, 4f);
            SetCostEntireShelf(goods.Durian, 6f);
            SetCostEntireShelf(goods.Love, 10f);
            SetCostEntireShelf(goods.Haste, 8f);
            SetCostEntireShelf(goods.Poison, 12f);
            SetCostEntireShelf(goods.UnicornHorn, 10f);
            SetCostEntireShelf(goods.Halo, 20f);
            SetCostEntireShelf(goods.Crystals, 15f);
            SetCostEntireShelf(goods.Halberd, 15f);
            SetCostEntireShelf(goods.Axe, 10f);
            SetCostEntireShelf(goods.Sword, 12f);


            // initialize selling price
            SetPricePerUnit(goods.Apple, 1f); 
            SetPricePerUnit(goods.DragonFruit, 2f); 
            SetPricePerUnit(goods.Durian, 3f); 
            SetPricePerUnit(goods.Love, 3f);
            SetPricePerUnit(goods.Haste, 2f);
            SetPricePerUnit(goods.Poison, 4f);
            SetPricePerUnit(goods.UnicornHorn, 5f);
            SetPricePerUnit(goods.Halo, 10f);
            SetPricePerUnit(goods.Crystals, 7f);
            SetPricePerUnit(goods.Halberd, 17f);
            SetPricePerUnit(goods.Axe, 12f);
            SetPricePerUnit(goods.Sword, 15f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //Set cost price
    public void SetCostEntireShelf(goods goods, float cost)
    {
        costEntireShelfForGoods[goods] = cost;
    }

    //get cost price
    public float GetCostEntireShelf(goods goods)
    {
        if (costEntireShelfForGoods.TryGetValue(goods, out float cost))
        {
            return cost;
        }

        // 如果指定的物品没有设置进价，可以选择返回0或抛出异常
        return 0f;
    }

    // set sell price
    public void SetPricePerUnit(goods goods, float price)
    {
        pricePerUnitForGoods[goods] = price;
    }

    // get sell price
    public float GetPricePerUnit(goods goods)
    {
        if (pricePerUnitForGoods.TryGetValue(goods, out float price))
        {
            return price;
        }

        // 如果指定的物品没有设置售价，可以选择返回0或抛出异常
        return 0f;
    }
}
