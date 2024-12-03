using UnityEngine;

public class OpenChiSo : MonoBehaviour
{
    public GameObject PanelChiSo;
    public void Open()
    {
        PanelManager.Instance.OpenPanel(PanelChiSo);
    }
    public void Close()
    {
        PanelManager.Instance.ClosePanel(PanelChiSo);
    }
}
