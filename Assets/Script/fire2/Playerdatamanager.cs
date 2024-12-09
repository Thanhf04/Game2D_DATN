using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerDataManager : MonoBehaviour
{
    public TMP_Text goldText;     // Hiển thị giá trị gold
    public TMP_Text diamondText;  // Hiển thị giá trị diamond
    public Slider healthSlider;   // Slider cho health
    public Slider energySlider;   // Slider cho energy
    public Slider expSlider;      // Slider cho exp
    public GameObject player;     // Nhân vật trong game

    // Các biến để lưu trữ giá trị
    private int gold = 0;         // Biến gold
    private int diamond = 0;      // Biến diamond
    private DatabaseReference databaseReference;
    private string username;

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

        // Lấy dữ liệu người chơi từ Firebase sau khi đăng nhập
        if (!string.IsNullOrEmpty(username))
        {
            // Tải tất cả dữ liệu người chơi từ Firebase
            await LoadPlayerDataFromFirebase(username);
        }
        else
        {
            Debug.LogWarning("Username is empty. Cannot load player data.");
        }
    }

    // Cập nhật các giá trị khi slider thay đổi
    private void UpdateHealth(float value)
    {
        SavePlayerData("HealthCurrent", (int)value);
    }

    private void UpdateEnergy(float value)
    {
        SavePlayerData("EnergyCurrent", (int)value);
    }

    private void UpdateExp(float value)
    {
        SavePlayerData("Exp", (int)value);
    }

    // Cập nhật giá trị Gold
    private void UpdateGold(int value)
    {
        gold = value;
        goldText.text = "Gold: " + gold.ToString(); // Hiển thị Gold
        SavePlayerData("Gold", value); // Lưu giá trị Gold vào Firebase
    }

    // Cập nhật giá trị Diamond
    private void UpdateDiamond(int value)
    {
        diamond = value;
        diamondText.text = "Diamond: " + diamond.ToString(); // Hiển thị Diamond
        SavePlayerData("Diamond", value); // Lưu giá trị Diamond vào Firebase
    }

    // Lưu dữ liệu người chơi vào Firebase
    private async void SavePlayerData(string field, int value)
    {
        if (string.IsNullOrEmpty(username)) return;

        try
        {
            // Lưu dữ liệu vào Firebase
            await databaseReference.Child("players").Child(username).Child(field).SetValueAsync(value);
            Debug.Log($"{field} saved to Firebase: {value}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving {field}: {ex.Message}");
        }
    }

    // Tải lại tất cả dữ liệu người chơi từ Firebase
    private async Task LoadPlayerDataFromFirebase(string username)
    {
        // Kiểm tra xem username có hợp lệ không
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Username is null or empty.");
            return;
        }

        // Kiểm tra nếu databaseReference đã được khởi tạo
        if (databaseReference == null)
        {
            Debug.LogError("Database reference is null. Firebase may not have been initialized properly.");
            return;
        }

        try
        {
            var playerRef = databaseReference.Child("players").Child(username);
            var snapshot = await playerRef.GetValueAsync();

            // Kiểm tra xem snapshot có tồn tại dữ liệu không
            if (snapshot.Exists)
            {
                // Lấy và cập nhật giá trị Health
                if (snapshot.Child("HealthCurrent").Value != null)
                {
                    int health = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());
                    healthSlider.value = health;
                }

                // Lấy và cập nhật giá trị Energy
                if (snapshot.Child("EnergyCurrent").Value != null)
                {
                    int energy = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());
                    energySlider.value = energy;
                }

                // Lấy và cập nhật giá trị EXP
                if (snapshot.Child("Exp").Value != null)
                {
                    int exp = int.Parse(snapshot.Child("Exp").Value.ToString());
                    expSlider.value = exp;
                }

                // Lấy và cập nhật giá trị Gold
                if (snapshot.Child("Gold").Value != null)
                {
                    gold = int.Parse(snapshot.Child("Gold").Value.ToString());
                    goldText.text = "Gold: " + gold.ToString(); // Hiển thị Gold
                }

                // Lấy và cập nhật giá trị Diamond
                if (snapshot.Child("Diamond").Value != null)
                {
                    diamond = int.Parse(snapshot.Child("Diamond").Value.ToString());
                    diamondText.text = "Diamond: " + diamond.ToString(); // Hiển thị Diamond
                }

                Debug.Log("Player data loaded from Firebase.");
            }
            else
            {
                Debug.LogWarning("No data found for player: " + username);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading player data: " + ex.Message);
        }
    }
}
