using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemStat { Name,Weight }
public enum goods { Apple,Durian,DragonFruit };
public enum weapons { Halbert,Axe,Sword };
public enum potions { Love,Haste,Poison };
public enum valuables { UnicornHorn,Halo,Crystals };

[CreateAssetMenu(fileName = "Goods", menuName = "Item", order = 0)]
public class Items : ScriptableObject
{
    [SerializeField]private goods goodsName;
    public goods GetItem()
    {
        return goodsName;
    }
}
