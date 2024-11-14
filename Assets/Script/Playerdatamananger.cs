using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerDataTest : MonoBehaviour
{
    public TMP_Text healthText; // Hiển thị giá trị health
    public TMP_Text energyText; // Hiển thị giá trị energy
    public TMP_Text expText;    // Hiển thị giá trị exp
    public TMP_Text goldText;   // Hiển thị giá trị gold
    public TMP_Text diamondText; // Hiển thị giá trị diamond
    public Slider healthSlider; // Slider cho health
    public Slider energySlider; // Slider cho energy
    public Slider expSlider;    // Slider cho exp
    public GameObject player;   // Nhân vật trong game

    // Các biến public cho phép bạn thay đổi giá trị Gold và Diamond từ Unity Editor
    public int gold = 0;
    public int diamond = 0;

    private DatabaseReference databaseReference;
    private string username;
    private Vector3 lastSavedPosition;

    private async void Start()
    {
        // Kiểm tra và thiết lập Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase khởi tạo thành công.");
            }
            else
            {
                Debug.LogError("Firebase không khởi tạo được: " + task.Result);
                return; // Dừng quá trình nếu Firebase không khởi tạo được
            }
        });

        // Lấy username từ PlayerPrefs
        username = PlayerPrefs.GetString("username", "");

        // Kiểm tra username và databaseReference trước khi tải dữ liệu
        if (!string.IsNullOrEmpty(username) && databaseReference != null)
        {
            // Tải dữ liệu người chơi từ Firebase
            await LoadPlayerDataFromFirebase(username);  // Load data từ Firebase
        }
        else
        {
            Debug.LogWarning("Username hoặc DatabaseReference chưa được khởi tạo.");
        }

        // Lưu vị trí ban đầu của người chơi
        lastSavedPosition = player.transform.position;
    }

    // Cập nhật các giá trị khi slider thay đổi
    private void UpdateHealth(float value)
    {
        healthText.text = "Health: " + value.ToString("F0");
        SavePlayerData("HealthCurrent", (int)value);
    }

    private void UpdateEnergy(float value)
    {
        energyText.text = "Energy: " + value.ToString("F0");
        SavePlayerData("EnergyCurrent", (int)value);
    }

    private void UpdateExp(float value)
    {
        expText.text = "EXP: " + value.ToString("F0");
        SavePlayerData("Exp", (int)value);
    }

    private void UpdateGold(int value)
    {
        goldText.text = "Gold: " + value.ToString();
        SavePlayerData("Gold", value); // Lưu giá trị Gold vào Firebase
    }

    private void UpdateDiamond(int value)
    {
        diamondText.text = "Diamond: " + value.ToString();
        SavePlayerData("Diamond", value); // Lưu giá trị Diamond vào Firebase
    }

    // Lưu dữ liệu người chơi vào Firebase
    private async void SavePlayerData(string field, int value)
    {
        if (string.IsNullOrEmpty(username)) return;

        // Lưu dữ liệu vào Firebase
        await databaseReference.Child("players").Child(username).Child(field).SetValueAsync(value);
        Debug.Log($"{field} saved to Firebase: {value}");
    }

    // Lưu vị trí của người chơi vào Firebase
    private async void SavePlayerPosition(Vector3 position)
    {
        if (string.IsNullOrEmpty(username)) return;

        // Tạo Dictionary chứa các thành phần Position (X, Y, Z)
        var positionDict = new Dictionary<string, object>
        {
            { "PositionX", position.x },
            { "PositionY", position.y },
            { "PositionZ", position.z }
        };

        // Lưu thông tin vị trí vào Firebase
        await databaseReference.Child("players").Child(username).Child("Position").SetValueAsync(positionDict);
        Debug.Log("Player position saved to Firebase: " + position);
    }

    // Tải lại tất cả dữ liệu người chơi từ Firebase
    private async Task LoadPlayerDataFromFirebase(string username)
    {
        if (databaseReference == null)
        {
            Debug.LogError("Database reference is not initialized.");
            return;
        }

        // Kiểm tra xem username có hợp lệ không
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is null or empty.");
            return;
        }

        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            // Kiểm tra sự tồn tại của các dữ liệu cần thiết
            var health = snapshot.Child("HealthCurrent").Value;
            var energy = snapshot.Child("EnergyCurrent").Value;
            var exp = snapshot.Child("Exp").Value;
            var gold = snapshot.Child("Gold").Value;
            var diamond = snapshot.Child("Diamond").Value;

            // Kiểm tra và chuyển đổi giá trị
            if (health != null && energy != null && exp != null && gold != null && diamond != null)
            {
                // Cập nhật các giá trị từ Firebase
                int healthValue = int.Parse(health.ToString());
                int energyValue = int.Parse(energy.ToString());
                int expValue = int.Parse(exp.ToString());
                this.gold = int.Parse(gold.ToString());
                this.diamond = int.Parse(diamond.ToString());

                // Cập nhật giá trị UI
                healthSlider.value = healthValue;
                energySlider.value = energyValue;
                expSlider.value = expValue;

                healthText.text = "Health: " + healthValue.ToString();
                energyText.text = "Energy: " + energyValue.ToString();
                expText.text = "EXP: " + expValue.ToString();
                goldText.text = "Gold: " + this.gold.ToString();
                diamondText.text = "Diamond: " + this.diamond.ToString();

                Debug.Log($"Player Data Loaded: {username}, Health: {healthValue}, Energy: {energyValue}, EXP: {expValue}, Gold: {this.gold}, Diamond: {this.diamond}");
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

    // Kiểm tra và lưu vị trí người chơi mỗi frame nếu có thay đổi
    private void Update()
    {
        // Kiểm tra nếu người chơi đã di chuyển, lưu vị trí mới vào Firebase
        if (player != null)
        {
            Vector3 currentPosition = player.transform.position;

            // Kiểm tra sự thay đổi vị trí
            if (Vector3.Distance(currentPosition, lastSavedPosition) > 0.1f) // Chỉ lưu khi có thay đổi vị trí
            {
                SavePlayerPosition(currentPosition);
                lastSavedPosition = currentPosition; // Cập nhật vị trí đã lưu
            }
        }
    }
}
