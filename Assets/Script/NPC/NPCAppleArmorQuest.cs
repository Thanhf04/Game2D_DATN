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
    public Text completionText; // Text hiển thị thông báo hoàn thành
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

        if (completionText != null)
        {
            completionText.gameObject.SetActive(false); // Ẩn Text hoàn thành khi bắt đầu
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
            appleCountText.gameObject.SetActive(false);
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

            // Ẩn thông báo hoàn thành khi nhiệm vụ đã được báo cáo
            HideCompletionText();
        }
        else if (hasCompletedAppleQuest)
        {
            if (uiCoin != null)
            {
                uiCoin.AddCoins(20);
            }
            appleCountText.gameObject.SetActive(false);

            // Ẩn thông báo hoàn thành khi nhiệm vụ đã được báo cáo
            HideCompletionText();
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

            ShowCompletionText("Báo cáo với Thợ rèn"); // Gọi hàm hiển thị thông báo

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

            ShowCompletionText("Báo cáo với Thợ rèn"); // Gọi hàm hiển thị thông báo

            if (uiCoin != null)
            {
                uiCoin.AddCoins(30);
            }
        }
    }

    private void ShowCompletionText(string message)
    {
        if (completionText != null)
        {
            completionText.text = message;
            completionText.gameObject.SetActive(true); // Hiển thị Text

            // Tắt Text sau 2 giây
            // Invoke(nameof(HideCompletionText), 2f);
        }
    }

    private void HideCompletionText()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }
}
