using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private GameObject currentPanel = null; // Panel hiện tại đang mở

    // Hàm mở panel
    public void OpenPanel(GameObject panel)
    {
        if (currentPanel != null)
        {
            // Nếu có panel đang mở, đóng nó
            currentPanel.SetActive(true);
        }

        // Mở panel mới
        currentPanel = panel;
        currentPanel.SetActive(true);
    }

    // Hàm đóng panel
    public void CloseCurrentPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            currentPanel = null;
        }
    }
}
