using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class ResetButtonHandler : MonoBehaviour
{
    public Button resetButton;  // Button để reset giá trị
    public Slider healthSlider; // Slider cho health
    public Slider energySlider; // Slider cho energy
    public GameObject player;   // Nhân vật trong game
    private FirebaseManager1 firebaseManager;
    public Button tryagain; // Nút bấm để phục hồi đầy máu
    Dichuyennv1 player1;
    private void Start()
    {
        firebaseManager.LoadPlayerData(OnPlayerDataLoaded);
        // Gán sự kiện khi nhấn nút reset
        resetButton.onClick.AddListener(ResetPlayerValues);
    }
    private void OnPlayerDataLoaded(FirebaseManager1.PlayerData playerData)
    {
        if (playerData != null)
        {
            // Cập nhật các thông tin về sức khỏe, mana và các vật phẩm
            player1.currentHealth = playerData.currentHealth;
            player1.currentMana = playerData.currentMana;
            player1.maxHealth = playerData.maxHealth;
            player1.maxMana = playerData.maxMana;

            // Giả sử bạn có cách lưu trữ các vật phẩm từ PlayerData vào kho (items).
            // Cập nhật dữ liệu các vật phẩm
            // Ví dụ:
            // AddItem(item, quantity); (Đảm bảo bạn có phương thức để thêm item từ PlayerData)
        }
        else
        {
            Debug.LogWarning("Player data not found or loading failed.");
        }
    }
    // Reset các giá trị về ban đầu
    private async void ResetPlayerValues()
    {
        // Vị trí reset
        Vector3 newPosition = new Vector3(-17.614965438842773f, 0.713690459728241f, 0f);
        player.transform.position = newPosition;

        // Reset lại slider health và energy về giá trị 100
        healthSlider.value = 100;
        energySlider.value = 100;

        // Lưu các giá trị vào Firebase
        await SavePlayerPosition(newPosition);
        await SavePlayerData("HealthCurrent", 100);
        await SavePlayerData("EnergyCurrent", 100);

        Debug.Log("Player values reset and saved to Firebase.");
    }

    // Lưu dữ liệu người chơi vào Firebase
    private async Task SavePlayerData(string field, int value)
    {
        var databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        string username = PlayerPrefs.GetString("username", "");

        if (string.IsNullOrEmpty(username)) return;

        try
        {
            await databaseReference.Child("players").Child(username).Child(field).SetValueAsync(value);
            Debug.Log($"{field} saved to Firebase: {value}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving {field}: {ex.Message}");
        }
    }

    // Lưu vị trí của người chơi vào Firebase
    private async Task SavePlayerPosition(Vector3 position)
    {
        var databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        string username = PlayerPrefs.GetString("username", "");

        if (string.IsNullOrEmpty(username)) return;

        try
        {
            var positionDict = new Dictionary<string, object>
            {
                { "PositionX", position.x },
                { "PositionY", position.y },
                { "PositionZ", position.z }
            };

            await databaseReference.Child("players").Child(username).Child("Position").SetValueAsync(positionDict);
            Debug.Log("Player position saved to Firebase: " + position);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving player position: {ex.Message}");
        }
    }
}
