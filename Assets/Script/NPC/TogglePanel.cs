using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    // Tham chiếu đến GameObject của Panel
    public GameObject panel;

    // Hàm này sẽ được gọi khi bấm nút
    public void TogglePanelVisibility()
    {
        if (panel != null)
        {
            // Đổi trạng thái của Panel (active/inactive)
            panel.SetActive(!panel.activeSelf);
        }
    }
}
