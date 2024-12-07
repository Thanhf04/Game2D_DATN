using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;

    private GameObject currentPanel; // Panel hiện đang mở

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OpenPanel(GameObject panel)
    {
        if (currentPanel != null && currentPanel != panel)
        {
            currentPanel.SetActive(false); // Đóng panel đang mở
        }

        currentPanel = panel; // Ghi nhận panel mới
        currentPanel.SetActive(true); // Mở panel mới
    }

    public void ClosePanel(GameObject panel)
    {
        if (currentPanel == panel)
        {
            currentPanel.SetActive(false);
            currentPanel = null; // Xóa tham chiếu khi panel đóng

        }
    }
}
