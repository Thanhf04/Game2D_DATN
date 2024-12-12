using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class NPCAppleArmorQuest : MonoBehaviour
{
    public GameObject questPanel;
    public Text questText;
    public Button continueButton;
    public Button confirmButton;
    public Text appleCountText;
    public Text armorCountText;
    public Text completionText; // Text hiển thị thông báo hoàn thành
    public Text knightCountText; // Text hiển thị số Hiệp sĩ đã gặp
    public UI_Coin uiCoin;

    private string encouragementText = "Giỏi lắm chàng trai, bạn đã đi được tới đây, hãy tiếp tục cuộc hành trình nào!";
    private string appleQuestText = "Nhiệm vụ mới: Thu thập 3 quả táo để tiếp tục hành trình!";
    private string appleCompletionText = "Chúc mừng bạn đã thu thập đủ 3 quả táo, nhận thêm 20 vàng!";
    private string nextTaskHintText = "Hãy tìm Nữ hiệp sĩ để nhận nhiệm vụ tiếp theo.";
    private string armorQuestText = "Nhiệm vụ mới: Thu thập 1 bộ giáp để tiếp tục hành trình!";
    private string armorCompletionText = "Chúc mừng bạn đã thu thập đủ giáp, nhận thêm 30 vàng!";

    public Image imageLock1;

    private bool isPanelVisible = false;
    public static bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false;
    private bool hasShownEncouragement = false;
    private bool hasShownNextTaskHint = false;

    private int appleCount = 0;
    private int armorCount = 0;
    private int knightCount; // Biến đếm số Hiệp sĩ đã gặp

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

        if (knightCountText != null)
        {
            knightCountText.text = "";
            knightCountText.gameObject.SetActive(false); // Ẩn Text khi bắt đầu
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
            else if (hasCompletedArmorQuest && !hasShownNextTaskHint)
            {
                questText.text = armorCompletionText;
            }
            else if (hasCompletedAppleQuest && !hasCompletedArmorQuest)
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
        else if (hasCompletedArmorQuest && !hasShownNextTaskHint)
        {
            questText.text = nextTaskHintText;
        knightCountText.gameObject.SetActive(true); // Hiển thị Text số Hiệp sĩ
        StartCoroutine(HideNextTaskHintAfterDelay()); // Bắt đầu coroutine ẩn text
        hasShownNextTaskHint = true;
        }
    }

    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;

            if (hasCompletedArmorQuest && hasShownNextTaskHint)
            {
                armorCountText.text = nextTaskHintText;
                armorCountText.color = Color.green;

                // Ẩn thông báo hoàn thành khi nhiệm vụ đã được báo cáo
                HideCompletionText();
            }
            else if (hasCompletedAppleQuest && !hasCompletedArmorQuest)
            {
                appleCountText.gameObject.SetActive(false);
                // Ẩn thông báo hoàn thành khi nhiệm vụ đã được báo cáo
                HideCompletionText();
            }
        }
    }

    public void MeetKnight()
    {
        knightCount++;
        if (knightCountText != null)
        {
            knightCountText.text = " " + knightCount;
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
            Destroy(imageLock1);

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
        }
    }

    private void HideCompletionText()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }

   private IEnumerator HideNextTaskHintAfterDelay()
{
    yield return new WaitForSeconds(10f); // Đợi 10 giây
    if (knightCountText != null)
    {
        knightCountText.gameObject.SetActive(false); // Ẩn text
    }
}

}
