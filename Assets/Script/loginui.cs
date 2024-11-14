using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;  // Input field cho username
    public TMP_InputField passwordInput;  // Input field cho mật khẩu
    public TextMeshProUGUI feedbackText;  // Text để hiển thị phản hồi

    private FirebaseAuth auth;            // FirebaseAuth instance
    private DatabaseReference databaseReference;  // Firebase Database instance
    private bool isFirebaseInitialized = false; // Biến cờ để kiểm tra Firebase đã khởi tạo chưa

    private async void Start()
    {
        // Kiểm tra và thiết lập Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                isFirebaseInitialized = true;
                Debug.Log("Firebase khởi tạo thành công.");
            }
            else
            {
                UpdateFeedback("Firebase không khởi tạo được!");
                Debug.LogError("Lỗi khởi tạo Firebase: " + task.Result);
            }
        });
    }

    // Hàm đăng nhập
    public async void LoginUser()
    {
        // Kiểm tra xem Firebase đã được khởi tạo chưa
        if (!isFirebaseInitialized)
        {
            UpdateFeedback("Firebase chưa được khởi tạo. Vui lòng thử lại.");
            return;
        }

        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            UpdateFeedback("Vui lòng điền đầy đủ thông tin!");
            return;
        }

        string email = await FindEmailByUsername(username);
        if (string.IsNullOrEmpty(email))
        {
            UpdateFeedback("Không tìm thấy tài khoản với tên đăng nhập này.");
            return;
        }

        try
        {
            // Đăng nhập Firebase
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;

            if (user != null)
            {
                if (user.IsEmailVerified)
                {
                    UpdateFeedback("Đăng nhập thành công!");
                    PlayerPrefs.SetString("username", username);
                    PlayerPrefs.Save();

                    // Chuyển cảnh sau khi đăng nhập thành công
                    SceneManager.LoadScene("Player1"); // Chuyển đến màn hình chính
                }
                else
                {
                    UpdateFeedback("Email chưa được xác minh.");
                }
            }
        }
        catch (FirebaseException firebaseEx)
        {
            UpdateFeedback($"Firebase Error: {firebaseEx.Message}");
            Debug.LogError($"Firebase Error: {firebaseEx.Message}");
        }
        catch (System.Exception ex)
        {
            UpdateFeedback($"Đăng nhập gặp lỗi: {ex.Message}");
            Debug.LogError($"Error: {ex.Message}");
        }
    }

    // Tìm email dựa trên username từ Firebase Realtime Database
    private async Task<string> FindEmailByUsername(string username)
    {
        var userRef = databaseReference.Child("users");
        var snapshot = await userRef.OrderByChild("Username").EqualTo(username).GetValueAsync();

        if (snapshot.Exists)
        {
            foreach (var user in snapshot.Children)
            {
                var emailValue = user.Child("Email").Value;
                if (emailValue != null)
                {
                    return emailValue.ToString();
                }
            }
        }
        return null;
    }

    // Cập nhật thông báo phản hồi
    private void UpdateFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;  // Hiển thị thông điệp trên UI
        }
        Debug.Log(message);  // In thông điệp ra Console của Unity
    }
}
