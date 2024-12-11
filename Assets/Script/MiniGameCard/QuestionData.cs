using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string question; // Câu hỏi với chỗ trống
    public string answer; // Đáp án đúng
    public string[] options; // Các gợi ý (nếu có)
}

[CreateAssetMenu(fileName = "QuestionData", menuName = "Game/Question Data")]
public class QuestionData : ScriptableObject
{
    public List<Question> questions; // Danh sách các câu hỏi
}
