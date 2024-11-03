using UnityEngine;

public class ShopIsTrigger : MonoBehaviour
{
    private UI_Shop uI_Shop;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        uI_Shop.Show();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        uI_Shop.Hide();
    }
}
