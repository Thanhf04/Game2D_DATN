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
    public UI_Coin uiCoin;

    private string initialQuestText = "Xin chào chàng hiệp sĩ, bạn là người được chọn để giải cứu vùng đất này.";
    private string secondQuestText = "Nhiệm vụ đầu tiên của bạn là tìm lại thanh kiếm đã mất.";
    private string congratulationText = "Giỏi lắm chàng trai, bây giờ bạn có thể dùng chuột trái để tấn công!";
    private string thirdQuestText = "Nhiệm vụ tiếp theo của bạn là giết 5 con quái.";
    private string rewardCompletionText = "Chúc mừng bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 50 vàng!";
    private string finalEncouragementText = "Chúc mừng chàng trai, bây giờ bạn có thể tiếp tục cuộc hành trình rồi.";
    private string continuareText = "Còn hãy đi tìm người thợ rèn để học tập thêm.";

    private bool isPanelVisible = false;
    private bool isQuestStarted = false;
    private bool hasShownCongratulation = false;
    private bool hasReceivedReward = false;

    private int swordCount = 0;
    private int monsterKillCount = 0;

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
        }
    }

    private void OnContinue()
    {
        if (questPanel != null)
        {
            if (!isQuestStarted && swordCount == 0)
            {
                // Nhiệm vụ kiếm
                questText.text = secondQuestText;
                isQuestStarted = true;

                swordCountText.gameObject.SetActive(true);
                swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1";
            }
            else if (swordCount == 1 && monsterKillCount < 5)
            {
                // Nhiệm vụ giết quái
                questText.text = thirdQuestText;

                swordCountText.gameObject.SetActive(false);
                monsterCountText.gameObject.SetActive(true);
                monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/5";
            }
            else if (swordCount == 1 && monsterKillCount >= 5 && !hasReceivedReward)
            {
                // Hoàn thành nhiệm vụ và nhận thưởng
                questText.text = rewardCompletionText;

                if (uiCoin != null)
                {
                    uiCoin.AddCoins(50);
                }

                hasReceivedReward = true;
                monsterCountText.gameObject.SetActive(false);
            }
            else if (hasReceivedReward)
            {
                // Câu chúc mừng cuối cùng
                questText.text = finalEncouragementText;
                questText.text = continuareText;
            }
        }
    }

    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;
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
}
