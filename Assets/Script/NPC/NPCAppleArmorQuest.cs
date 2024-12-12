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
    public Text completionText;
    public UI_Coin uiCoin;

    private string encouragementText = "Giỏi lắm chàng trai, bạn đã đi được tới đây, hãy tiếp tục cuộc hành trình nào!";
    private string appleQuestText = "Nhiệm vụ mới: Thu thập 3 quả táo để tiếp tục hành trình!";
    private string appleCompletionText = "Chúc mừng bạn đã thu thập đủ 3 quả táo, nhận thêm 20 vàng!";
    private string armorQuestText = "Nhiệm vụ mới: Thu thập 1 bộ giáp để tiếp tục hành trình!";
    private string armorCompletionText = "Chúc mừng bạn đã thu thập đủ giáp, nhận thêm 30 vàng!";

    private bool isPanelVisible = false;
    public static bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false;
    private bool hasShownEncouragement = false;

    private int appleCount = 0;
    private int armorCount = 0;

    private FirebaseQuestManager firebaseQuestManager;

    void Start()
    {
        firebaseQuestManager = FindObjectOfType<FirebaseQuestManager>();

        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        if (completionText != null)
        {
            completionText.gameObject.SetActive(false); // Hide completion text at the beginning
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

        // Load quest status from Firebase when the game starts
        LoadQuestStatusFromFirebase();
    }

    // Load quest status from Firebase
    private void LoadQuestStatusFromFirebase()
    {
        // Get the quest data from Firebase
        appleCount = firebaseQuestManager.GetAppleCount();
        armorCount = firebaseQuestManager.GetArmorCount();
        hasCompletedAppleQuest = firebaseQuestManager.GetHasCompletedAppleQuest();
        hasCompletedArmorQuest = firebaseQuestManager.GetHasCompletedArmorQuest();

        // Update UI if there is data from Firebase
        if (hasCompletedAppleQuest)
        {
            appleCountText.color = Color.yellow;
            appleCountText.text = "Số táo đã thu thập: " + appleCount + "/3";
        }

        if (hasCompletedArmorQuest)
        {
            armorCountText.color = Color.yellow;
            armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
        }
    }

    // Handle NPC interaction
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

    // Handle continue button click
    private void OnContinue()
    {
        // Load latest data from Firebase before continuing
        LoadQuestStatusFromFirebase();

        // Debug logs to check if data is updated correctly
        Debug.Log("Apple Count from Firebase: " + appleCount);
        Debug.Log("Armor Count from Firebase: " + armorCount);
        Debug.Log("Has Completed Apple Quest: " + hasCompletedAppleQuest);
        Debug.Log("Has Completed Armor Quest: " + hasCompletedArmorQuest);

        // Check quest status and update UI accordingly
        if (!hasCompletedAppleQuest)
        {
            questText.text = appleQuestText;
            appleCountText.gameObject.SetActive(true);
            appleCountText.text = "Số táo đã thu thập: " + appleCount + "/3";
        }
        else if (hasCompletedAppleQuest && !hasCompletedArmorQuest)
        {
            questText.text = armorQuestText;
            armorCountText.gameObject.SetActive(true);
            appleCountText.gameObject.SetActive(false);
            armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
        }
        else if (hasCompletedAppleQuest && hasCompletedArmorQuest)
        {
            questText.text = "Chúc mừng bạn đã hoàn thành tất cả nhiệm vụ!";
            appleCountText.gameObject.SetActive(false);
            armorCountText.gameObject.SetActive(false);
        }
    }

    // Handle confirm button click
    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;

            // Update UI after quest completion
            if (hasCompletedArmorQuest)
            {
                if (uiCoin != null)
                {
                    uiCoin.AddCoins(30); // Reward gold when completing armor quest
                }
                armorCountText.gameObject.SetActive(false);
                firebaseQuestManager.SaveQuestStatus(); // Save quest status to Firebase
            }
            else if (hasCompletedAppleQuest)
            {
                if (uiCoin != null)
                {
                    uiCoin.AddCoins(20); // Reward gold when completing apple quest
                }
                appleCountText.gameObject.SetActive(false);
                firebaseQuestManager.SaveQuestStatus(); // Save quest status to Firebase
            }
        }
    }

    // Handle collecting an apple
    public void CollectApple()
    {
        appleCount++;
        appleCountText.text = "Số táo đã thu thập: " + appleCount + "/3";

        if (appleCount >= 3 && !hasCompletedAppleQuest)
        {
            hasCompletedAppleQuest = true;
            questText.text = appleCompletionText;
            appleCountText.color = Color.yellow;

            // Save quest status to Firebase
            firebaseQuestManager.SetAppleCount(appleCount);
            firebaseQuestManager.SetHasCompletedAppleQuest(true);

            // Update UI
            ShowCompletionText("Báo cáo với Thợ rèn");

            if (uiCoin != null)
            {
                uiCoin.AddCoins(20); // Reward gold when completing the apple quest
            }
        }
    }

    // Handle collecting armor
    public void CollectArmor()
    {
        armorCount++;
        armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";

        if (armorCount >= 1 && !hasCompletedArmorQuest)
        {
            hasCompletedArmorQuest = true;
            questText.text = armorCompletionText;
            armorCountText.color = Color.yellow;
            firebaseQuestManager.SetArmorCount(armorCount);
            firebaseQuestManager.SetHasCompletedArmorQuest(true);
            ShowCompletionText("Báo cáo với Thợ rèn");

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
            completionText.gameObject.SetActive(true);
        }
    }

    private void HideCompletionText()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }

    // Update UI after quest data is loaded from Firebase
    public void UpdateUI()
    {
        // Update apple and armor count
        if (appleCountText != null)
        {
            appleCountText.text = "Số táo đã thu thập: " + appleCount + "/3";
            appleCountText.gameObject.SetActive(appleCount > 0);
        }

        if (armorCountText != null)
        {
            armorCountText.text = "Số giáp đã thu thập: " + armorCount + "/1";
            armorCountText.gameObject.SetActive(armorCount > 0);
        }

        // Update quest completion status
        if (questText != null)
        {
            if (hasCompletedAppleQuest)
            {
                questText.text = appleCompletionText;
            }
            else if (hasCompletedArmorQuest)
            {
                questText.text = armorCompletionText;
            }
            else
            {
                questText.text = "Chưa hoàn thành nhiệm vụ!";
            }
        }

        // Show completion text if a quest is completed
        if (completionText != null)
        {
            if (hasCompletedAppleQuest || hasCompletedArmorQuest)
            {
                completionText.gameObject.SetActive(true);
                completionText.text = "Chúc mừng bạn đã hoàn thành nhiệm vụ!";
            }
            else
            {
                completionText.gameObject.SetActive(false);
            }
        }
    }
}