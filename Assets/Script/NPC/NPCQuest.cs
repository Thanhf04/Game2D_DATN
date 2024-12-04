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
    public Text armorCountText; // Text để hiển thị số lượng giáp
    public UI_Coin uiCoin; // Tham chiếu đến UI_Coin để thêm vàng

    private string initialQuestText = "Xin chào chàng hiệp sĩ, bạn là người được chọn để giải cứu vùng đất này.";
    private string secondQuestText = "Nhiệm vụ đầu tiên của bạn là tìm lại thanh kiếm đã mất.";
    private string congratulationText = "Giỏi lằm chàng trai, bây giờ bạn có thể dùng chuột trái để tấn công!";
    private string thirdQuestText = "Nhiệm vụ tiếp theo của bạn là giết 1 con quái.";
    private string rewardCompletionText = "Chúc mừng bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 50 vàng!";
    private string appleQuestText = "Nhiệm vụ mới: Thu thập 1 quả táo để tiếp tục hành trình!";
    private string appleCompletionText = "Chúc mừng bạn đã thu thập đủ 1 quả táo, nhận thêm 20 vàng!";
    private string armorQuestText = "Nhiệm vụ mới: Thu thập 1 bộ giáp để tiếp tục hành trình!";
    private string armorCompletionText = "Chúc mừng bạn đã thu thập đủ giáp, nhận thêm 30 vàng!";

    private bool isPanelVisible = false;
    private bool isQuestStarted = false;

    private bool hasShownCongratulation = false;
    private bool hasReceivedReward = false; 
    private bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false; // Kiểm tra nhiệm vụ giáp

    private int swordCount = 0;
    private int monsterKillCount = 0;
    private int appleCount = 0; // Biến để lưu số táo đã thu thập
    private int armorCount = 0; // Biến để lưu số giáp đã thu thập

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
        if (armorCountText != null)
        {
            armorCountText.text = ""; 
            armorCountText.gameObject.SetActive(false); // Ẩn text số giáp
        }
        
        uiCoin = FindObjectOfType<UI_Coin>();
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
            else if (hasCompletedAppleQuest) 
            {
                questText.text = appleCompletionText; 
            }
            else if (hasCompletedArmorQuest) 
            {
                questText.text = armorCompletionText; // Hiển thị nhiệm vụ giáp hoàn thành
            }
        }
    }

   private void OnContinue()
{
    if (questPanel != null)
    {
        // Kiểm tra nhiệm vụ đầu tiên (tìm kiếm thanh kiếm)
        if (!isQuestStarted && swordCount == 0)
        {
            questText.text = secondQuestText;
            isQuestStarted = true;
        }
        // Kiểm tra nhiệm vụ giết quái
        else if (swordCount == 1 && monsterKillCount < 1)
        {
            questText.text = thirdQuestText;
            swordCountText.text = "";
            monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/1";
        }
        // Kiểm tra nhiệm vụ giết quái đã hoàn thành và trao thưởng
        else if (swordCount == 1 && monsterKillCount >= 1 && !hasCompletedAppleQuest && !hasReceivedReward)
        {
            questText.text = rewardCompletionText;
            if (uiCoin != null)
            {
                uiCoin.AddCoins(50);
            }

            hasReceivedReward = true;

            swordCountText.gameObject.SetActive(false);
            monsterCountText.gameObject.SetActive(false);
        }
        // Nếu đã nhận thưởng cho nhiệm vụ giết quái, bắt đầu nhiệm vụ táo
        else if (hasReceivedReward && !hasCompletedAppleQuest)
        {
            questText.text = appleQuestText;
            appleCountText.gameObject.SetActive(true);
            appleCountText.text = "Số táo đã thu thập: " + appleCount + "/1";
        }
        // Nếu đã hoàn thành nhiệm vụ táo, hiển thị thông báo chúc mừng
        else if (appleCount >= 1 && !hasCompletedAppleQuest)
        {
            questText.text = appleCompletionText;
            if (uiCoin != null && !hasCompletedAppleQuest) // Cộng vàng khi hoàn thành táo và chưa cộng
            {
                uiCoin.AddCoins(20);
            }
            hasCompletedAppleQuest = true;
            appleCountText.gameObject.SetActive(false);
        }
        // Nếu đã hoàn thành nhiệm vụ táo, bắt đầu nhiệm vụ giáp
        else if (hasCompletedAppleQuest && !hasCompletedArmorQuest && armorCount == 0)
        {
            questText.text = armorQuestText;
            armorCountText.gameObject.SetActive(true);
            armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
        }
        // Nếu đã hoàn thành nhiệm vụ giáp, hiển thị thông báo chúc mừng và vàng
        else if (armorCount >= 1 && !hasCompletedArmorQuest)
        {
            questText.text = armorCompletionText; // Thông báo giáp đã thu thập đủ
            armorCountText.gameObject.SetActive(false); // Ẩn text giáp
            hasCompletedArmorQuest = true;

            // Chờ người chơi nhấn tiếp tục để nhận vàng
            continueButton.gameObject.SetActive(true); // Đảm bảo button continue hiển thị
        }
    }
}
    private void OnConfirm()
{
    if (questPanel != null)
    {
        questPanel.SetActive(false);
        isPanelVisible = false;

        // Ẩn text sau khi nhiệm vụ hoàn thành
        if (hasCompletedAppleQuest && !hasCompletedArmorQuest)
        {
            appleCountText.gameObject.SetActive(false);  // Chỉ ẩn khi đã hoàn thành nhiệm vụ táo
            armorCountText.gameObject.SetActive(true);   // Hiển thị nhiệm vụ giáp
        }
        else if (hasCompletedArmorQuest)
        {
            armorCountText.gameObject.SetActive(false); // Chỉ ẩn khi đã hoàn thành nhiệm vụ giáp
            questText.text = armorCompletionText; // Hiển thị thông báo hoàn thành nhiệm vụ giáp
            if (uiCoin != null && !hasCompletedArmorQuest) // Cộng vàng khi nhiệm vụ giáp hoàn thành
            {
                uiCoin.AddCoins(30); // Cộng vàng cho nhiệm vụ giáp
            }
        }
    }
}

    public void FindSword()
    {
        swordCount = 1; 
        swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1"; 
    }

    public void KillMonster()
    {
        monsterKillCount++; 
        monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/1";
    }

    public void CollectApple()
{
    appleCount++;
    appleCountText.text = "Số táo đã thu thập: " + appleCount + "/1";

    if (appleCount >= 1 && !hasCompletedAppleQuest)
    {
        hasCompletedAppleQuest = true;
        questText.text = appleCompletionText;
        if (uiCoin != null && !hasCompletedAppleQuest) // Chỉ cộng vàng khi hoàn thành và chưa cộng
        {
            uiCoin.AddCoins(20);
        }
    }
    // Không ẩn text ngay lập tức, để nó hiển thị lâu hơn
}

public void CollectArmor()
{
    armorCount++;
    armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";

    if (armorCount >= 1 && !hasCompletedArmorQuest)
    {
        hasCompletedArmorQuest = true;
        questText.text = armorCompletionText; // Hiển thị thông báo hoàn thành nhiệm vụ giáp
        armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
        
        // Thêm vàng nếu chưa nhận
        if (uiCoin != null && !hasCompletedArmorQuest)
        {
            uiCoin.AddCoins(30); // Cộng vàng cho nhiệm vụ giáp
        }
    }
}
}
