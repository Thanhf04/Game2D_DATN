using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LogoutUI : MonoBehaviour
{


    private FirebaseAuth auth;

    private void Start()
    {
        // Khởi tạo FirebaseAuth instance
        auth = FirebaseAuth.DefaultInstance;
    }

    // Hàm đăng xuất
    public void LogoutUser()
    {
        // Kiểm tra xem người dùng có đang đăng nhập hay không
        if (auth.CurrentUser != null)
        {
            auth.SignOut();  // Đăng xuất người dùng hiện tại

            // Xóa dữ liệu username trong PlayerPrefs
            PlayerPrefs.DeleteKey("username");  // Chỉ xóa key "username"

           

            // Sau khi đăng xuất, bạn có thể chuyển người chơi về màn hình đăng nhập
            // Thay số 0 với chỉ số scene của màn hình đăng nhập
            SceneManager.LoadScene(0);
        }
       
    }

    // Cập nhật thông báo phản hồi
}
