using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCAppleArmorQuest : MonoBehaviour
{
    public GameObject questPanel;
    public Text questText;
    public Button continueButton;
    public Button confirmButton;
    public Text appleCountText;
    public Text armorCountText;
    public UI_Coin uiCoin;
    public Text currentCardText;

    private string encouragementText = "Giỏi lắm chàng trai, bạn đã đi được tới đây, hãy tiếp tục cuộc hành trình nào!";
    private string appleQuestText = "Nhiệm vụ mới: Thu thập 3 quả táo để tiếp tục hành trình!";
    private string appleCompletionText = "Chúc mừng bạn đã thu thập đủ 3 quả táo, nhận thêm 20 vàng!";
    private string armorQuestText = "Nhiệm vụ mới: Thu thập 1 bộ giáp để tiếp tục hành trình!";
    private string armorCompletionText = "Chúc mừng bạn đã thu thập đủ giáp, nhận thêm 30 vàng!";
    private string textTiepTuc = "Bây giờ còn hãy tìm đường tới Thành phố bỏ hoang và gặp người bí ẩn.";

    private bool isPanelVisible = false;
    private bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false;

    private int appleCount = 0;
    private int armorCount = 0;
    private int currentCard = 0;  // Lưu số thẻ đã lật thành công

    private NPCAppleArmorQuestFirebase firebaseScript;

    void Start()
    {
        firebaseScript = FindObjectOfType<NPCAppleArmorQuestFirebase>(); // Kết nối với NPCAppleArmorQuestFirebase script

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
        LoadQuestStatus();  // Tải trạng thái nhiệm vụ khi bắt đầu
    }

    void OnMouseDown()
    {
        if (questPanel != null && !isPanelVisible)
        {
            questPanel.SetActive(true);

            if (!hasCompletedAppleQuest)
            {
                questText.text = appleQuestText;
                appleCountText.gameObject.SetActive(true);
                appleCountText.text = $"Số táo đã thu thập: {appleCount}/3";
            }
            else if (!hasCompletedArmorQuest)
            {
                questText.text = armorQuestText;
                armorCountText.gameObject.SetActive(true);
                armorCountText.text = $"Số giáp đã thu thập: {armorCount}/1";
            }
            else
            {
                questText.text = armorCompletionText + "\n" + textTiepTuc;
            }

            isPanelVisible = true;
        }
    }

    private void OnContinue()
    {
        if (!hasCompletedAppleQuest)
        {
            questText.text = appleQuestText;
            appleCountText.gameObject.SetActive(true);
            appleCountText.text = $"Số táo đã thu thập: {appleCount}/3";
        }
        else if (!hasCompletedArmorQuest)
        {
            questText.text = armorQuestText;
            armorCountText.gameObject.SetActive(true);
            armorCountText.text = $"Số giáp đã thu thập: {armorCount}/1";
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
                uiCoin.AddCoins(30);
                armorCountText.gameObject.SetActive(false);
            }
            else if (hasCompletedAppleQuest)
            {
                uiCoin.AddCoins(20);
                appleCountText.gameObject.SetActive(false);
            }

            // Save quest status
            SaveQuestStatus();  // Lưu trạng thái nhiệm vụ khi hoàn thành
        }
    }

    public void CollectApple()
    {
        appleCount++;
        appleCountText.text = $"Số táo đã thu thập: {appleCount}/3";

        if (appleCount >= 3 && !hasCompletedAppleQuest)
        {
            hasCompletedAppleQuest = true;
            questText.text = appleCompletionText;
            appleCountText.color = Color.yellow;

            uiCoin.AddCoins(20);
            SaveQuestStatus(); // Lưu trạng thái sau khi hoàn thành nhiệm vụ
        }

        // Cập nhật trạng thái thẻ sau khi thu thập táo
        UpdateCurrentCardStatus();
    }

    public void CollectArmor()
    {
        armorCount++;
        armorCountText.text = $"Số giáp đã thu thập: {armorCount}/1";

        if (armorCount >= 1 && !hasCompletedArmorQuest)
        {
            hasCompletedArmorQuest = true;
            questText.text = armorCompletionText;
            armorCountText.color = Color.yellow;

            uiCoin.AddCoins(30);
            SaveQuestStatus(); // Lưu trạng thái sau khi hoàn thành nhiệm vụ
        }

        // Cập nhật trạng thái thẻ sau khi thu thập giáp
        UpdateCurrentCardStatus();
    }

    public void SaveQuestStatus()
    {
        var questData = new Dictionary<string, object>
        {
            { "hasCompletedAppleQuest", hasCompletedAppleQuest },
            { "appleCount", appleCount },
            { "hasCompletedArmorQuest", hasCompletedArmorQuest },
            { "armorCount", armorCount },
            { "currentCard", currentCard }
        };

        // Đảm bảo bạn đang lưu vào đúng đường dẫn Firebase
        firebaseScript.SaveQuestStatus(questData);
    }

    public void LoadQuestStatus()
    {
        // Tải trạng thái từ Firebase
        firebaseScript.LoadQuestStatus();
    }

    // Thêm phương thức để cập nhật trạng thái thẻ
    public void UpdateCurrentCardStatus()
    {
        // Cập nhật số thẻ đã lật (hoặc các logic khác nếu cần)
        currentCardText.text = $"Số thẻ đã lật: {currentCard}/1";  // Hoặc thay đổi theo logic của bạn

        // Lưu trạng thái thẻ vào Firebase nếu cần thiết
        SaveQuestStatus();  // Cập nhật trạng thái vào Firebase
    }
}
