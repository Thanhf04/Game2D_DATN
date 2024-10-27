
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName ="New Item")]
public abstract class ItemClass : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;

    public bool isStackable = true;

    public int maxStackQuantity = 0;

    public abstract ItemClass GetItem();
    public abstract ToolClass GetTool();
    public abstract MiscClass GetMisc();
    public abstract ConsumableClass GetConsumable();
}
