using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField] private GameObject Shop;
    [SerializeField] private GameObject Panel;
    [SerializeField] private Button BtnOpenShop;
    [SerializeField] private Button BtnCloseShop;
    public static bool isOpenShop = false;

    private void OnMouseDown()
    {
        isOpenShop = true;
        Panel.SetActive(true);
    }
    public void OpenShop()
    {
        isOpenShop = true;
        Shop.SetActive(true);
        Panel.SetActive(false);
    }
    public void CloseShop()
    {
        isOpenShop = false;
        Shop.SetActive(false);
        Panel.SetActive(false);
    }
}
