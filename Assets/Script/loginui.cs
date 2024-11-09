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
    public TMP_InputField usernameInput;  // Input field cho username
    public TMP_InputField passwordInput;  // Input field cho password
    public TextMeshProUGUI feedbackText;  // Text để hiển thị thông báo

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    private async void Start()
    {
        // Kiểm tra và khắc phục các phụ thuộc của Firebase
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
            // Đăng nhập người dùng bằng email và password
            Debug.Log("Đăng nhập với email: " + email);
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;
            await user.ReloadAsync();  // Tải lại thông tin người dùng

            // Kiểm tra email có xác minh chưa
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
                    await SaveCharacterData(username);  // Lưu dữ liệu nhân vật nếu chưa tồn tại
                }

                // Chuyển đến màn hình chính
                SceneManager.LoadScene("Player1");
            }
            else
            {
                UpdateFeedback("Email chưa được xác minh. Vui lòng kiểm tra hộp thư của bạn.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Đăng nhập gặp lỗi: " + ex.Message);
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
                    Debug.Log("Tìm thấy email: " + user.Child("Email").Value.ToString());
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
        return snapshot.Exists;
    }

    private async Task SaveCharacterData(string username)
    {
        CharacterData characterData = new CharacterData
        {
            username = username,
            healthMax = 100f,           // Health tối đa
            healthCurrent = 100f,       // Health hiện tại (ban đầu là max)
            energyMax = 100f,           // Energy tối đa
            energyCurrent = 100f,       // Energy hiện tại (ban đầu là max)
            gold = 0,
            diamond = 0,
            exp = 0,                    // EXP mặc định
            enemyPoints = 0,            // Điểm enemy mặc định
            level = 1,                  // Level mặc định là 1
            damage = 10f,               // Damage mặc định
            position = new Vector3(0, 0, 0),
            skillID = "skill_01",
            scene = "Player1"           // Cảnh mặc định
        };

        string jsonData = JsonUtility.ToJson(characterData);
        try
        {
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

[System.Serializable]
public class CharacterData
{
    public string username;
    public float healthMax;
    public float healthCurrent;
    public float energyMax;
    public float energyCurrent;
    public int gold;
    public int diamond;
    public float exp;
    public int enemyPoints;
    public int level;
    public float damage;  // Thêm sát thương (damage) của nhân vật
    public Vector3 position;
    public string skillID;
    public string scene;
}
