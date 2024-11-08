using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using Firebase;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    // Các thành phần UI
    public TMP_InputField usernameInput; // Input field cho username
    public TMP_InputField passwordInput; // Input field cho password
    public TextMeshProUGUI feedbackText; // Text để hiển thị thông báo
    public TextMeshProUGUI goldText; // Text UI để hiển thị Gold
    public TextMeshProUGUI diamondText; // Text UI để hiển thị Diamond
    public Image healthImage; // Image UI để hiển thị thanh tiến trình của Health
    public Image energyImage; // Image UI để hiển thị thanh tiến trình của Energy

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    // Khởi tạo Firebase khi bắt đầu
    private async void Start()
    {
        // Kiểm tra và khắc phục các phụ thuộc của Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync();
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Phương thức đăng nhập người dùng
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
                else
                {
                    // Nếu nhân vật đã tồn tại, tải dữ liệu từ Firebase
                    await LoadCharacterData(username);
                }

                // Chuyển đến màn hình chính
                SceneManager.LoadScene("Player1"); // Thay "Player1" bằng tên của cảnh bạn muốn chuyển đến
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

    // Phương thức tìm email của người dùng theo username
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

    // Kiểm tra xem dữ liệu nhân vật đã tồn tại hay chưa
    private async Task<bool> CheckCharacterExists(string username)
    {
        var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
        return snapshot.Exists; // Trả về true nếu dữ liệu nhân vật tồn tại
    }

    // Lưu dữ liệu nhân vật lên Firebase
    private async Task SaveCharacterData(string username)
    {
        // Tạo một đối tượng CharacterData
        CharacterData characterData = new CharacterData
        {
            username = username,
            health = 100f,  // Sức khỏe khởi tạo
            energy = 100f,  // Năng lượng khởi tạo
            gold = 0,       // Gold là kiểu int
            diamond = 0,    // Diamond là kiểu int
            position = new Vector3(0, 0, 0), // Vị trí khởi tạo
            skillID = "skill_01", // ID skill
            scene = "Player1" // Lưu thông tin cảnh mà nhân vật đang đứng
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

    // Tải dữ liệu nhân vật từ Firebase
    private async Task LoadCharacterData(string username)
    {
        try
        {
            var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
            if (snapshot.Exists)
            {
                var characterData = JsonUtility.FromJson<CharacterData>(snapshot.GetRawJsonValue());

                // Cập nhật thông tin Gold, Diamond, Health và Energy trên UI
                goldText.text = "Gold: " + characterData.gold.ToString();  // Chuyển int sang string để hiển thị
                diamondText.text = "Diamond: " + characterData.diamond.ToString();  // Chuyển int sang string để hiển thị

                // Cập nhật thanh tiến trình cho Health và Energy
                healthImage.fillAmount = characterData.health / 100f;  // Giả sử max health = 100
                energyImage.fillAmount = characterData.energy / 100f;  // Giả sử max energy = 100

                // Lưu thông tin cảnh
                if (!string.IsNullOrEmpty(characterData.scene))
                {
                    string currentScene = characterData.scene;
                    SceneManager.LoadScene(currentScene);  // Tải lại cảnh mà nhân vật đang đứng
                }
                else
                {
                    Debug.LogWarning("Cảnh không được chỉ định trong dữ liệu nhân vật.");
                }
            }
            else
            {
                UpdateFeedback("Không tìm thấy dữ liệu nhân vật.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi tải dữ liệu nhân vật: " + ex.Message);
        }
    }

    // Phương thức để cập nhật thông báo trên UI
    private void UpdateFeedback(string message)
    {
        feedbackText.text = message;
        Debug.Log(message);
    }
}

// Đưa lớp CharacterData vào namespace để tránh xung đột
[System.Serializable]
public class CharacterData
{
    public string username;
    public float health;
    public float energy;
    public int gold;      // Gold là kiểu int
    public int diamond;   // Diamond là kiểu int
    public Vector3 position;
    public string skillID;
    public string scene; // Thêm trường scene để lưu tên cảnh
}
