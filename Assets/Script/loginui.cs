using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;  // Input field cho username
    public TMP_InputField passwordInput;  // Input field cho mật khẩu
    public TextMeshProUGUI feedbackText;  // Text để hiển thị phản hồi
    public GameObject player; // Thêm GameObject cho nhân vật, khai báo ở đây

    private FirebaseAuth auth;            // FirebaseAuth instance
    private DatabaseReference databaseReference;  // Firebase Database instance

    private async void Start()
    {
        // Kiểm tra và thiết lập Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                UpdateFeedback("Firebase không khởi tạo được!");
                Debug.LogError("Firebase initialization failed: " + task.Result);
            }
        });
    }

    // Hàm đăng nhập
    public async void LoginUser()
    {
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

                    // Tải dữ liệu người chơi từ Firebase sau khi đăng nhập thành công
                    await LoadPlayerDataFromFirebase(username);  // Load data từ Firebase

                    // Chuyển cảnh sau khi load dữ liệu
                    SceneManager.LoadScene("Player1");
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
                if (user.Child("Email").Value != null)
                {
                    return user.Child("Email").Value.ToString();
                }
            }
        }
        return null;
    }

    // Lưu dữ liệu người chơi lên Firebase
    public async Task SavePlayerDataToFirebase(string username)
    {
        // Giả sử bạn đã có các giá trị này từ game
        int damage = 10;
        int diamond = 100;
        Vector3 position = new Vector3(1, 1, 1);
        int gold = 50;
        int exp = 200;
        int energyCurrent = 80;
        int energyMax = 100;
        int healthMax = 100;
        int healthCurrent = 75;

        // Tạo đối tượng PlayerData
        var playerDict = new Dictionary<string, object>
        {
            { "Damage", damage },
            { "Diamond", diamond },
            { "PositionX", position.x },
            { "PositionY", position.y },
            { "PositionZ", position.z },
            { "Gold", gold },
            { "Exp", exp },
            { "EnergyCurrent", energyCurrent },
            { "EnergyMax", energyMax },
            { "HealthMax", healthMax },
            { "HealthCurrent", healthCurrent }
        };

        // Lưu vào Firebase Realtime Database
        await databaseReference.Child("players").Child(username).SetValueAsync(playerDict);
        Debug.Log("Player data saved to Firebase.");
    }

    // Tải dữ liệu người chơi từ Firebase
    public async Task LoadPlayerDataFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            // Lấy dữ liệu từ snapshot
            int health = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());
            int energy = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());
            int exp = int.Parse(snapshot.Child("Exp").Value.ToString());

            // Hiển thị thông tin trong game hoặc gán lại giá trị cho các biến
            Debug.Log($"Player Data Loaded: {username}, Health: {health}, Energy: {energy}, EXP: {exp}");

            // Cập nhật lại UI hoặc các biến trong game (ví dụ, gán giá trị cho slider, text)
        }
        else
        {
            Debug.LogWarning("No data found for player: " + username);
        }
    }

    // Cập nhật thông báo phản hồi
    private void UpdateFeedback(string message)
    {
        feedbackText.text = message;  // Hiển thị thông điệp trên UI
        Debug.Log(message);  // In thông điệp ra Console của Unity
    }
}
