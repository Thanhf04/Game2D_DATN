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
        uiCoin.AddCoins(Coin);
        Debug.Log("Item pick up");
        Debug.Log("Coin:" + Coin);
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