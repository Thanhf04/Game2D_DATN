using UnityEngine;
using UnityEngine.UI;

public class NPCQuestSkill2 : MonoBehaviour
{
    public GameObject questPanel;
    public Text questText;
    public Button continueButton;
    public Button confirmButton;
    public Text fireEnemyCountText;
    public Text textNPCApple;
    public Text completionText; // Thêm Text hoàn thành

    public static bool isNPCQuestSkill2 = false;

    public Image imageLock2;


    private string greetingText = "Chào chàng trai, bạn thật kiên cường khi đến được nơi đây.";
    private string nextQuestText = "Nhiệm vụ tiếp theo của bạn là đi lên phía trên đánh nhau với quái lửa để mở khóa cho mình kỹ năng thứ 2!";
    private string completionQuestText = "Chúc mừng chàng trai, bây giờ bạn có thể ấn E để sử dụng kỹ năng thứ 2 của mình! Hãy chuẩn bị và tiếp tục cuộc hành trình phía trước!";
    private string continueJourneyText = "Bây giờ bạn có thể tiếp tục cuộc hành trình của mình!";

    private bool isPanelVisible = false;
    private bool hasShownGreeting = false;
    public static bool hasCompletedQuest = false;

    private int fireEnemyCount = 0;
    private int requiredFireEnemyCount = 1;

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

        if (fireEnemyCountText != null)
        {
            fireEnemyCountText.gameObject.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinue);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    void OnMouseDown()
    {
        if (questPanel != null && !isPanelVisible)
        {
            questPanel.SetActive(true);
            isNPCQuestSkill2 = true;

            if (hasCompletedQuest)
            {
                questText.text = completionQuestText + "\n" + continueJourneyText;
                fireEnemyCountText.gameObject.SetActive(false);
            }
            else
            {
                textNPCApple.gameObject.SetActive(false);
                questText.text = greetingText;
                hasShownGreeting = true;
            }

            isPanelVisible = true;
        }
    }

    private void OnContinue()
    {
        if (hasShownGreeting)
        {
            questText.text = nextQuestText;
            hasShownGreeting = false;

            if (fireEnemyCountText != null)
            {
                fireEnemyCountText.gameObject.SetActive(true);
                UpdateFireEnemyCountText();
            }
        }
    }

    private void OnConfirm()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
            isPanelVisible = false;
            isNPCQuestSkill2 = false;
            HideCompletionText(); // Ẩn Text hoàn thành khi nhiệm vụ được xác nhận
        }
    }

    public void DefeatFireEnemy()
    {
        fireEnemyCount++;
        UpdateFireEnemyCountText();

        if (fireEnemyCount >= requiredFireEnemyCount && !hasCompletedQuest)
        {
            hasCompletedQuest = true;
            Destroy(imageLock2, 2f);
            ShowCompletionText("Báo cáo với NPC để tiếp tục"); // Hiển thị thông báo hoàn thành
        }
    }

    private void UpdateFireEnemyCountText()
    {
        if (fireEnemyCountText != null)
        {
            fireEnemyCountText.text = "Quái lửa cần tiêu diệt: " + fireEnemyCount + "/" + requiredFireEnemyCount;

            if (fireEnemyCount >= requiredFireEnemyCount)
            {
                fireEnemyCountText.color = Color.yellow;
            }
        }
    }

    private void ShowCompletionText(string message)
    {
        if (completionText != null)
        {
            completionText.text = message;
            completionText.gameObject.SetActive(true); // Hiển thị Text

            // Tắt Text sau 2 giây
            // Invoke(nameof(HideCompletionText), 2f);
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
