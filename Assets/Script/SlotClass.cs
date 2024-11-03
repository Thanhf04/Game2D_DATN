using System.Collections;
using UnityEngine;

[System.Serializable]
public class SlotClass
{
    [SerializeField] private ItemClass item;
    [SerializeField] private int quanity = 0;

    public SlotClass()
    {
        item = null;
        quanity = 0;
    }

    public SlotClass(ItemClass _item, int _quantity)
    {
        this.item = _item;
        this.quanity = _quantity;
    }

    public ItemClass GetItem() { return this.item; }
    public int GetQuantity() { return this.quanity; }
    public void AddQuantity(int _quantity) { quanity = quanity + _quantity; }
    public void SubQuantity(int _quantity) { quanity = quanity - _quantity; }

    public void SetQuantity(int _quantity) { quanity = _quantity; }

    public void AddItem(ItemClass item, int quantity)
    {
        this.item = item;
        this.quanity = quantity;
    }

    public void RemoveItem()
    {
        this.item = null;
        this.quanity = 0;
    }
}
