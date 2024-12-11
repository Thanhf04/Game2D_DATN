using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public TMP_Text healthText; // Text để hiển thị điểm Health
    public TMP_Text manaText; // Text để hiển thị điểm Mana
    public TMP_Text damageText; // Text để hiển thị điểm Damage

    private DatabaseReference databaseReference;

    // Hàm này chạy khi bắt đầu, thiết lập Firebase và tải dữ liệu người chơi
    private async void Start()
    {
        // Kiểm tra và thiết lập Firebase
        await FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWith(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                    Debug.Log("Firebase khởi tạo thành công.");
                }
                else
                {
                    Debug.LogError("Lỗi khởi tạo Firebase: " + task.Result);
                }
            });

        // Lấy username từ PlayerPrefs (vì bạn đã lưu nó trong quá trình đăng nhập)
        string username = PlayerPrefs.GetString("username");

        if (!string.IsNullOrEmpty(username))
        {
            await LoadPlayerStatsFromFirebase(username); // Tải dữ liệu người chơi từ Firebase
        }
        else
        {
            Debug.LogWarning("Username không hợp lệ!");
        }
    }

    // Lưu dữ liệu người chơi lên Firebase
    public async Task SavePlayerStatsToFirebase(string username, int health, int mana, int damage)
    {
        // Tạo dictionary lưu các điểm số của người chơi
        var playerStats = new Dictionary<string, object>
        {
            { "HealthCurrent", health },
            { "ManaCurrent", mana },
            { "Damage", damage }
        };

        // Lưu vào Firebase
        await databaseReference.Child("players").Child(username).SetValueAsync(playerStats);
        Debug.Log("Player stats saved to Firebase.");
    }

    // Tải dữ liệu người chơi từ Firebase
    public async Task LoadPlayerStatsFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            var health = snapshot.Child("HealthCurrent").Value;
            var mana = snapshot.Child("ManaCurrent").Value;
            var damage = snapshot.Child("Damage").Value;

            if (health != null && mana != null && damage != null)
            {
                int healthValue = int.Parse(health.ToString());
                int manaValue = int.Parse(mana.ToString());
                int damageValue = int.Parse(damage.ToString());

                // Cập nhật thông tin lên UI
                healthText.text = "Health: " + healthValue.ToString();
                manaText.text = "Mana: " + manaValue.ToString();
                damageText.text = "Damage: " + damageValue.ToString();

                Debug.Log(
                    $"Player Data Loaded: {username}, Health: {healthValue}, Mana: {manaValue}, Damage: {damageValue}"
                );
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
}
