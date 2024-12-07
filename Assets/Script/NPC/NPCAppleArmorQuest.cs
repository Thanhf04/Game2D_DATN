using UnityEngine;
using UnityEngine.UI;

public class NPCAppleArmorQuest : MonoBehaviour
{
    public GameObject questPanel;
    public Text questText;
    public Button continueButton;
    public Button confirmButton;
    public Text appleCountText;
    public Text armorCountText;
    public UI_Coin uiCoin;

    private string encouragementText = "Giỏi lắm chàng trai, bạn đã đi được tới đây, hãy tiếp tục cuộc hành trình nào!";
    private string appleQuestText = "Nhiệm vụ mới: Thu thập 3 quả táo để tiếp tục hành trình!";
    private string appleCompletionText = "Chúc mừng bạn đã thu thập đủ 3 quả táo, nhận thêm 20 vàng!";
    private string armorQuestText = "Nhiệm vụ mới: Thu thập 1 bộ giáp để tiếp tục hành trình!";
    private string armorCompletionText = "Chúc mừng bạn đã thu thập đủ giáp, nhận thêm 30 vàng!";
    private string textTiepTuc = "Bây giờ còn hãy tìm đường tới Thành phố bỏ hoang và gặp người bí ẩn.";

    private bool isPanelVisible = false;
    private bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false;
    private bool hasCardQuest = false;
    private bool hasShownEncouragement = false;

    private int appleCount = 0;
    private int armorCount = 0;
    private int currentCard = 0;

    public Text currentCardText;

    #region Nhiệm vụ lật thẻ
    private string CardQuestText = "Nhiệm vụ lần này của bạn là lật đúng 2 lần thẻ giống nhau";
    private string CardQuestText1 = "Sau đó quay về đây cậu sẽ nhận phần quà bất ngờ";
    private string congratulationCardQuestText = "Bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 70 vàng";

    #endregion

    void Start()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinue);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }

        if (appleCountText != null)
        {
            appleCountText.text = "";
            appleCountText.gameObject.SetActive(false);
        }

        if (armorCountText != null)
        {
            armorCountText.text = "";
            armorCountText.gameObject.SetActive(false);
        }
        if (currentCardText != null)
        {
            currentCardText.text = "";
            currentCardText.gameObject.SetActive(false);
        }

        uiCoin = FindObjectOfType<UI_Coin>();
    }

    void OnMouseDown()
    {
        if (questPanel != null && !isPanelVisible)
        {
            questPanel.SetActive(true);

            if (!hasShownEncouragement && !hasCompletedAppleQuest)
            {
                questText.text = encouragementText;
                hasShownEncouragement = true;
            }
            else if (hasCompletedArmorQuest)
            {
                questText.text = armorCompletionText;
                questText.text = textTiepTuc;
            }
            else if (hasCompletedAppleQuest)
            {
                questText.text = appleCompletionText;
            }
            else
            {
                questText.text = appleQuestText;
                appleCountText.gameObject.SetActive(true);
                appleCountText.text = "Số táo đã thu thập: " + appleCount + "/3";
            }

            isPanelVisible = true;
        }
    }

    private void OnContinue()
    {
        if (!hasCompletedAppleQuest)
        {
            if (hasShownEncouragement)
            {
                questText.text = appleQuestText;
                appleCountText.gameObject.SetActive(true);
                appleCountText.text = "Số táo đã thu thập: " + appleCount + "/3";

                appleCountText.color = Color.white;
            }
        }
        else if (hasCompletedAppleQuest && !hasCompletedArmorQuest)
        {
            questText.text = armorQuestText;
            armorCountText.gameObject.SetActive(true);
            armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";

            armorCountText.color = Color.white;
        }
    }

    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;

            if (hasCompletedArmorQuest)
            {
                if (uiCoin != null)
                {
                    uiCoin.AddCoins(30);
                }
                armorCountText.gameObject.SetActive(false);
            }
            else if (hasCompletedAppleQuest)
            {
                if (uiCoin != null)
                {
                    uiCoin.AddCoins(20);
                }
                appleCountText.gameObject.SetActive(false);
            }
        }
    }

    public void CollectApple()
    {
        appleCount++;
        appleCountText.text = "Số táo đã thu thập: " + appleCount + "/3";

        if (appleCount >= 3 && !hasCompletedAppleQuest)
        {
            hasCompletedAppleQuest = true;
            questText.text = appleCompletionText;
            appleCountText.color = Color.yellow;
            if (uiCoin != null)
            {
                uiCoin.AddCoins(20);
            }
        }
    }

    public void CollectArmor()
    {
        armorCount++;
        armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";

        if (armorCount >= 1 && !hasCompletedArmorQuest)
        {
            hasCompletedArmorQuest = true;
            questText.text = armorCompletionText;
            armorCountText.color = Color.yellow;
            if (uiCoin != null)
            {
                uiCoin.AddCoins(30);
            }
        }
    }
     public void CurrentCard()
    {
        currentCard++;
        currentCardText.text = "Số thẻ đã lật thành công: " + currentCard + "/2";

          if (currentCard == 0 && !hasCardQuest)
            {
                questText.text = CardQuestText;
                hasCardQuest = true;

                currentCardText.gameObject.SetActive(true);
                currentCardText.text = "Số thẻ đã lật thành công: " + currentCard + "/2";
            }
            else if (currentCard == 2)
            {
                questText.text = congratulationCardQuestText;
                currentCardText.gameObject.SetActive(false);
                uiCoin.AddCoins(70);
            }
    }
}
