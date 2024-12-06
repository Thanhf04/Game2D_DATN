using UnityEngine;

public class OpenChiSoCaNhan : MonoBehaviour
{
    public GameObject panelChiSoCaNhan;
    public void Open()
    {
        PanelManager.Instance.OpenPanel(panelChiSoCaNhan);
    }
    public void Close()
    {
        PanelManager.Instance.ClosePanel(panelChiSoCaNhan);
    }
}
