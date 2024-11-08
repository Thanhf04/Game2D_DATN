using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using Firebase;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput; // Input field cho username
    public TMP_InputField passwordInput; // Input field cho password
    public TextMeshProUGUI feedbackText; // Text để hiển thị thông báo

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    private async void Start()
    {
        // Khởi tạo Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync();
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async void LoginUser()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        // Kiểm tra tính hợp lệ của các trường
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            UpdateFeedback("Vui lòng điền tất cả các trường.");
            return;
        }

        // Tìm email của người dùng theo username
        string email = await FindEmailByUsername(username);
        if (string.IsNullOrEmpty(email))
        {
            UpdateFeedback("Không tìm thấy người dùng với tên đăng nhập này.");
            return;
        }

        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;
            await user.ReloadAsync(); // Tải lại thông tin người dùng

            if (user.IsEmailVerified)
            {
                UpdateFeedback("Đăng nhập thành công!");

                // Lưu tên người dùng vào PlayerPrefs
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.Save();

                // Kiểm tra xem dữ liệu nhân vật đã tồn tại hay chưa
                bool characterExists = await CheckCharacterExists(username);
                if (!characterExists)
                {
                    // Lưu dữ liệu nhân vật vào Firebase nếu chưa tồn tại
                    await SaveCharacterData(username);
                }

                // Chuyển đến màn hình mới
                SceneManager.LoadScene("HUD"); // Thay "TEST Database" bằng tên của cảnh bạn muốn chuyển đến
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

    private async Task<string> FindEmailByUsername(string username)
    {
        if (databaseReference == null)
        {
            Debug.LogError("DatabaseReference is null! Please check Firebase setup.");
            return null;
        }

        var userRef = databaseReference.Child("users");

        var snapshot = await userRef.OrderByChild("Username").EqualTo(username).GetValueAsync();

        if (snapshot != null && snapshot.Exists)
        {
            foreach (var user in snapshot.Children)
            {
                if (user.Child("Email").Value != null)
                {
                    return user.Child("Email").Value.ToString();
                }
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy người dùng với tên đăng nhập: " + username);
        }

        return null;
    }

    private async Task<bool> CheckCharacterExists(string username)
    {
        var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
        return snapshot.Exists; // Trả về true nếu dữ liệu nhân vật tồn tại
    }

    private async Task SaveCharacterData(string username)
    {
        // Tạo một đối tượng CharacterData
        CharacterData characterData = new CharacterData
        {
            username = username,
            avatar = "path/to/avatar.png", // Đường dẫn thực tế tới avatar
            health = 0f,
            energy = 0f,
            gold = 0,
            diamond = 0,
            position = new Vector3(0, 0, 0), // Vị trí khởi tạo
            skillID = "skill_01" // ID skill
        };

        string jsonData = JsonUtility.ToJson(characterData);
        try
        {
            // Lưu dữ liệu lên Firebase
            var task = databaseReference.Child("characters").Child(username).SetRawJsonValueAsync(jsonData);
            await task;

            if (task.IsCompleted)
            {
                Debug.Log("Dữ liệu nhân vật đã được lưu lên Firebase!");
            }
            else
            {
                Debug.LogError("Có lỗi xảy ra khi lưu dữ liệu lên Firebase.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi lưu dữ liệu: " + ex.Message);
        }
    }

    private void UpdateFeedback(string message)
    {
        feedbackText.text = message;
        Debug.Log(message);
    }
}
