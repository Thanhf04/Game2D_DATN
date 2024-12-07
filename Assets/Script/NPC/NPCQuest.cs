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
    public Text currentCardText;
    public UI_Coin uiCoin;

    private string initialQuestText = "Xin chào chàng hiệp sĩ, bạn là người được chọn để giải cứu vùng đất này.";
    private string secondQuestText = "Nhiệm vụ đầu tiên của bạn là tìm lại thanh kiếm đã mất.";
    private string congratulationText = "Giỏi lắm chàng trai, bây giờ bạn có thể dùng chuột trái để tấn công!";
    private string thirdQuestText = "Nhiệm vụ tiếp theo của bạn là giết 5 con quái.";
    private string rewardCompletionText = "Chúc mừng bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 50 vàng!";
    private string finalEncouragementText = "Chúc mừng chàng trai, bây giờ bạn có thể tiếp tục cuộc hành trình rồi.";

    #region Nhiệm vụ lật thẻ
    private string CardQuestText = "Nhiệm vụ lần này của bạn là lật đúng 2 lần thẻ giống nhau";
    private string CardQuestText1 = "Sau đó quay về đây cậu sẽ nhận phần quà bất ngờ";
    private string congratulationCardQuestText = "Bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 70 vàng";
    #endregion

    private bool isPanelVisible = false;
    private bool isQuestStarted = false;
    private bool hasShownCongratulation = false;
    private bool hasReceivedReward = false;

    public int swordCount = 0;
    public int monsterKillCount = 0;
    public int currentCard = 0;

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
        if (currentCardText != null)
        {
            currentCardText.text = "";
            currentCardText.gameObject.SetActive(false);
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
            else if (currentCard == 0 && monsterKillCount == 5)
            {
                questText.text = CardQuestText;
                isQuestStarted = true;

                currentCardText.gameObject.SetActive(true);
                currentCardText.text = "Số thẻ đã lật thành công: " + currentCard + "/2";
            }
            else if (currentCard == 2)
            {
                questText.text = congratulationCardQuestText;
                currentCardText.gameObject.SetActive(false);
                uiCoin.AddCoins(70);
            }
            else if (hasReceivedReward && currentCard == 2)
            {
                // Câu chúc mừng cuối cùng
                questText.text = finalEncouragementText;
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
    public void CurrentCard()
    {
        currentCard++;
        currentCardText.text = "Số thẻ đã lật thành công: " + currentCard + "/2";
    }
}

