using UnityEngine;

public class Model_Shop
{
    public enum ItemType
    {
        HP_1,
        MP_1
    }
    public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.HP_1: return 10;
            case ItemType.MP_1: return 5;
        }
    }
    public static Sprite GetSprite(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.HP_1: return GameAssets.i.s_HP_1;
            case ItemType.MP_1: return GameAssets.i.s_MP_1;
        }
    }
}

