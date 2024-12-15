using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    // Hàm chỉ chuyển màn hình về màn hình đăng nhập mà không đăng xuất
    public void GoToLoginScene()
    {
        // Chuyển đến màn hình đăng nhập (bạn thay "LoginScene" bằng tên cảnh của bạn)
        SceneManager.LoadScene(0);  // Thay "LoginScene" bằng tên của màn hình đăng nhập của bạn
    }
}
