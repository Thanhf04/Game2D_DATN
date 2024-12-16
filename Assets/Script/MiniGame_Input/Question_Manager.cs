using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Question_Manager : MonoBehaviour
{
    public TextMeshProUGUI ques_number;
    public TextMeshProUGUI question;
    public TMP_InputField[] answer;
    public Button submit;
    public TextMeshProUGUI resultAnswer;
    Quest_3 q3;

    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;

    void Start()
    {
        // Thêm danh sách câu hỏi
        questions.Add(new Question("Nhiệm vụ đầu tiên bạn nhận ở Màn 2 là từ NPC [...] ", new List<string> { "Thương nhân" }));
        questions.Add(new Question("Nhiệm vụ thứ 1 bạn cần lật tổng cộng [...] cặp thẻ. ", new List<string> { "4" }));
        questions.Add(new Question("Nhiệm vụ thứ 2 bạn cần dắt chú chó đi đến [...] ", new List<string> { "Cô Suna" }));


        // Hiển thị câu hỏi đầu tiên
        DisplayQuestion();
        submit.onClick.AddListener(CheckAnswer);

        q3 = FindObjectOfType<Quest_3>();
    }

    private void DisplayQuestion()
    {
        // Cập nhật UI
        ques_number.text = $"Câu hỏi {currentQuestionIndex + 1}/{questions.Count}";
        Question currentQuestion = questions[currentQuestionIndex];
        question.text = currentQuestion.Text;

        // Reset các ô nhập liệu và kết quả
        foreach (var inputField in answer)
        {
            inputField.text = "";
        }
        resultAnswer.text = "";
    }

    private void CheckAnswer()
    {
        Question currentQuestion = questions[currentQuestionIndex];
        bool isCorrect = true;

        for (int i = 0; i < currentQuestion.Answers.Count; i++)
        {
            if (i >= answer.Length)
            {
                isCorrect = false;
                Debug.LogError($"Không đủ ô nhập liệu cho câu hỏi {currentQuestionIndex + 1}.");
                break;
            }

            // Lấy đáp án từ người chơi và chuẩn hóa
            string playerAnswer = (answer[i].text.Trim().ToLower());
            string correctAnswer = (currentQuestion.Answers[i].Trim().ToLower());

            Debug.Log($"Đáp án đúng: {correctAnswer}, Người chơi nhập: {playerAnswer}");

            // So sánh đáp án
            if (playerAnswer != correctAnswer)
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            resultAnswer.text = "Bạn đã trả lời đúng";
            resultAnswer.color = Color.green;
            q3.CompleteQuestInput();
            currentQuestionIndex++;
            if (currentQuestionIndex < questions.Count)
            {
                // Hiển thị câu hỏi tiếp theo sau 1.5 giây
                Invoke("DisplayQuestion", 1.5f);
            }
            else
            {
                resultAnswer.text = "Bạn đã hoàn thành tất cả câu hỏi!";
                submit.interactable = false; // Vô hiệu hóa nút Submit
            }
        }
        else
        {
            resultAnswer.text = "Bạn đã trả lời sai";
            resultAnswer.color = Color.red;
        }
    }


    // Lớp định nghĩa câu hỏi
    private class Question
    {
        public string Text { get; }
        public List<string> Answers { get; }

        public Question(string text, List<string> answers)
        {
            this.Text = text;
            this.Answers = answers;
        }
    }
}