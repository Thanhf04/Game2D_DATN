using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject PanelSettings;

    public void Open()
    {
        PanelSettings.SetActive(true);
    }
    public void Close()
    {
        PanelSettings.SetActive(false);
    }
}
