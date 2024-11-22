using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject PanelSettings;

    public void Open()
    {
        PanelSettings.SetActive(true);
        Debug.Log("fuickkkkkkkk");
    }
    public void Close()
    {
        PanelSettings.SetActive(false);
    }
}
