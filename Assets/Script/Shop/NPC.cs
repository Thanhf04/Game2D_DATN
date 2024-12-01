using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField] private GameObject Shop;
    // [SerializeField] private GameObject Panel;
    [SerializeField] private Button BtnOpenShop;
    [SerializeField] private Button BtnCloseShop;
    public static bool isOpenShop = false;
    PanelManager panelManager;

    public void OpenShop()
    {
        //panelManager.Instance.OpenPanel(Shop);
        Shop.SetActive(true);
        //  Panel.SetActive(false);
    }
    public void CloseShop()
    {
        //panelManager.Instance.ClosePanel(Shop);
        isOpenShop = false;
        Shop.SetActive(false);
        //  Panel.SetActive(false);
    }
}
