using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ui_CoinApply : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI coinText;
    public Image coinImage;

    [Header("References")]
    [SerializeField]
    private UI_Coin uiCoin; // Tham chiếu đến UI_Coin

    private void Start()
    {
        if (uiCoin == null)
        {
            Debug.LogError("Ui_CoinApply: UI_Coin reference is missing!");
            return;
        }

        uiCoin.CoinChanged += UpdateCoinUI;
        UpdateCoinUI(uiCoin.GetCurrentCoins()); // Cập nhật giao diện lần đầu
    }

    private void UpdateCoinUI(int newCoinCount)
    {
        if (coinText != null)
        {
            coinText.text = newCoinCount.ToString();
        }
    }

    private void OnDestroy()
    {
        if (uiCoin != null)
        {
            uiCoin.CoinChanged -= UpdateCoinUI;
        }
    }
}
