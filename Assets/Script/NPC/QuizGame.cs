using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizGame : MonoBehaviour
{
    public List<Question> questions;
    public Text questionText;
    public Button[] answerButtons;
    public GameObject quizPanel;
    public Text[] feedbackTexts;
    public Text scoreText;
    public GameObject doorObject;
    public GameObject messagePanel;
    public Text messageText;
    public Button confirmButton;
    public Button exitButton;

    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;

    void Start()
    {
        quizPanel.SetActive(false);
        messagePanel.SetActive(false);
        confirmButton.onClick.AddListener(StartQuiz);
        exitButton.onClick.AddListener(CloseQuiz);
        HideAllFeedback();
        UpdateScore();
    }

    void ShowQuestion()
    {
        HideAllFeedback();

        if (currentQuestionIndex < questions.Count)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            questionText.text = currentQuestion.questionText;

            for (int i = 0; i < currentQuestion.options.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.options[i];
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
            }
        }
    }

    void CheckAnswer(int index)
    {
        Question currentQuestion = questions[currentQuestionIndex];

        if (index == currentQuestion.correctAnswerIndex)
        {
            CreateFeedback(currentQuestionIndex, "Chúc mừng bạn đã trả lời đúng!", true);
            correctAnswers++;
        }
        else
        {
            CreateFeedback(currentQuestionIndex, "Thật tiếc bạn đã trả lời sai! Bạn sẽ phải trả lời lại từ đầu.", false);
            StartCoroutine(HandleWrongAnswer());
            return;
        }

        currentQuestionIndex++;
        StartCoroutine(WaitAndShowNextQuestion(1.5f));
    }

    IEnumerator WaitAndShowNextQuestion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HideAllFeedback();

        if (correctAnswers >= 4)
        {
            quizPanel.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(3);
        }
        else
        {
            if (currentQuestionIndex < questions.Count)
            {
                ShowQuestion();
            }
        }

        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Số câu trả lời đúng: " + $"{correctAnswers}/4";
    }

    IEnumerator HandleWrongAnswer()
    {
        yield return new WaitForSeconds(1.5f);
        HideAllFeedback();
        ResetQuiz();
    }

    void ResetQuiz()
    {
        correctAnswers = 0;
        currentQuestionIndex = 0;
        ShowQuestion();
        UpdateScore();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ShowMessage("Thật không may, cánh cửa này đã bị đóng.\nBạn phải trả lời đúng 4 câu hỏi để mở khóa được cánh cửa.");
        }
    }

    void ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);
    }

    public void StartQuiz()
    {
        messagePanel.SetActive(false);
        quizPanel.SetActive(true);
        ShowQuestion();
    }

    public void CloseQuiz()
    {
        quizPanel.SetActive(false);
        HideAllFeedback();
    }

    void CreateFeedback(int questionIndex, string message, bool isCorrect)
    {
        if (questionIndex < feedbackTexts.Length)
        {
            Text feedbackText = feedbackTexts[questionIndex];
            feedbackText.text = message;
            feedbackText.color = isCorrect ? Color.green : Color.red;
            feedbackText.gameObject.SetActive(true);

            StartCoroutine(HideFeedbackAfterDelay(feedbackText, 1.5f));
        }
    }

    IEnumerator HideFeedbackAfterDelay(Text feedback, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedback != null)
        {
            feedback.gameObject.SetActive(false);
        }
    }

    void HideAllFeedback()
    {
        foreach (var feedback in feedbackTexts)
        {
            if (feedback != null)
            {
                feedback.gameObject.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options;
        public int correctAnswerIndex;
    }
}
