using UnityEngine;

public class UI_Coin : MonoBehaviour
{
    private int StartingCoins = 0;
    private int curentCoins = 0;
    private UI_Shop ui_Shop;
    public delegate void CoinChangedDelegate(int newCoinCount);
    public CoinChangedDelegate CoinChanged;

    public static UI_Coin _instance;
    public static UI_Coin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UI_Coin>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("Text_Coin");
                    _instance = obj.AddComponent<UI_Coin>();
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        curentCoins = StartingCoins;
    }
    private void Start()
    {
        AddCoins(100);
        Debug.Log("Add coins");
    }
    public void AddCoins(int amount)
    {
        curentCoins = curentCoins + amount;
        CoinChanged?.Invoke(curentCoins);

    }
    public bool SubTractCoins(int mount, Model_Shop.ItemType itemType)
    {
        if (curentCoins >= mount)
        {
            curentCoins -= mount;
            CoinChanged?.Invoke(curentCoins);
            Debug.Log("Buy Item Success");
            Debug.Log("Coin: " + curentCoins);
            Debug.Log("Bought item: " + itemType);
            return true;
        }
        if (curentCoins <= 0)
        {
            Debug.Log("You don't have money");
            return false;
        }
        else
        {
            Debug.Log("Buy Item Fail");
            return false;
        }
    }
    public int GetCurrentCoins()
    {
        return curentCoins;
    }

}
