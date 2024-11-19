using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ResetPlayerData : MonoBehaviour
{
    // Public để bạn kéo thả vào Inspector
    public Button resetButton;     // Nút reset
    public Slider healthSlider;    // Slider health
    public Slider energySlider;    // Slider energy
    public GameObject player;      // GameObject nhân vật
    public DatabaseReference databaseReference;

    // Các giá trị mặc định để reset
    public int startingHealth = 100;   // Giá trị health ban đầu
    public int startingEnergy = 100;    // Giá trị energy ban đầu
    public Vector3 resetPosition = new Vector3(-17.33f, 0.71f, 0.00f);  // Vị trí reset

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

        // Gán sự kiện cho nút reset
        resetButton.onClick.AddListener(ResetPlayerDataToDefaults);
    }

    // Hàm reset các giá trị về giá trị ban đầu
    private void ResetPlayerDataToDefaults()
    {
        // Reset các giá trị về giá trị ban đầu
        healthSlider.value = startingHealth;
        energySlider.value = startingEnergy;

        // Reset vị trí nhân vật về vị trí mới (-17.33, 0.71, 0.00)
        player.transform.position = resetPosition;

        // Lưu các giá trị mới vào Firebase
        SavePlayerData("HealthCurrent", startingHealth);
        SavePlayerData("EnergyCurrent", startingEnergy);

        // Lưu vị trí mới vào Firebase
        SavePlayerPosition(resetPosition);
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

    // Lưu vị trí của người chơi vào Firebase
    private async void SavePlayerPosition(Vector3 position)
    {
        if (string.IsNullOrEmpty(username)) return;

        try
        {
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
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving player position: {ex.Message}");
        }
    }
}
