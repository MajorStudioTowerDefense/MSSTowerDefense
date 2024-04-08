using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum shelvesType
{
    Shelf=0,
    HighShelf=1,
    Table=2,
    Rack

}

// 产品类型
public enum goods
{
    Apple=0,
    DragonFruit=1,
    Durian=2,
    Love=3,
    Haste=4,
    Poison=5,
    UnicornHorn=6,
    Halo=7,
    Crystals=8,
    Halberd=9,
    Axe=10,
    Sword=11,
}

[CreateAssetMenu(fileName = "Goods", menuName = "Item", order = 0)]
public class Items : ScriptableObject
{
    [SerializeField]private goods goodsName;
    public goods GetItem()
    {
        return goodsName;
    }

    public shelvesType GetShelfType(goods goods)
    {
        switch (goods)
        {
            case goods.Apple:
            case goods.DragonFruit:
            case goods.Durian:
                return shelvesType.Shelf;
            case goods.Love:
            case goods.Haste:
            case goods.Poison:
                return shelvesType.HighShelf;
            case goods.UnicornHorn:
            case goods.Halo:
            case goods.Crystals:
                return shelvesType.Table;
            case goods.Halberd:
            case goods.Axe:
            case goods.Sword:
                return shelvesType.Rack;
            default:
                return shelvesType.Shelf;
        }
    }

    public shelvesType GetShelfType(ShelfScript shelfscript)
    {
        return shelfscript.thisType;
    }

}
