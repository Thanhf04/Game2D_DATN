using UnityEngine;

public class CloseKillBoss : MonoBehaviour
{
    public GameObject PanelKillBoss;
    public void Close()
    {
        PanelKillBoss.SetActive(false);
    }
}