using UnityEngine;

public class ShopOpen : MonoBehaviour
{
    [SerializeField] private GameObject Shop;
    public static bool isOpenShop = false;

    public void OpenShop()
    {
        PanelManager.Instance.OpenPanel(Shop);
        isOpenShop = true;
    }
    public void CloseShop()
    {
        PanelManager.Instance.ClosePanel(Shop);
        isOpenShop = false;

    }
}
