using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToPreviousScene : MonoBehaviour
{
    private int previousSceneIndex = -1; // Biến lưu trữ index của scene trước

    private void Start()
    {
        // Lấy index của scene trước đó (nếu có)
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với Player
        if (collision.CompareTag("Player"))
        {
            // Kiểm tra nếu có scene trước đó
            if (previousSceneIndex >= 0)
            {
                SceneManager.LoadScene(previousSceneIndex); // Quay lại scene trước đó
            }
            else
            {
                Debug.LogWarning("No previous scene to return to.");
            }
        }
    }
}
