using UnityEngine;
using UnityEngine.UI;

public class Quest_3 : MonoBehaviour
{
    public GameObject questPanel;
    public Text questText;
    public Button continueButton;
    public Button confirmButton;
    public Text currentCardText;
    public Text questWolf;
    public Text questNext;
    public Text completionText; // Text hiển thị thông báo hoàn thành
    public GameObject GOwolf;
    UI_Coin uiCoin;

    // đối thoại
    private string encouragementText = "Xin chào, ta là một thương nhân của vùng đất này!";
    private string encouragementText1 = "Hãy giúp ta truy tìm những thẻ bị thất lạc ở chỗ Cô Suna.";
    private string encouragementText2 = "Sau khi hoàn thành hãy quay về đây ta có món quà dành cho cậu.";
    private string quest1 = "Nhiệm vụ hiện tại của bạn là lật 4 cặp thẻ.";
    private string CompleteText = "Bạn nhận được 100 vàng";

    private string quest2 = "Hãy giúp ta đưa chú chó này an toàn đến Cô Suna";

    private bool isPanelVisible = false;
    private bool hasCompletedCardQuest = false;
    public static bool hasCompletedWolfQuest = false;
    private bool hasShownEncouragement = false;
    public int currentCard = 0;
    private int dialogueStep = 0;

    public static bool isQuest3;

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

        if (currentCardText != null)
        {
            currentCardText.text = "";
            currentCardText.gameObject.SetActive(false);
        }
        if (questWolf != null)
        {
            questWolf.text = "";
            questWolf.gameObject.SetActive(false);
        }
        if (questNext != null)
        {
            questNext.text = "";
            questNext.gameObject.SetActive(false);
        }

        uiCoin = FindObjectOfType<UI_Coin>();
    }

    void OnMouseDown()
    {
        if (questPanel != null && !isPanelVisible)
        {
            questPanel.SetActive(true);
            isQuest3 = true;

            if (!hasShownEncouragement && !hasCompletedCardQuest)
            {
                questText.text = encouragementText;
                hasShownEncouragement = true;
            }
            isPanelVisible = true;
        }
    }

    private void OnContinue()
    {
        if (dialogueStep == 0)
        {
            questText.text = encouragementText1;
            dialogueStep = 1;
        }
        else if (dialogueStep == 1)
        {
            questText.text = encouragementText2;
            dialogueStep = 2;
        }
        else if (dialogueStep == 2)
        {
            questText.text = quest1;
            currentCardText.gameObject.SetActive(true);
            currentCardText.text = "Số cặp thẻ đã lật: " + currentCard + "/4";

            // Kiểm tra nếu đủ 4 thẻ mới chuyển qua bước tiếp theo
            if (currentCard == 4)
            {
                questText.text = CompleteText;
                currentCardText.text = "Bạn đã lật đủ thẻ!";
                currentCardText.color = Color.yellow;
                uiCoin.AddCoins(100);
                dialogueStep = 3;

            }
        }

        else if (dialogueStep == 3)
        {
            Debug.Log("sói");
            currentCardText.gameObject.SetActive(false);
            GOwolf.gameObject.SetActive(true);
            questWolf.gameObject.SetActive(true);
            questWolf.text = "Dẫn chú chó đến Cô Suna";
            questText.text = quest2;

            // Hoàn thành nhiệm vụ sói và thưởng
            if (hasCompletedWolfQuest == true)
            {
                GOwolf.gameObject.SetActive(false);
                questText.text = CompleteText;
                dialogueStep = 4;
                questWolf.text = "";
            }
        }
        else if (dialogueStep == 4)
        {
            questNext.gameObject.SetActive(true);
            questNext.text = "Hãy đến NPC khác nhận nhiệm vụ";
            questText.text = "Cảm ơn ngươi vì sự giúp đỡ, ta đã hết thứ cho ngươi rồi!";
            Debug.Log("4");
        }
    }


    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;

            if (hasCompletedCardQuest)
            {
                currentCardText.gameObject.SetActive(false);
            }
            if (hasCompletedWolfQuest)
            {
                questWolf.gameObject.SetActive(false);
            }
        }
        isQuest3 = false;
    }
    public void CompleteQuestCard()
    {
        currentCard++;
        currentCardText.text = "Số cặp thẻ đã lật: " + currentCard + "/4";
        if (currentCard == 4)
        {
            currentCardText.color = Color.yellow;
            hasCompletedCardQuest = true;
        }
    }
    public void CompleteWolf()
    {
        hasCompletedWolfQuest = true;
        questWolf.text = "Bạn đã hoàn thành nhiệm vụ";
        questWolf.color = Color.yellow;

    }


    private void ShowCompletionText(string message)
    {
        if (completionText != null)
        {
            completionText.text = message;
            completionText.gameObject.SetActive(true); // Hiển thị Text

            // Tắt Text sau 2 giây
            Invoke(nameof(HideCompletionText), 2f);
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
