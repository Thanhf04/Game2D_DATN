using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int Coin;
    private UI_Coin uiCoin;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        PlayerStats.Instance.gold += 100;
        Destroy(gameObject);
    }

    private void Start()
    {
        uiCoin = FindObjectOfType<UI_Coin>();
        if (uiCoin == null)
        {
            Debug.LogError("Không tìm thấy UI_Coin trong cảnh!");
        }
    }
}
