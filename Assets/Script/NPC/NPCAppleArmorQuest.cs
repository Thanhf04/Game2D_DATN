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

    private bool isPanelVisible = false;
    private bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false;
    private bool hasShownEncouragement = false;

    private int appleCount = 0;
    private int armorCount = 0;

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
            }
        }
        else if (hasCompletedAppleQuest && !hasCompletedArmorQuest)
        {
            questText.text = armorQuestText;
            armorCountText.gameObject.SetActive(true);
            armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
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
            if (uiCoin != null)
            {
                uiCoin.AddCoins(30);
            }
        }
    }
}
