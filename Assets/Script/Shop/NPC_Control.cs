using UnityEngine;

public class NPC_Control : MonoBehaviour
{
    private NPC nPC;  // Tham chiếu đến ShopController

    private void Start()
    {
        // Tìm đối tượng ShopController trong scene
        nPC = FindObjectOfType<NPC>();
    }

    private void OnMouseDown()
    {
        // Mở cửa hàng thông qua ShopController
        if (nPC != null)
        {
            nPC.OpenShop();
        }
        else
        {
            Debug.LogError("NPC không được tìm thấy!");
        }
    }
}
