using System.Collections.Generic;
using UnityEngine;

public class Model_Shop
{
    public enum ItemType
    {
        HP_1,
        MP_1
    }
    private static Dictionary<ItemType, ItemClass> itemTypeToScriptableObject;

    private void Awake()
    {
        itemTypeToScriptableObject = new Dictionary<ItemType, ItemClass>
        {
            { ItemType.HP_1, Resources.Load<ItemClass>("HP_1") }, // Đường dẫn tới ScriptableObject
            { ItemType.MP_1, Resources.Load<ItemClass>("MP_1") },
            // Thêm các item khác
        };
    }
    public static ItemClass GetItemByType(ItemType itemType)
    {
        if (itemTypeToScriptableObject.TryGetValue(itemType, out ItemClass item))
        {
            return item;
        }
        Debug.LogError("Không tìm thấy ItemClass cho loại item: " + itemType);
        return null;
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

