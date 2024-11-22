using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadZone : MonoBehaviour
{
    public GameObject gameOverPanel; // Panel Game Over
    public Button tryAgainButton;    // Nút "Try Again"
    public Button resetButton;       // Nút "Reset"
    public Button mainMenuButton;    // Nút "Main Menu"

    void Start()
    {
        gameOverPanel.SetActive(false);
        // Gán sự kiện cho các nút
        tryAgainButton.onClick.AddListener(OnTryAgain);
        resetButton.onClick.AddListener(OnReset);
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có tag "Player" không
        if (collision.CompareTag("Player"))
        {
            Die(); // Gọi hàm Die để hiển thị GameOver Panel
            collision.gameObject.SetActive(false); // Vô hiệu hóa Player
        }
    }

    void OnTryAgain()
    {
        // Tải lại cảnh hiện tại để chơi lại
        Time.timeScale = 1f; // Tiếp tục game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnReset()
    {
        // Reset lại trạng thái game
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnMainMenu()
    {
        // Quay lại menu chính
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene"); // Thay "SampleScene" bằng tên cảnh menu chính
    }

    void Die()
    {
        Debug.Log("Player is dead");
        ShowGameOverPanel();
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true); // Hiển thị panel Game Over
        Time.timeScale = 0f;           // Tạm dừng game
    }
}