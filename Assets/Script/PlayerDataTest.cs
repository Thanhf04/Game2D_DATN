using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;  // Để quản lý việc chuyển cảnh
using UnityEngine.UI;

public class PlayerDataTest : MonoBehaviour
{
    public TMP_Text healthText;  // Hiển thị giá trị health
    public TMP_Text energyText;  // Hiển thị giá trị energy
    public TMP_Text expText;     // Hiển thị giá trị exp
    public TMP_Text goldText;    // Hiển thị giá trị gold
    public TMP_Text diamondText; // Hiển thị giá trị diamond
    public Slider healthSlider;  // Slider cho health
    public Slider energySlider;  // Slider cho energy
    public Slider expSlider;     // Slider cho exp
    public GameObject player;    // Nhân vật trong game

    public int gold = 0;
    public int diamond = 0;

    private DatabaseReference databaseReference;
    private string username;
    private Vector3 lastSavedPosition;

    private async void Start()
    {
        // Kiểm tra và khởi tạo Firebase
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Debug.LogError("Firebase không thể khởi tạo: " + dependencyStatus);
            return;
        }

        // Lấy username từ PlayerPrefs
        username = PlayerPrefs.GetString("username", "");

        // Kiểm tra username hợp lệ
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is empty. Cannot load player data.");
            return;
        }

        // Kiểm tra player
        if (player == null)
        {
            Debug.LogError("Player GameObject is null. Cannot track position.");
            return;
        }

        // Gán các listener cho các slider
        if (healthSlider != null) healthSlider.onValueChanged.AddListener(UpdateHealth);
        if (energySlider != null) energySlider.onValueChanged.AddListener(UpdateEnergy);
        if (expSlider != null) expSlider.onValueChanged.AddListener(UpdateExp);

        // Lấy dữ liệu người chơi từ Firebase khi game bắt đầu
        await LoadPlayerDataFromFirebase(username);
        await LoadPlayerPositionFromFirebase(username);

        // Lưu vị trí ban đầu của người chơi
        lastSavedPosition = player.transform.position;
    }

    // Cập nhật giá trị khi slider thay đổi
    private void UpdateHealth(float value)
    {
        if (healthText != null)
            healthText.text = "Health: " + value.ToString("F0");

        SavePlayerData("HealthCurrent", (int)value);  // Lưu giá trị health vào Firebase
    }

    private void UpdateEnergy(float value)
    {
        if (energyText != null)
            energyText.text = "Energy: " + value.ToString("F0");

        SavePlayerData("EnergyCurrent", (int)value);  // Lưu giá trị energy vào Firebase
    }

    private void UpdateExp(float value)
    {
        if (expText != null)
            expText.text = "EXP: " + value.ToString("F0");

        SavePlayerData("Exp", (int)value);  // Lưu giá trị exp vào Firebase
    }

    private void UpdateGold(int value)
    {
        if (goldText != null)
            goldText.text = "Gold: " + value.ToString();

        SavePlayerData("Gold", value); // Lưu giá trị Gold vào Firebase
    }

    private void UpdateDiamond(int value)
    {
        if (diamondText != null)
            diamondText.text = "Diamond: " + value.ToString();

        SavePlayerData("Diamond", value); // Lưu giá trị Diamond vào Firebase
    }

    // Lưu dữ liệu người chơi vào Firebase
    private async void SavePlayerData(string field, int value)
    {
        if (string.IsNullOrEmpty(username) || databaseReference == null)
        {
            Debug.LogError("SavePlayerData failed: Username is empty or Firebase is not initialized.");
            return;
        }

        await databaseReference.Child("players").Child(username).Child(field).SetValueAsync(value);
    }

    // Lưu vị trí của người chơi vào Firebase
    private async void SavePlayerPosition(Vector3 position)
    {
        if (string.IsNullOrEmpty(username) || databaseReference == null || player == null) return;

        var positionDict = new Dictionary<string, object>
        {
            { "PositionX", position.x },
            { "PositionY", position.y },
            { "PositionZ", position.z }
        };

        await databaseReference.Child("players").Child(username).Child("Position").SetValueAsync(positionDict);
    }

    // Lưu tên scene hiện tại vào Firebase
    private async void SavePlayerScene(string sceneName)
    {
        if (string.IsNullOrEmpty(username) || databaseReference == null) return;

        await databaseReference.Child("players").Child(username).Child("Scene").SetValueAsync(sceneName);
    }

    // Tải lại tất cả dữ liệu người chơi từ Firebase
    private async Task LoadPlayerDataFromFirebase(string username)
    {
        if (databaseReference == null || string.IsNullOrEmpty(username))
        {
            Debug.LogError("Database reference or username is null. Cannot load player data.");
            return;
        }

        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            // Lấy và cập nhật giá trị Health
            if (snapshot.Child("HealthCurrent").Exists)
            {
                int health = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());
                if (healthSlider != null)
                    healthSlider.value = health;
                if (healthText != null)
                    healthText.text = "Health: " + health.ToString();
            }

            // Lấy và cập nhật giá trị Energy
            if (snapshot.Child("EnergyCurrent").Exists)
            {
                int energy = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());
                if (energySlider != null)
                    energySlider.value = energy;
                if (energyText != null)
                    energyText.text = "Energy: " + energy.ToString();
            }

            // Lấy và cập nhật giá trị EXP
            if (snapshot.Child("Exp").Exists)
            {
                int exp = int.Parse(snapshot.Child("Exp").Value.ToString());
                if (expSlider != null)
                    expSlider.value = exp;
                if (expText != null)
                    expText.text = "EXP: " + exp.ToString();
            }

            // Lấy và cập nhật giá trị Gold
            if (snapshot.Child("Gold").Exists)
            {
                int gold = int.Parse(snapshot.Child("Gold").Value.ToString());
                this.gold = gold;
                if (goldText != null)
                    goldText.text = "Gold: " + gold.ToString();
            }

            // Lấy và cập nhật giá trị Diamond
            if (snapshot.Child("Diamond").Exists)
            {
                int diamond = int.Parse(snapshot.Child("Diamond").Value.ToString());
                this.diamond = diamond;
                if (diamondText != null)
                    diamondText.text = "Diamond: " + diamond.ToString();
            }
        }
        else
        {
            Debug.LogWarning("No data found for player: " + username);
        }
    }

    // Tải vị trí của người chơi từ Firebase
    private async Task LoadPlayerPositionFromFirebase(string username)
    {
        if (databaseReference == null || string.IsNullOrEmpty(username) || player == null)
        {
            Debug.LogError("Database reference, username or player object is null. Cannot load player position.");
            return;
        }

        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists && snapshot.Child("Position").Exists)
        {
            // Lấy giá trị Position từ Firebase
            float positionX = float.Parse(snapshot.Child("Position").Child("PositionX").Value.ToString());
            float positionY = float.Parse(snapshot.Child("Position").Child("PositionY").Value.ToString());
            float positionZ = float.Parse(snapshot.Child("Position").Child("PositionZ").Value.ToString());

            // Cập nhật vị trí của nhân vật trong game
            player.transform.position = new Vector3(positionX, positionY, positionZ);
        }
        else
        {
            Debug.LogWarning("No position data found for player: " + username);
        }
    }

    // Kiểm tra và lưu vị trí người chơi mỗi frame nếu có thay đổi
    private void Update()
    {
        if (player != null)
        {
            Vector3 currentPosition = player.transform.position;

            // Kiểm tra sự thay đổi vị trí
            if (Vector3.Distance(currentPosition, lastSavedPosition) > 0.1f) // Chỉ lưu khi có thay đổi vị trí
            {
                SavePlayerPosition(currentPosition);
                lastSavedPosition = currentPosition; // Cập nhật vị trí đã lưu
            }

            // Lưu tên scene hiện tại vào Firebase khi chuyển màn hình
            SavePlayerScene(SceneManager.GetActiveScene().name);
        }
    }
}
