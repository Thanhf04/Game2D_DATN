using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    private FirebaseAuth auth;  // Firebase Auth instance

    private void Start()
    {
        // Lấy instance FirebaseAuth
        auth = FirebaseAuth.DefaultInstance;
    }

    // Hàm để đăng xuất
    public void LogoutUser()
    {
        // Kiểm tra xem người dùng có đăng nhập hay không
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            Debug.Log("Người dùng đã đăng xuất.");
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.Save();
            SceneManager.LoadScene("Login");
        }
        else
        {
            Debug.LogWarning("Không có người dùng nào đang đăng nhập.");
        }
    }
}
