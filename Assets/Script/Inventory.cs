using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel; // Tham chiếu đến Canvas của bạn
    public Button toggleButton;     // Tham chiếu đến Button

    private bool isInventoryVisible = false;

    void Start()
    {
        // Đảm bảo Canvas được tắt khi khởi động
        inventoryPanel.gameObject.SetActive(false);

        // Gán sự kiện cho Button
        toggleButton.onClick.AddListener(ToggleInventory);
    }

    void Update()
    {
        // Kiểm tra nếu người chơi nhấn phím "B"
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    // Hàm bật/tắt Canvas
    void ToggleInventory()
    {
        isInventoryVisible = !isInventoryVisible;
        PanelManager.Instance.OpenPanel(inventoryPanel);
    }
}
