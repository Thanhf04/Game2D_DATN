using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerProfileUI : MonoBehaviour
{
    // Các đối tượng TMP_Text để hiển thị thông tin người chơi
    public TMP_Text healthInfoText;   // Hiển thị thông tin sức khỏe
    public TMP_Text manaInfoText;     // Hiển thị thông tin mana
    public TMP_Text damageInfoText;   // Hiển thị thông tin sát thương

    private DatabaseReference databaseReference;  // Firebase Database reference
    private string username;  // Lưu username của người chơi

    private void Start()
    {
        // Kiểm tra các đối tượng TMP_Text
        if (healthInfoText == null || manaInfoText == null || damageInfoText == null)
        {
            Debug.LogError("Một trong các đối tượng TMP_Text chưa được gán trong Inspector!");
            return;
        }

        // Lấy username từ PlayerPrefs (sau khi đăng nhập thành công)
        username = PlayerPrefs.GetString("username", "");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Không tìm thấy username trong PlayerPrefs!");
            return;
        }

        // Kiểm tra Firebase đã sẵn sàng chưa
        if (FirebaseApp.CheckAndFixDependenciesAsync().Result != Firebase.DependencyStatus.Available)
        {
            Debug.LogError("Firebase không thể khởi tạo!");
            return;
        }

        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Tải dữ liệu người chơi từ Firebase khi bắt đầu
        LoadPlayerData();
    }

    private void Update()
    {
        // Kiểm tra khi bấm phím Y (lưu dữ liệu lên Firebase)
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SavePlayerData(healthInfoText.text, manaInfoText.text, damageInfoText.text);
        }

        // Kiểm tra khi bấm phím Z (tải lại dữ liệu từ Firebase)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            LoadPlayerData();
        }
    }

    // Hàm tải dữ liệu người chơi từ Firebase và cập nhật UI
    private async void LoadPlayerData()
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không hợp lệ!");
            return;
        }

        // Tạo reference đến dữ liệu người chơi trong Firebase
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            Debug.Log("Dữ liệu Firebase tồn tại.");

            // Kiểm tra sự tồn tại của các trường trước khi truy cập
            var health = snapshot.Child("Health").Value != null ? snapshot.Child("Health").Value.ToString() : "Không có dữ liệu";
            var mana = snapshot.Child("Mana").Value != null ? snapshot.Child("Mana").Value.ToString() : "Không có dữ liệu";
            var damage = snapshot.Child("Damage").Value != null ? snapshot.Child("Damage").Value.ToString() : "Không có dữ liệu";

            // Cập nhật text trên UI
            healthInfoText.text = "Health: " + health;
            manaInfoText.text = "Mana: " + mana;
            damageInfoText.text = "Damage: " + damage;

            Debug.Log("Dữ liệu người chơi đã được tải thành công.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy dữ liệu cho người chơi: " + username);
        }
    }

    // Hàm lưu dữ liệu người chơi vào Firebase
    public async void SavePlayerData(string newHealth, string newMana, string newDamage)
    {
        var playerData = new Dictionary<string, object>
        {
            { "Health", newHealth },
            { "Mana", newMana },
            { "Damage", newDamage }
        };

        // Lưu dữ liệu vào Firebase Realtime Database
        await databaseReference.Child("players").Child(username).UpdateChildrenAsync(playerData);

        Debug.Log("Dữ liệu người chơi đã được lưu vào Firebase.");
    }
}
