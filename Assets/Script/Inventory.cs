using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel; // Tham chiếu đến Canvas của bạn
    public static bool isInventoryOpen = false;
    public void OpenInventory()
    {
        PanelManager.Instance.OpenPanel(inventoryPanel);
        isInventoryOpen = true;
    }
    public void CloseInventory()
    {
        PanelManager.Instance.ClosePanel(inventoryPanel);
        isInventoryOpen = false;
    }



}
