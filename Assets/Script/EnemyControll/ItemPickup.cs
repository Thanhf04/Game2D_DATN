using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int Coin;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Pickup();
        }
    }
    void Pickup()
    {
        UI_Coin.Instance.AddCoins(Coin); ;
        Debug.Log("Item pick up");
        Debug.Log("Coin:" + Coin);
        Destroy(gameObject);
    }
}
