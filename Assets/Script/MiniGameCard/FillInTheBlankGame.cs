using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillInTheBlankGame : MonoBehaviour
{
    public QuestionData questionData; // File dữ liệu câu hỏi
    public TextMeshProUGUI questionText; // Hiển thị câu hỏi
    public TextMeshProUGUI resultText; // Hiển thị kết quả
    public TMP_InputField inputField; // Ô nhập liệu
    public GameObject minigame;
    public Button confirmButton; // Nút xác nhận
    public GameObject player;
    private Dichuyennv1 dichuyen1Script;

    public GameObject inventory;
    private Inventory inventoryScript;

    public GameObject rewardPanel; // Panel hiển thị phần thưởng
    public TextMeshProUGUI rewardText; // Văn bản trong panel phần thưởng

    public GameObject dialoguePanel; // Panel hội thoại NPC
    public TextMeshProUGUI dialogueText; // Văn bản hội thoại NPC
    public Button nextButton; // Nút "Tiếp theo" trong hội thoại

    public GameObject[] uiElements; // Các đối tượng UI muốn vô hiệu hóa

    private Question currentQuestion;
    private int currentIndex = 0;
    private int correctAnswers = 0; // Số câu trả lời đúng
    private bool isMiniGameActive = false; // Kiểm tra mini game có đang hoạt động không
    private bool isDialogueActive = false; // Kiểm tra đối thoại có đang hiển thị không
    private bool isInNpcZone = false;

    void Start()
    {
        // Ẩn các panel ban đầu
        dialoguePanel.SetActive(false);
        rewardPanel.SetActive(false);

        // Gán sự kiện cho các nút
        confirmButton.onClick.AddListener(CheckAnswer); // Nút "Xác nhận"
        nextButton.onClick.AddListener(ShowNextDialogue); // Nút "Tiếp theo"

        dichuyen1Script = player.GetComponent<Dichuyennv1>();
        inventoryScript = inventory.GetComponent<Inventory>();
    }

    void Update()
    {
        // Kiểm tra nếu người chơi nhấn phím F để hiển thị đối thoại NPC
        if (
            Input.GetKeyDown(KeyCode.F)
            && !isMiniGameActive
            && !isDialogueActive
            && isInNpcZone == true
        )
        {
            ShowDialogue(); // Gọi hàm để hiển thị đối thoại
        }

        // Kiểm tra nếu người chơi đã trả lời đúng 5 câu
        if (correctAnswers == 3 && !isMiniGameActive)
        {
            GrantReward(); // Gọi hàm thưởng
            isMiniGameActive = true;
        }

        // Kiểm tra nếu mini game đã hoàn thành và người chơi đang ở trong đối thoại
        if (isMiniGameActive && !isDialogueActive)
        {
            ShowHintDialogue(); // Hiển thị gợi ý từ NPC
        }
    }

    void ShowDialogue()
    {
        // Hiển thị đối thoại khi người chơi nhấn F và không có mini game đang hoạt động
        if (!isMiniGameActive && !isDialogueActive)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text =
                "Chào bạn! Bạn muốn chơi trò chơi điền vào chỗ trống không? Nhấn xác nhận để bắt đầu.";
            isDialogueActive = true;
            nextButton.gameObject.SetActive(true); // Hiển thị nút "Tiếp theo"
        }
    }

    void ShowNextDialogue()
    {
        if (isMiniGameActive == false)
        {
            // Ẩn panel đối thoại sau khi người chơi nhấn "Tiếp theo" và hiển thị mini game
            dialoguePanel.SetActive(false);
            dialogueText.text = ""; // Xóa văn bản đối thoại
            nextButton.gameObject.SetActive(false); // Ẩn nút "Tiếp theo"

            // Bắt đầu mini game
            minigame.SetActive(true);
            LoadQuestion(); // Tải câu hỏi đầu tiên
            if (dichuyen1Script != null)
            {
                dichuyen1Script.enabled = false;
            }
            if (inventoryScript != null)
            {
                inventoryScript.enabled = false;
            }
        }

        if (isMiniGameActive == true)
        {
            // Ẩn panel đối thoại sau khi người chơi nhấn "Tiếp theo" và đóng tất cả UI
            dialoguePanel.SetActive(false);
            dialogueText.text = ""; // Xóa văn bản đối thoại
            nextButton.gameObject.SetActive(false); // Ẩn nút "Tiếp theo"
            dichuyen1Script.enabled = true;
            inventoryScript.enabled = true;
        }
    }

    void LoadQuestion()
    {
        // Kiểm tra xem đã hết câu hỏi chưa
        if (currentIndex >= questionData.questions.Count)
        {
            resultText.text = "Hoàn thành tất cả câu hỏi!";
            questionText.text = ""; // Xóa câu hỏi
            inputField.gameObject.SetActive(false); // Ẩn ô nhập liệu
            confirmButton.gameObject.SetActive(false); // Ẩn nút xác nhận

            return;
        }

        currentQuestion = questionData.questions[currentIndex];
        questionText.text = currentQuestion.question; // Hiển thị câu hỏi
        inputField.text = ""; // Xóa ô nhập liệu cũ
        resultText.text = ""; // Xóa kết quả cũ
    }

    void CheckAnswer()
    {
        // So sánh đáp án
        if (inputField.text.Trim().ToLower() == currentQuestion.answer.Trim().ToLower())
        {
            resultText.text = "Đúng rồi!";
            correctAnswers++; // Tăng số câu trả lời đúng

            // Kiểm tra nếu trả lời đúng 5 câu
            if (correctAnswers == 5)
            {
                GrantReward(); // Gọi hàm thưởng
                return;
            }
        }
        else
        {
            resultText.text = "Sai rồi! Đáp án đúng là: " + currentQuestion.answer;
        }

        // Chuyển sang câu hỏi tiếp theo sau 2 giây
        Invoke(nameof(NextQuestion), 2f);
    }

    void NextQuestion()
    {
        currentIndex++;
        LoadQuestion();
    }

    void GrantReward()
    {
        // Hiển thị phần thưởng
        rewardText.text = "Chúc mừng! Bạn đã trả lời đúng 5 câu!";
        rewardPanel.SetActive(true);

        // Ẩn các thành phần câu hỏi
        questionText.text = "";
        minigame.SetActive(false);

        // Gọi hàm để ẩn reward sau 3 giây và hiển thị đối thoại tiếp theo
        Invoke(nameof(HideRewardAndShowDialogue), 3f);
    }

    void HideRewardAndShowDialogue()
    {
        // Ẩn panel thưởng sau 3 giây
        rewardPanel.SetActive(false);

        // Hiển thị gợi ý từ NPC
        ShowHintDialogue();
    }

    void ShowHintDialogue()
    {
        // Hiển thị panel hội thoại NPC và gợi ý
        dialoguePanel.SetActive(true);
        dialogueText.text =
            "Chúc mừng! Để đi qua vùng đất mới, bạn cần tìm cây cầu đá ở phía Tây Nam.";
        nextButton.gameObject.SetActive(true); // Hiển thị nút "Tiếp theo"
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi không sử dụng
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có phải là player không
        if (other.CompareTag("Player")) // Đảm bảo đối tượng player có tag "Player"
        {
            Debug.Log("Player entered NPC zone");
            isInNpcZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInNpcZone = false;
        }
    }
}
