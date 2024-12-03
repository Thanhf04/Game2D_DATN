using System.Collections.Generic;
using UnityEngine;

public class GameInventory : MonoBehaviour
{
    // Danh sách các item trong inventory
    public List<string> items = new List<string>();

    // Phương thức trả về danh sách các item
    public List<string> GetItems()
    {
        return items;
    }

    // Phương thức thêm item vào inventory
    public void AddItem(string item)
    {
        items.Add(item);
    }

    // Phương thức xóa item khỏi inventory
    public void RemoveItem(string item)
    {
        items.Remove(item);
    }

    // Phương thức kiểm tra xem có item trong inventory không
    public bool HasItem(string item)
    {
        return items.Contains(item);
    }
}
