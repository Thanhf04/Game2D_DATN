using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerProfileManager : MonoBehaviour
{
    private DatabaseReference databaseReference;  // Firebase Database reference
    private string username;  // Lưu username của người chơi


    // Các giá trị cần lưu vào Firebase
    public int playerHealth = 0;  // Giá trị mặc định
    public int playerMana = 0;    // Giá trị mặc định
    public int playerDamage = 0;  // Giá trị mặc định
    private int totalScore = 5;   // Tổng điểm ban đầu

    private void Start()
    {
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
            SavePlayerData(playerHealth, playerMana, playerDamage);
        }

        // Kiểm tra khi bấm phím Z (tải lại dữ liệu từ Firebase)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            LoadPlayerData();
        }

        // Tăng giá trị các trường khi bấm phím
        if (Input.GetKeyDown(KeyCode.Alpha1))  // Tăng Health
        {
            playerHealth++;
            UpdateTotalScore();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))  // Tăng Mana
        {
            playerMana++;
            UpdateTotalScore();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))  // Tăng Damage
        {
            playerDamage++;
            UpdateTotalScore();
        }
    }

    // Hàm tính lại tổng điểm (TotalScore) sau mỗi thay đổi
    private void UpdateTotalScore()
    {
        totalScore = 5 - (playerHealth + playerMana + playerDamage); // Tổng điểm = 5 - tổng các giá trị
    }

    // Hàm tải dữ liệu người chơi từ Firebase và cập nhật giá trị
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
            int loadedHealth = snapshot.Child("PlayerHealth").Value != null ? int.Parse(snapshot.Child("PlayerHealth").Value.ToString()) : 0;
            int loadedMana = snapshot.Child("PlayerMana").Value != null ? int.Parse(snapshot.Child("PlayerMana").Value.ToString()) : 0;
            int loadedDamage = snapshot.Child("PlayerDamage").Value != null ? int.Parse(snapshot.Child("PlayerDamage").Value.ToString()) : 0;

            // Cập nhật các giá trị
            playerHealth = loadedHealth;
            playerMana = loadedMana;
            playerDamage = loadedDamage;

            // Cập nhật lại tổng điểm
            UpdateTotalScore();

            Debug.Log($"Dữ liệu người chơi đã được tải thành công. Health: {playerHealth}, Mana: {playerMana}, Damage: {playerDamage}, TotalScore: {totalScore}");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy dữ liệu cho người chơi: " + username);
        }
    }

    // Hàm lưu dữ liệu người chơi vào Firebase
    public async void SavePlayerData(int healthValue, int manaValue, int damageValue)
    {
        // Tính tổng điểm mới
        UpdateTotalScore();

        var playerData = new Dictionary<string, object>
        {
            { "PlayerHealth", healthValue },
            { "PlayerMana", manaValue },
            { "PlayerDamage", damageValue },
            { "TotalScore", totalScore }
        };

        // Lưu dữ liệu vào Firebase Realtime Database
        await databaseReference.Child("players").Child(username).UpdateChildrenAsync(playerData);

        Debug.Log($"Dữ liệu người chơi đã được lưu vào Firebase. Health: {healthValue}, Mana: {manaValue}, Damage: {damageValue}, TotalScore: {totalScore}");
    }
}
