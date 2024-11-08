using System;
using System.Threading.Tasks;
using Firebase.Database;
using TMPro; // Thêm thư viện TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataManager : MonoBehaviour
{
    // Các tham chiếu đến UI components sử dụng TMP_Text
    public TMP_Text goldText;      // Hiển thị gold, sử dụng TMP_Text thay vì Text
    public TMP_Text diamondText;   // Hiển thị diamond, sử dụng TMP_Text thay vì Text
    public Image healthImage;      // Hiển thị health (Image)
    public Image energyImage;      // Hiển thị energy (Image)

    private DatabaseReference databaseReference;

    private void Start()
    {
        // Khởi tạo Firebase
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Phương thức lưu dữ liệu người chơi lên Firebase
    public async Task SavePlayerData(string username)
    {
        // Lấy các giá trị từ UI
        int gold = int.Parse(goldText.text);      // Giả sử giá trị gold là số nguyên từ TMP_Text
        int diamond = int.Parse(diamondText.text); // Giả sử giá trị diamond là số nguyên từ TMP_Text

        // Kiểm tra lại giá trị fillAmount của health và energy
        float health = healthImage.fillAmount * 100f; // Giả sử health được lưu dưới dạng phần trăm (0-100)
        float energy = energyImage.fillAmount * 100f; // Giả sử energy được lưu dưới dạng phần trăm (0-100)

        // In giá trị health và energy ra console để kiểm tra
        Debug.Log("Health FillAmount: " + healthImage.fillAmount);
        Debug.Log("Energy FillAmount: " + energyImage.fillAmount);
        Debug.Log("Health: " + health + "%");
        Debug.Log("Energy: " + energy + "%");

        // In giá trị username để kiểm tra xem có bị trống hoặc sai không
        Debug.Log("Username: " + username);

        // Tạo đối tượng chứa dữ liệu người chơi
        PlayerData playerData = new PlayerData
        {
            username = username,
            gold = gold,
            diamond = diamond,
            health = health,
            energy = energy
        };

        // Chuyển đối tượng PlayerData thành JSON
        string jsonData = JsonUtility.ToJson(playerData);

        // In ra dữ liệu JSON để kiểm tra xem nó đúng không
        Debug.Log("Dữ liệu JSON: " + jsonData);

        try
        {
            // Lưu dữ liệu vào Firebase (dưới mục "players/{username}")
            var task = databaseReference.Child("players").Child(username).SetRawJsonValueAsync(jsonData);
            await task;

            if (task.IsCompleted)
            {
                Debug.Log("Dữ liệu người chơi đã được lưu lên Firebase!");
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
}

[System.Serializable]
public class PlayerData
{
    public string username;
    public int gold;
    public int diamond;
    public float health;  // Health dưới dạng phần trăm (0-100)
    public float energy;  // Energy dưới dạng phần trăm (0-100)
}
