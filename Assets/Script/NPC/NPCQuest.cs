using System.Collections;
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
    public Text NVtimThoRenText;
    public Text completionText; // Text để hiển thị thông báo
    public UI_Coin uiCoin;
    public static bool isQuest = false;

    private string initialQuestText =
        "Xin chào chàng hiệp sĩ, bạn là người được chọn để giải cứu vùng đất này.";
    private string secondQuestText = "Nhiệm vụ đầu tiên của bạn là tìm lại thanh kiếm đã mất.";
    private string congratulationText =
        "Giỏi lắm chàng trai, bây giờ bạn có thể dùng chuột trái để tấn công!";
    private string thirdQuestText = "Nhiệm vụ tiếp theo của bạn là giết 5 con quái.";
    private string rewardCompletionText =
        "Chúc mừng bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 50 vàng!";
    private string finalEncouragementText =
        "Chúc mừng chàng trai, bây giờ bạn có thể tiếp tục cuộc hành trình rồi.";
    private string continuareText = "Còn hãy đi tìm người thợ rèn để học tập thêm.";

    private bool isPanelVisible = false;
    private bool isQuestStarted = false;
    private bool hasShownCongratulation = false;
    private bool hasReceivedReward = false;
    private bool hasShownContinuareText = false;


    private int swordCount = 0;
    private int monsterKillCount = 0;
    private NPCQuestFirebase npcQuestFirebase;


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
            swordCountText.gameObject.SetActive(false);
        }

        if (monsterCountText != null)
        {
            monsterCountText.text = "";
            monsterCountText.gameObject.SetActive(false);
        }
        if (NVtimThoRenText != null)
        {
            NVtimThoRenText.text = "";
            NVtimThoRenText.gameObject.SetActive(false);
        }

        if (completionText != null)
        {
            completionText.gameObject.SetActive(false); // Ẩn thông báo ban đầu
        }

        uiCoin = FindObjectOfType<UI_Coin>();
    }

    void OnMouseDown()
    {
        if (questPanel != null && !isPanelVisible)
        {
            questPanel.SetActive(true);
            isQuest = true;

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

                swordCountText.gameObject.SetActive(true);
                swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1";
                swordCountText.color = Color.white;

                // Ẩn thông báo khi bắt đầu nhiệm vụ mới
                HideCompletionMessage();
            }
            else if (swordCount == 1 && monsterKillCount < 5)
            {
                questText.text = thirdQuestText;

                swordCountText.gameObject.SetActive(false);
                monsterCountText.gameObject.SetActive(true);
                monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/5";
                monsterCountText.color = Color.white;

                // Ẩn thông báo khi tiếp tục nhiệm vụ
                HideCompletionMessage();
            }
            else if (swordCount == 1 && monsterKillCount >= 5 && !hasReceivedReward)
            {
                questText.text = rewardCompletionText;

                if (uiCoin != null)
                {
                    uiCoin.AddCoins(50);
                }
                hasReceivedReward = true;
                monsterCountText.gameObject.SetActive(false);
            }
            else if (hasReceivedReward && !hasShownContinuareText)
            {
                questText.text = continuareText;
                NVtimThoRenText.gameObject.SetActive(true); // Hiển thị text
                NVtimThoRenText.text = continuareText;
                hasShownContinuareText = true;
                // Bắt đầu coroutine để ẩn text sau 10 giây
                StartCoroutine(HideContinuareTextAfterDelay());
            }
        }
    }

    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;
            isQuest = false;

            // Ẩn thông báo nếu đã hoàn thành nhiệm vụ
            if (hasReceivedReward)
            {
                HideCompletionMessage();
            }
        }
    }

    public void FindSword()
    {
        swordCount = 1;
        swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1";

        if (swordCount == 1)
        {
            swordCountText.color = Color.yellow;

            // Hiển thị thông báo hoàn thành nhiệm vụ
            // StartCoroutine(ShowCompletionMessage("Đã hoàn thành nhiệm vụ, hãy quay lại NPC để nhận thưởng!"));
            ShowCompletionMessage("Báo cáo cho Trưởng làng");
        }
    }

    public void KillMonster()
    {
        monsterKillCount++;
        monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/5";

        if (monsterKillCount == 5)
        {
            monsterCountText.color = Color.yellow;

            // Hiển thị thông báo hoàn thành nhiệm vụ
            // StartCoroutine(ShowCompletionMessage("Đã hoàn thành nhiệm vụ, hãy quay lại NPC để nhận thưởng!"));
            ShowCompletionMessage("Báo cáo cho Trưởng làng");
        }
    }

    private void ShowCompletionMessage(string message)
    {
        if (completionText != null)
        {
            completionText.text = message;
            completionText.color = Color.yellow;
            completionText.gameObject.SetActive(true);
        }
    }

    private void HideCompletionMessage()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }
    private IEnumerator HideContinuareTextAfterDelay()
    {
        yield return new WaitForSeconds(10f); // Đợi 10 giây
        if (NVtimThoRenText != null)
        {
            NVtimThoRenText.gameObject.SetActive(false); // Ẩn text
        }
    }

}