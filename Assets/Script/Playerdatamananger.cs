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
        // Kiểm tra và khởi tạo Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase khởi tạo thành công.");
            }
            else
            {
                Debug.LogError("Firebase không thể khởi tạo: " + task.Result);
            }
        });

        // Lấy username từ PlayerPrefs
        username = PlayerPrefs.GetString("username", "");

        // Gán các listener cho các slider
        healthSlider.onValueChanged.AddListener(UpdateHealth);
        energySlider.onValueChanged.AddListener(UpdateEnergy);
        expSlider.onValueChanged.AddListener(UpdateExp);

        // Tải dữ liệu người chơi từ Firebase ngay khi đăng nhập
        if (!string.IsNullOrEmpty(username))
        {
            // Tải tất cả dữ liệu người chơi từ Firebase
            await LoadPlayerDataFromFirebase(username);

            // Tải vị trí người chơi từ Firebase sau khi tải các dữ liệu khác
            await LoadPlayerPositionFromFirebase(username);
        }
        else
        {
            Debug.LogWarning("Username is empty. Cannot load player data.");
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
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            // Lấy và cập nhật giá trị Health
            int health = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());
            healthSlider.value = health;
            healthText.text = "Health: " + health.ToString();

            // Lấy và cập nhật giá trị Energy
            int energy = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());
            energySlider.value = energy;
            energyText.text = "Energy: " + energy.ToString();

            // Lấy và cập nhật giá trị EXP
            int exp = int.Parse(snapshot.Child("Exp").Value.ToString());
            expSlider.value = exp;
            expText.text = "EXP: " + exp.ToString();

            // Lấy và cập nhật giá trị Gold
            int gold = int.Parse(snapshot.Child("Gold").Value.ToString());
            this.gold = gold; // Cập nhật giá trị Gold
            goldText.text = "Gold: " + gold.ToString(); // Hiển thị Gold

            // Lấy và cập nhật giá trị Diamond
            int diamond = int.Parse(snapshot.Child("Diamond").Value.ToString());
            this.diamond = diamond; // Cập nhật giá trị Diamond
            diamondText.text = "Diamond: " + diamond.ToString(); // Hiển thị Diamond

            Debug.Log("Player data loaded from Firebase.");
        }
        else
        {
            Debug.LogWarning("No data found for player: " + username);
        }
    }

    // Tải vị trí của người chơi từ Firebase
    private async Task LoadPlayerPositionFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            if (snapshot.Child("Position").Exists)
            {
                // Lấy giá trị Position từ Firebase
                float positionX = float.Parse(snapshot.Child("Position").Child("PositionX").Value.ToString());
                float positionY = float.Parse(snapshot.Child("Position").Child("PositionY").Value.ToString());
                float positionZ = float.Parse(snapshot.Child("Position").Child("PositionZ").Value.ToString());

                // Debug: In ra các giá trị vị trí
                Debug.Log($"Loaded position: X={positionX}, Y={positionY}, Z={positionZ}");

                // Cập nhật vị trí của nhân vật trong game
                player.transform.position = new Vector3(positionX, positionY, positionZ);
                Debug.Log("Player position loaded from Firebase: " + player.transform.position);
            }
            else
            {
                Debug.LogWarning("No position data found for player: " + username);
            }
        }
        else
        {
            Debug.LogWarning("No data found for player: " + username);
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
