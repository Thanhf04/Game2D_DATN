using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseAuthManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput; // Trường mới để xác nhận mật khẩu
    public TMP_InputField usernameInput;
    public TextMeshProUGUI feedbackText;

    public Button registerButton; // Nút Đăng Ký
    public Button verifyButton;   // Nút Xác Minh

    public GameObject mainCanvas; // Canvas chính để chuyển đến
    public GameObject currentCanvas; // Canvas hiện tại (canvas này sẽ bị ẩn)

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    private async void Start()
    {
        // Khởi tạo Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync();
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Gán các hàm cho nút
        registerButton.onClick.AddListener(RegisterUser);
        verifyButton.onClick.AddListener(CheckEmailVerification);

        // Ẩn nút xác minh ban đầu
        verifyButton.gameObject.SetActive(false);
    }

    public async void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        string username = usernameInput.text;

        // Kiểm tra tính hợp lệ của các trường
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(username))
        {
            UpdateFeedback("Vui lòng điền tất cả các trường.");
            return;
        }

        if (password != confirmPassword)
        {
            UpdateFeedback("Mật khẩu không khớp. Vui lòng thử lại.");
            return;
        }

        if (password.Length < 6) // Kiểm tra độ dài mật khẩu
        {
            UpdateFeedback("Mật khẩu phải ít nhất 6 ký tự.");
            return;
        }

        // Kiểm tra xem tên người dùng đã tồn tại chưa
        bool usernameExists = await CheckUsernameExists(username);
        if (usernameExists)
        {
            UpdateFeedback("Tên người dùng này đã tồn tại. Vui lòng chọn tên khác.");
            return;
        }

        try
        {
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = result.User;

            // Gửi email xác minh
            await SendEmailVerification(newUser);
            UpdateFeedback("Email xác minh đã được gửi đến: " + newUser.Email);

            // Lưu thông tin người dùng vào Firebase
            await SaveUserData(newUser.UserId, username, newUser.Email);

            // Hiện nút xác minh
            verifyButton.gameObject.SetActive(true);
        }
        catch (Exception ex)
        {
            UpdateFeedback("Đăng ký gặp lỗi: " + ex.Message);
        }
    }


    private async Task SendEmailVerification(FirebaseUser user)
    {
        await user.SendEmailVerificationAsync();
    }

    public async void CheckEmailVerification()
    {
        FirebaseUser user = auth.CurrentUser; // Lấy người dùng hiện tại
        if (user != null)
        {
            await user.ReloadAsync(); // Tải lại thông tin người dùng
            if (user.IsEmailVerified)
            {
                UpdateFeedback("Email đã được xác minh.");
                // Lưu tên người dùng và email vào Firebase
                await SaveUserData(user.UserId, usernameInput.text, user.Email);
                SwitchToNewCanvas();
            }
            else
            {
                UpdateFeedback("Email chưa được xác minh. Vui lòng kiểm tra hộp thư của bạn.");
                UpdateFeedback("Nếu bạn không thấy email, hãy kiểm tra thư mục Spam hoặc yêu cầu gửi lại email xác minh.");
            }
        }
        else
        {
            UpdateFeedback("Không có người dùng hiện tại. Vui lòng đăng nhập.");
        }
    }

    private async Task SaveUserData(string userId, string username, string email)
    {
        User user = new User { Username = username, Email = email };
        string json = JsonUtility.ToJson(user);

        // Lưu thông tin người dùng vào Firebase
        await databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        UpdateFeedback("Tên người dùng và email đã được lưu.");
    }
    private async Task<bool> CheckUsernameExists(string username)
    {
        // Truy vấn vào Firebase để kiểm tra xem tên người dùng đã tồn tại
        DataSnapshot snapshot = await databaseReference.Child("users").OrderByChild("Username").EqualTo(username).GetValueAsync();

        return snapshot.Exists; // Nếu có dữ liệu trả về, tức là tên người dùng đã tồn tại
    }


    public async void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;
            await user.ReloadAsync(); // Tải lại thông tin người dùng
            if (user.IsEmailVerified)
            {
                UpdateFeedback("Người dùng đã đăng nhập thành công và email đã được xác minh.");
                SwitchToNewCanvas();
            }
            else
            {
                UpdateFeedback("Email chưa được xác minh. Vui lòng kiểm tra hộp thư của bạn.");
            }
        }
        catch (Exception ex)
        {
            UpdateFeedback("Đăng nhập gặp lỗi: " + ex.Message);
        }
    }

    private void UpdateFeedback(string message)
    {
        feedbackText.text = message;
        Debug.Log(message);
    }

    private void SwitchToNewCanvas()
    {
        if (mainCanvas != null)
        {
            mainCanvas.SetActive(true); // Hiển thị canvas mới
            if (currentCanvas != null)
            {
                currentCanvas.SetActive(false); // Ẩn canvas hiện tại
            }
        }
        else
        {
            UpdateFeedback("Không tìm thấy canvas chính. Vui lòng kiểm tra tên canvas.");
        }
    }

    // Lớp để lưu thông tin người dùng
    [Serializable]
    public class User
    {
        public string Username;
        public string Email; // Thêm trường Email
    }
}
