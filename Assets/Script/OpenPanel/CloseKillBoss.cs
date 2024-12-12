using UnityEngine;

public class CloseKillBoss : MonoBehaviour
{
    public GameObject PanelKillBoss;
    public void Close()
    {
        PanelManager.Instance.ClosePanel(PanelKillBoss);
    }
}
