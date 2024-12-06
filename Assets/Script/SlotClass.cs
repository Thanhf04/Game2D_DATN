using System.Collections;
using UnityEngine;

[System.Serializable]
public class SlotClass
{
    [SerializeField] private ItemClass item;  // Item liên kết với slot
    [SerializeField] private int quanity = 0; // Số lượng của item trong slot

    // Thêm thuộc tính itemName và itemQuantity để tương thích với InventoryManager1.cs
    public string itemName
    {
        get { return item != null ? item.itemName : ""; } // Kiểm tra nếu item không null, lấy tên item
    }

    public int itemQuantity
    {
        get { return quanity; }
    }

    // Constructor mặc định
    public SlotClass()
    {
        item = null;
        quanity = 0;
    }

    // Constructor có tham số
    public SlotClass(ItemClass _item, int _quantity)
    {
        this.item = _item;
        this.quanity = _quantity;
    }

    // Lấy item
    public ItemClass GetItem() { return this.item; }

    // Lấy số lượng item
    public int GetQuantity() { return this.quanity; }

    // Thêm quantity vào item
    public void AddQuantity(int _quantity) { quanity += _quantity; }

    // Giảm quantity của item
    public void SubQuantity(int _quantity) { quanity -= _quantity; }

    // Cập nhật lại số lượng
    public void SetQuantity(int _quantity) { quanity = _quantity; }

    // Thêm item vào slot
    public void AddItem(ItemClass item, int quantity)
    {
        this.item = item;
        this.quanity = quantity;
    }

    // Xóa item khỏi slot
    public void RemoveItem()
    {
        this.item = null;
        this.quanity = 0;
    }

    // Phương thức để xóa item khỏi slot (tương tự như RemoveItem)
    public void ClearSlot()
    {
        item = null; // Đặt item về null
        quanity = 0; // Đặt số lượng về 0
    }
}
