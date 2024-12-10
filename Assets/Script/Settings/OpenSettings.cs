using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject PanelSettings;
    public static bool isSettings = false;
    public void Open()
    {
        PanelManager.Instance.OpenPanel(PanelSettings);
        isSettings = true;
    }
    public void Close()
    {
        PanelManager.Instance.ClosePanel(PanelSettings);
        isSettings = false;
    }
}
