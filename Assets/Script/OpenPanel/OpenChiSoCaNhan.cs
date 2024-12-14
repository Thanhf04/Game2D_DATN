using UnityEngine;

public class OpenChiSoCaNhan : MonoBehaviour
{
    public GameObject panelChiSoCaNhan;
    public static bool ischisoCaNhan = false;
    public void Open()
    {
        PanelManager.Instance.OpenPanel(panelChiSoCaNhan);
        ischisoCaNhan = true;
    }
    public void Close()
    {
        PanelManager.Instance.ClosePanel(panelChiSoCaNhan);
        ischisoCaNhan = false;
    }
}
