using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int Coin;
    private void OnMouseDown()
    {
        Pickup();
    }
    void Pickup()
    {
        UI_Coin.Instance.AddCoins(Coin); ;
        Debug.Log("Item pick up");
        Debug.Log("Coin:" + Coin);
        Destroy(gameObject);
    }
}
