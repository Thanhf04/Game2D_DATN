using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordManager : MonoBehaviour
{
    public TMP_InputField emailInput; // Input field cho email
    public Button resetPasswordButton; // Nút Đặt Lại Mật Khẩu
    public TextMeshProUGUI feedbackText; // Text để hiển thị thông báo

    private FirebaseAuth auth;

    private async void Start()
    {
        // Khởi tạo Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync();
        auth = FirebaseAuth.DefaultInstance;

        // Gán hàm cho nút
        resetPasswordButton.onClick.AddListener(ResetPassword);
    }

    public async void ResetPassword()
    {
        string email = emailInput.text;

        if (string.IsNullOrEmpty(email))
        {
            UpdateFeedback("Vui lòng nhập địa chỉ email.");
            return;
        }

        try
        {
            // Gửi email đặt lại mật khẩu
            await auth.SendPasswordResetEmailAsync(email);
            UpdateFeedback("Email đặt lại mật khẩu đã được gửi đến: " + email);
            UpdateFeedback("Vui lòng kiểm tra email của bạn và làm theo hướng dẫn để đặt lại mật khẩu.");
        }
        catch (Exception ex)
        {
            UpdateFeedback("Đã xảy ra lỗi: " + ex.Message);
        }
    }

    private void UpdateFeedback(string message)
    {
        feedbackText.text = message;
        Debug.Log(message);
    }
}
