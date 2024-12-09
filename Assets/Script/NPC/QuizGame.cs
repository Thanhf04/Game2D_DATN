using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizGame : MonoBehaviour
{
    public List<Question> questions; // Danh sách câu hỏi
    public Text questionText; // Text hiển thị câu hỏi
    public Button[] answerButtons; // Các nút trả lời
    public GameObject quizPanel; // Panel câu hỏi
    public Text[] feedbackTexts; // Các Text hiển thị feedback
    public Text scoreText; // Text hiển thị điểm
    public GameObject doorObject; // Cánh cửa
    public GameObject messagePanel; // Panel thông báo
    public Text messageText; // Text hiển thị thông báo
    public Button confirmButton; // Nút xác nhận

    private int currentQuestionIndex = 0; // Chỉ số câu hỏi hiện tại
    private int correctAnswers = 0; // Số câu trả lời đúng

    void Start()
    {
        quizPanel.SetActive(false); // Ẩn panel câu hỏi khi bắt đầu
        messagePanel.SetActive(false); // Ẩn panel thông báo khi bắt đầu
        confirmButton.onClick.AddListener(StartQuiz); // Thêm sự kiện cho nút xác nhận
        UpdateScore(); // Cập nhật điểm
    }

    void ShowQuestion()
    {
        // Ẩn tất cả feedback trước khi hiển thị câu hỏi mới
        HideAllFeedback();

        if (currentQuestionIndex < questions.Count)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            questionText.text = currentQuestion.questionText; // Hiển thị câu hỏi

            // Cập nhật các nút trả lời
            for (int i = 0; i < currentQuestion.options.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.options[i];
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners(); // Xóa các sự kiện cũ
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index)); // Thêm sự kiện kiểm tra câu trả lời
            }

            feedbackTexts[currentQuestionIndex].gameObject.SetActive(true); // Hiển thị feedback cho câu hỏi hiện tại
        }
    }

    void CheckAnswer(int index)
    {
        Question currentQuestion = questions[currentQuestionIndex];

        // Kiểm tra câu trả lời đúng hay sai
        if (index == currentQuestion.correctAnswerIndex)
        {
            feedbackTexts[currentQuestionIndex].text = "Chúc mừng bạn đã trả lời đúng!";
            correctAnswers++; // Tăng số câu trả lời đúng
        }
        else
        {
            feedbackTexts[currentQuestionIndex].text = "Thật tiếc bạn đã trả lời sai! Bạn sẽ phải trả lời lại từ đầu.";
            StartCoroutine(HandleWrongAnswer()); // Xử lý khi trả lời sai
            return;
        }

        currentQuestionIndex++; // Chuyển sang câu hỏi tiếp theo
        StartCoroutine(WaitAndShowNextQuestion(1.5f)); // Đợi 1,5 giây trước khi chuyển sang câu hỏi tiếp theo
    }

    IEnumerator WaitAndShowNextQuestion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Chờ trong 1,5 giây

        // Ẩn feedback sau 1,5 giây
        HideAllFeedback();

        // Kiểm tra nếu trả lời đúng 4 câu
        if (correctAnswers >= 4)
        {
            quizPanel.SetActive(false); // Ẩn panel câu hỏi
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(3); // Chuyển đến Scene 3
        }
        else
        {
            if (currentQuestionIndex < questions.Count)
            {
                ShowQuestion(); // Hiển thị câu hỏi tiếp theo
            }
        }

        UpdateScore(); // Cập nhật điểm
    }

    void HideAllFeedback()
    {
        // Ẩn tất cả feedback trước khi hiển thị câu hỏi mới hoặc khi reset
        foreach (var feedback in feedbackTexts)
        {
            feedback.gameObject.SetActive(false);
        }
    }

    void UpdateScore()
    {
        // Cập nhật điểm số hiển thị
        scoreText.text = "Số câu trả lời đúng: " + $"{correctAnswers}/4";
    }

    IEnumerator HandleWrongAnswer()
    {
        yield return new WaitForSeconds(1.5f); // Đợi 1,5 giây để người chơi đọc feedback

        // Ẩn feedback sau khi trả lời sai và reset quiz
        HideAllFeedback();
        ResetQuiz(); // Đặt lại quiz
    }

    void ResetQuiz()
    {
        correctAnswers = 0; // Đặt lại số câu trả lời đúng
        currentQuestionIndex = 0; // Quay lại câu hỏi đầu tiên
        ShowQuestion(); // Hiển thị câu hỏi đầu tiên
        UpdateScore(); // Cập nhật điểm lại
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Khi người chơi chạm vào cửa
        if (collision.collider.CompareTag("Player"))
        {
            ShowMessage("Thật không may, cánh cửa này đã bị đóng.\nBạn phải trả lời đúng 4 câu hỏi để mở khóa được cánh cửa.");
        }
    }

    void ShowMessage(string message)
    {
        // Hiển thị thông báo
        messageText.text = message;
        messagePanel.SetActive(true); // Hiển thị panel thông báo
    }

    public void StartQuiz()
    {
        // Khi người chơi nhấn nút xác nhận, ẩn thông báo và bắt đầu quiz
        messagePanel.SetActive(false);
        quizPanel.SetActive(true);
        ShowQuestion(); // Hiển thị câu hỏi đầu tiên
    }

    [System.Serializable]
    public class Question
    {
        public string questionText; // Câu hỏi
        public string[] options; // Các lựa chọn trả lời
        public int correctAnswerIndex; // Câu trả lời đúng (0, 1, 2, 3)
    }
}
