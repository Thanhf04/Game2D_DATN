using UnityEngine;
using UnityEngine.UI;

public class UI_Coin : MonoBehaviour
{
    [SerializeField]
    private int StartingCoins = 0; // Số coin bắt đầu

    [Header("UI References")]
    public GameObject panelNotification;
    public GameObject panelShop;
    public Button btn_Close;

    public delegate void CoinChangedDelegate(int newCoinCount);
    public CoinChangedDelegate CoinChanged;

    private void Awake()
    {
        // Gán sự kiện cho nút đóng
        if (btn_Close != null)
        {
            btn_Close.onClick.AddListener(ClosePanel);
        }
    }

    private void Start()
    {
        // AddCoins(100);
        // Debug.Log("Coins Initialized");
    }

    // public void AddCoins(int amount)
    // {
    //     PlayerStats.Instance.gold += amount;
    //     CoinChanged?.Invoke(PlayerStats.Instance.gold); // Gọi sự kiện khi thay đổi số coin
    //     PlayerStats.Instance.SaveStats();
    // }

    public bool SubTractCoins(int amount, Model_Shop.ItemType itemType)
    {
        if (PlayerStats.Instance.gold >= amount)
        {
            PlayerStats.Instance.gold -= amount;
            PlayerStats.Instance.SaveStats();
            CoinChanged?.Invoke(PlayerStats.Instance.gold);

            Debug.Log($"Buy Item Success | Coin: {PlayerStats.Instance.gold} | Item: {itemType}");
            return true;
        }
        else if (PlayerStats.Instance.gold <= 0)
        {
            Debug.Log("You don't have money");
            panelNotification.SetActive(true);
            panelShop.SetActive(false);
            return false;
        }
        else
        {
            Debug.Log("Buy item fail");
            panelNotification.SetActive(true);
            panelShop.SetActive(false);
            return false;
        }
    }

    public int GetCurrentCoins()
    {
        return PlayerStats.Instance.gold;
    }

    public void ClosePanel()
    {
        panelNotification.SetActive(false);
        panelShop.SetActive(true);
    }
}
