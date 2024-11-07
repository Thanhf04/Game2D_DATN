using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject currentPanel;  // Panel hiện tại sẽ bị ẩn
    public GameObject newPanel;       // Panel mới sẽ được hiển thị
    public Button toggleButton;        // Nút để chuyển đổi

    private void Start()
    {
        // Đảm bảo panel hiện tại được hiển thị và panel mới bị ẩn khi bắt đầu
        currentPanel.SetActive(true);
        newPanel.SetActive(false);

        // Thêm listener cho nút
        toggleButton.onClick.AddListener(TogglePanel);
    }

    private void TogglePanel()
    {
        // Ẩn panel hiện tại và hiển thị panel mới
        currentPanel.SetActive(false);
        newPanel.SetActive(true);
    }
}
