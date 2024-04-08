using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemStat { Name,Weight }
public enum goods { DragonFruit,JackFruit };
public enum weapons { Halbert,Axe };
public enum potions { Love,Haste,Poison };
public enum valuables { Fairydust,Halo,Crystals };

[CreateAssetMenu(fileName = "Goods", menuName = "Item", order = 0)]
public class Items : ScriptableObject
{
    [SerializeField]private goods goodsName;
    public goods GetItem()
    {
        return goodsName;
    }
}
