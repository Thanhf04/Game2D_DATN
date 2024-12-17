using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;  // Input field cho username
    public TMP_InputField passwordInput;  // Input field cho mật khẩu
    public TextMeshProUGUI feedbackText;  // Text để hiển thị phản hồi
    public GameObject player; // Thêm GameObject cho nhân vật, khai báo ở đây

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

        // Kiểm tra các trường nhập liệu có null không
        if (usernameInput == null || passwordInput == null || feedbackText == null)
        {
            Debug.LogError("Các đối tượng UI chưa được gán đúng trong Inspector!");
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
                    PlayerPrefs.SetString("username", username);  // Lưu tên người dùng vào PlayerPrefs
                    PlayerPrefs.Save();  // Lưu lại ngay lập tức

                    // Tải dữ liệu người chơi từ Firebase sau khi đăng nhập thành công
                    await LoadPlayerDataFromFirebase(username);

                    // Sau khi tải dữ liệu người chơi, tải lại scene mà người chơi đã chơi
                    await LoadPlayerSceneFromFirebase(username);
                }
                else
                {
                    UpdateFeedback("Email chưa được xác minh.");
                }
            }
        }
        catch (FirebaseException firebaseEx)
        {
            UpdateFeedback($"Đăng nhập gặp lỗi: sai mật khẩu");
            Debug.Log($"Firebase Error: {firebaseEx.Message}");
        }
        catch (System.Exception ex)
        {
            UpdateFeedback($"Đăng nhập gặp lỗi: sai mật khẩu");
            Debug.Log($"Error: {ex.Message}");
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
            var health = snapshot.Child("HealthCurrent").Value;
            var energy = snapshot.Child("EnergyCurrent").Value;
            var exp = snapshot.Child("Exp").Value;

            if (health != null && energy != null && exp != null)
            {
                int healthValue = int.Parse(health.ToString());
                int energyValue = int.Parse(energy.ToString());
                int expValue = int.Parse(exp.ToString());

                Debug.Log($"Player Data Loaded: {username}, Health: {healthValue}, Energy: {energyValue}, EXP: {expValue}");

                // Cập nhật lại UI hoặc các biến trong game (ví dụ, gán giá trị cho slider, text)
            }
            else
            {
                Debug.LogWarning("Một số dữ liệu người chơi bị thiếu.");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy dữ liệu cho người chơi: " + username);
        }
    }

    // Tải tên scene người chơi đang chơi từ Firebase
    private async Task LoadPlayerSceneFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists && snapshot.Child("Scene").Exists)
        {
            string sceneName = snapshot.Child("Scene").Value.ToString();
            Debug.Log("Tải lại scene: " + sceneName);
            SceneManager.LoadScene(sceneName); // Tải lại scene mà người chơi đã lưu trước đó
        }
        else
        {
            Debug.LogWarning("Không tìm thấy scene cho người chơi: " + username);
            // Nếu không tìm thấy scene đã lưu, có thể chuyển đến scene mặc định
            SceneManager.LoadScene(1);
        }
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
