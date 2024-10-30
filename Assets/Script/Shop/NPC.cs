using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    private Dichuyennv1 dichuyennv1;
    [SerializeField] private GameObject Shop;
    [SerializeField] private GameObject Panel;
    [SerializeField] private Button BtnOpenShop;
    [SerializeField] private Button BtnCloseShop;

    private void OnMouseDown()
    {
        Panel.SetActive(true);
    }
    public void OpenShop()
    {
        Shop.SetActive(true);
        Panel.SetActive(false);
    }
    public void CloseShop()
    {
        Panel.SetActive(false);
    }
}
