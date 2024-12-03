using UnityEngine;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour
{
    public GameObject questPanel;
    public Text questText;
    public Button continueButton;
    public Button confirmButton;
    public Text swordCountText;
    public Text monsterCountText;
    public Text appleCountText; // Text để hiển thị số lượng táo
    public UI_Coin uiCoin; // Reference to UI_Coin script to add coins

    private string initialQuestText = "Xin chào chàng hiệp sĩ, bạn là người được chọn để giải cứu vùng đất này.";
    private string secondQuestText = "Nhiệm vụ đầu tiên của bạn là tìm lại thanh kiếm đã mất.";
    private string congratulationText = "Chúc mừng bạn đã hoàn thành nhiệm vụ, bây giờ bạn có thể dùng chuột trái để tấn công!";
    private string thirdQuestText = "Nhiệm vụ tiếp theo của bạn là giết 1 con quái.";
    private string rewardCompletionText = "Xin chúc mừng bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 50 vàng!";
    private string appleQuestText = "Nhiệm vụ mới: Thu thập 10 quả táo để tiếp tục hành trình!";
    private string appleCompletionText = "Chúc mừng bạn đã thu thập đủ 10 quả táo, nhận thêm 20 vàng!";

    private bool isPanelVisible = false;
    private bool isQuestStarted = false;

    private bool hasShownCongratulation = false;
    private bool hasReceivedReward = false; 
    private bool hasCompletedAppleQuest = false;

    private int swordCount = 0;
    private int monsterKillCount = 0;
    private int appleCount = 0; // Biến để lưu số táo đã thu thập

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

        if (swordCountText != null)
        {
            swordCountText.text = ""; 
        }
        if (monsterCountText != null)
        {
            monsterCountText.text = ""; 
        }
        if (appleCountText != null)
        {
            appleCountText.text = ""; 
            appleCountText.gameObject.SetActive(false); // Ẩn text số táo
        }
    }

    void OnMouseDown()
    {
        if (questPanel != null && !isPanelVisible)
        {
            questPanel.SetActive(true);

            if (swordCount == 1 && !hasShownCongratulation) 
            {
                questText.text = congratulationText;
                hasShownCongratulation = true; 
                isQuestStarted = false; 
            }
            else if (swordCount == 0)
            {
                questText.text = initialQuestText; 
                isPanelVisible = true;
            }
        }
    }

    private void OnContinue()
    {
        if (questPanel != null)
        {
            if (!isQuestStarted && swordCount == 0)
            {
                questText.text = secondQuestText;
                isQuestStarted = true;
            }
            else if (swordCount == 1 && monsterKillCount < 1)
            {
                questText.text = thirdQuestText;
                swordCountText.text = "";
                monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/1";
            }
            else if (swordCount == 1 && monsterKillCount >= 1 && !hasCompletedAppleQuest && !hasReceivedReward)
            {
                questText.text = rewardCompletionText; // Thông báo hoàn thành nhiệm vụ giết quái
                if (uiCoin != null)
                {
                    uiCoin.AddCoins(50); // Thưởng 50 vàng
                    Debug.Log("Nhận phần thưởng 50 vàng!");
                }

                hasReceivedReward = true; // Đánh dấu nhiệm vụ giết quái đã hoàn thành
            }
            else if (hasReceivedReward && !hasCompletedAppleQuest)
            {
                questText.text = appleQuestText; // Bắt đầu nhiệm vụ táo
                appleCountText.gameObject.SetActive(true); // Hiển thị text số táo
                appleCountText.text = "Số táo đã thu thập: " + appleCount + "/10";
            }
            else if (appleCount >= 10 && !hasCompletedAppleQuest)
            {
                questText.text = appleCompletionText;
                if (uiCoin != null)
                {
                    uiCoin.AddCoins(20); // Thưởng 20 vàng
                    Debug.Log("Nhận phần thưởng 20 vàng!");
                }

                hasCompletedAppleQuest = true; // Đánh dấu nhiệm vụ táo hoàn thành
                appleCountText.gameObject.SetActive(false); // Ẩn text số táo
            }
        }
    }

    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;

            if (swordCount == 1 && monsterKillCount < 1)
            {
                swordCountText.text = "";
                monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/1";
            }
            else if (swordCount == 1 && monsterKillCount >= 1 && !hasCompletedAppleQuest)
            {
                monsterCountText.text = "";
                appleCountText.text = "Số táo đã thu thập: " + appleCount + "/10";
            }
            else if (hasCompletedAppleQuest)
            {
                appleCountText.text = "";
            }
        }
    }

    public void FindSword()
    {
        swordCount = 1; 
        Debug.Log("Số kiếm đã tìm: " + swordCount);
        swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1"; 
    }

    public void KillMonster()
    {
        monsterKillCount++; 
        Debug.Log("Giết 1 con quái thành công!");
        monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/1";
    }

    public void CollectApple()
    {
        appleCount++;
        Debug.Log("Thu thập táo: " + appleCount + "/10");
        appleCountText.text = "Số táo đã thu thập: " + appleCount + "/10";
    }
}
