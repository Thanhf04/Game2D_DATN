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
    public Text appleCountText;
    public Text armorCountText;
    public UI_Coin uiCoin; 

    private string initialQuestText = "Xin chào chàng hiệp sĩ, bạn là người được chọn để giải cứu vùng đất này.";
    private string secondQuestText = "Nhiệm vụ đầu tiên của bạn là tìm lại thanh kiếm đã mất.";
    private string congratulationText = "Giỏi lắm chàng trai, bây giờ bạn có thể dùng chuột trái để tấn công!";
    private string thirdQuestText = "Nhiệm vụ tiếp theo của bạn là giết 5 con quái.";
    private string rewardCompletionText = "Chúc mừng bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 50 vàng!";
    private string rewardTbaoShopText = "Giỏi lắm chàng trai mở thêm chức năng shop, bạn có thể mua vật phẩm để hồi năng lượng và máu!";
    private string appleQuestText = "Nhiệm vụ mới: Thu thập 1 quả táo để tiếp tục hành trình!";
    private string appleCompletionText = "Chúc mừng bạn đã thu thập đủ 1 quả táo, nhận thêm 20 vàng!";
    private string rewardTbaoSkillText = "Giỏi lắm chàng trai bạn đã hoàn thành nhiệm vụ, bây giờ bạn có thể sài skill cho riêng bản thân mình";
    private string armorQuestText = "Nhiệm vụ mới: Thu thập 1 bộ giáp để tiếp tục hành trình!";
    private string armorCompletionText = "Chúc mừng bạn đã thu thập đủ giáp, nhận thêm 30 vàng!";

    private bool isPanelVisible = false;
    private bool isQuestStarted = false;

    private bool hasShownCongratulation = false;
    private bool hasReceivedReward = false; 
    private bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false; 

    private int swordCount = 0;
    private int monsterKillCount = 0;
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

        if (hasCompletedArmorQuest)
        {
            questText.text = armorCompletionText; 
        }
        else if (hasCompletedAppleQuest)
        {
            questText.text = appleCompletionText; 
        }
        else if (swordCount == 1 && !hasShownCongratulation)
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

            // Hiển thị số kiếm khi nhận nhiệm vụ nhặt kiếm
            swordCountText.gameObject.SetActive(true);
            swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1";
        }
        else if (swordCount == 1 && monsterKillCount < 5)
        {
            questText.text = thirdQuestText;

            // Ẩn số kiếm khi chuyển sang nhiệm vụ giết quái
            swordCountText.gameObject.SetActive(false);
            monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/5";
        }
        else if (swordCount == 1 && monsterKillCount >= 5 && !hasReceivedReward)
        {
            questText.text = rewardCompletionText;
            if (uiCoin != null)
            {
                uiCoin.AddCoins(50);
            }

            hasReceivedReward = true;

            swordCountText.gameObject.SetActive(false);
            monsterCountText.gameObject.SetActive(false);

            // Hiển thị thông báo về mở chức năng shop
            questText.text = rewardTbaoShopText;
        }
        else if (hasReceivedReward && !hasCompletedAppleQuest)
        {
            // Sau khi nhận reward cho shop, tiến tới nhiệm vụ nhặt táo
            questText.text = appleQuestText;
            appleCountText.gameObject.SetActive(true);
            appleCountText.text = "Số táo đã thu thập: " + appleCount + "/1";
        }
        else if (appleCount >= 1 && !hasCompletedAppleQuest)
        {
            questText.text = appleCompletionText;
            if (uiCoin != null && !hasCompletedAppleQuest)
            {
                uiCoin.AddCoins(20);
            }
            hasCompletedAppleQuest = true;
            appleCountText.gameObject.SetActive(false);

            // Hiển thị thông báo về việc có thể sử dụng skill
            questText.text = rewardTbaoSkillText; // Hiển thị thông báo skill
        }
        else if (hasCompletedAppleQuest && !hasCompletedArmorQuest && armorCount == 0)
        {
            // Sau khi hoàn thành nhiệm vụ táo, tiến tới nhiệm vụ giáp
            questText.text = armorQuestText;
            armorCountText.gameObject.SetActive(true);
            armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
        }
        else if (armorCount >= 1 && !hasCompletedArmorQuest)
        {
            questText.text = armorCompletionText; 
            armorCountText.gameObject.SetActive(false);
            hasCompletedArmorQuest = true;
            continueButton.gameObject.SetActive(true); 
        }
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
            return; 
        }
        if (hasCompletedAppleQuest)
        {
            if (uiCoin != null && !hasCompletedAppleQuest)
            {
                uiCoin.AddCoins(20); 
            }
            appleCountText.gameObject.SetActive(false); 
            return;
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
        monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/5";
    }

    public void CollectApple()
{
    appleCount++;
    appleCountText.text = "Số táo đã thu thập: " + appleCount + "/1";

    if (appleCount >= 1 && !hasCompletedAppleQuest)
    {
        hasCompletedAppleQuest = true;
        questText.text = appleCompletionText;
        if (uiCoin != null && !hasCompletedAppleQuest) 
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
        armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
        if (uiCoin != null && !hasCompletedArmorQuest)
        {
            uiCoin.AddCoins(30);
        }
    }
}
}