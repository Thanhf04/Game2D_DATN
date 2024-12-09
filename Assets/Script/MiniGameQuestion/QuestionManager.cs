using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Question[] questions;
    private int currentQuestionIndex = 0;

    public TextMeshProUGUI nameQuestion;
    public Button[] answerQuestions;
}
