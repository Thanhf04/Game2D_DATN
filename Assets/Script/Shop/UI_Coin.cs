using UnityEngine;
using UnityEngine.UI;

public class UI_Coin : MonoBehaviour
{
    [SerializeField] private int StartingCoins = 0; // Số coin bắt đầu
    private int currentCoins = 0;

    [Header("UI References")]
    public GameObject panelNotification;
    public GameObject panelShop;
    public Button btn_Close;

    public delegate void CoinChangedDelegate(int newCoinCount);
    public CoinChangedDelegate CoinChanged;

    private void Awake()
    {
        currentCoins = StartingCoins;

        // Gán sự kiện cho nút đóng
        if (btn_Close != null)
        {
            btn_Close.onClick.AddListener(ClosePanel);
        }
    }

    private void Start()
    {
        AddCoins(100);
        Debug.Log("Coins Initialized");
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        CoinChanged?.Invoke(currentCoins); // Gọi sự kiện khi thay đổi số coin
    }

    public bool SubTractCoins(int amount, Model_Shop.ItemType itemType)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            CoinChanged?.Invoke(currentCoins);

            Debug.Log($"Buy Item Success | Coin: {currentCoins} | Item: {itemType}");
            return true;
        }

        // Trường hợp không đủ tiền
        Debug.Log(currentCoins <= 0 ? "You don't have money" : "Buy Item Fail");
        panelNotification?.SetActive(true);
        panelShop?.SetActive(false);

        return false;
    }

    public int GetCurrentCoins()
    {
        return currentCoins;
    }

    public void ClosePanel()
    {
        panelNotification?.SetActive(false);
        panelShop?.SetActive(true);
    }
}
