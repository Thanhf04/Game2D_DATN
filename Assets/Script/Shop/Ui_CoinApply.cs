using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ui_CoinApply : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI coinText;
    public Image coinImage;
    void Start()
    {
        UI_Coin.Instance.CoinChanged += UpdateCoinUI;
        UpdateCoinUI(UI_Coin.Instance.GetCurrentCoins());
    }

    private void UpdateCoinUI(int newCoinCount)
    {
        coinText.text = newCoinCount.ToString();
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        UI_Coin.Instance.CoinChanged -= UpdateCoinUI;
    }
}
