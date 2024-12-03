using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject PanelSettings;

    public void Open()
    {
        PanelManager.Instance.OpenPanel(PanelSettings);
    }
    public void Close()
    {
        PanelManager.Instance.ClosePanel(PanelSettings);
    }
}
