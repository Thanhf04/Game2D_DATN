using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

public class PlayerProfileManager1 : MonoBehaviour
{
    private DatabaseReference databaseReference;  // Firebase Database reference
    private string username;  // Lưu username của người chơi

    // Các đối tượng TMP_Text để hiển thị và chỉnh sửa thông tin người chơi
    public TMP_Text healthInfoText;   // Hiển thị và chỉnh sửa thông tin sức khỏe
    public TMP_Text manaInfoText;     // Hiển thị và chỉnh sửa thông tin mana
    public TMP_Text damageInfoText;   // Hiển thị và chỉnh sửa thông tin sát thương
    public Text totalScoreText;  // Tham chiếu tới đối tượng Text trong UI

    private int totalScore = 5;   // Tổng điểm ban đầu (bắt đầu từ 5)

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

        // Lắng nghe các thay đổi trong TMP_Text thủ công
        // Bạn sẽ phải cập nhật lại các giá trị khi người chơi tương tác với UI
    }

    private void Update()
    {
        // Kiểm tra và cập nhật tổng điểm mỗi khi thông tin sức khỏe, mana, hoặc damage thay đổi
        UpdateTotalScore();

        // Tự động lưu dữ liệu khi có thay đổi
        if (healthInfoText.text != "" && manaInfoText.text != "" && damageInfoText.text != "")
        {
            SavePlayerData();
        }
    }

    // Hàm tính lại tổng điểm (TotalScore) sau mỗi thay đổi
    private void UpdateTotalScore()
    {
        // Lấy các giá trị hiện tại từ TMP_Text
        int health = int.Parse(healthInfoText.text);
        int mana = int.Parse(manaInfoText.text);
        int damage = int.Parse(damageInfoText.text);

        // Cập nhật tổng điểm
        totalScore = 5 - (health + mana + damage);

        // Cập nhật giá trị vào TMP_Text cho TotalScore
        totalScoreText.text = " " + totalScore.ToString();
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
            int loadedTotalScore = snapshot.Child("TotalScore").Value != null ? int.Parse(snapshot.Child("TotalScore").Value.ToString()) : 5;

            // Cập nhật giá trị vào TMP_Text
            healthInfoText.text = loadedHealth.ToString();
            manaInfoText.text = loadedMana.ToString();
            damageInfoText.text = loadedDamage.ToString();

            // Cập nhật lại tổng điểm
            totalScore = loadedTotalScore;
            totalScoreText.text = " " + totalScore.ToString();

            Debug.Log($"Dữ liệu người chơi đã được tải thành công. Health: {loadedHealth}, Mana: {loadedMana}, Damage: {loadedDamage}, TotalScore: {totalScore}");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy dữ liệu cho người chơi: " + username);
        }
    }

    // Hàm lưu dữ liệu người chơi vào Firebase
    public async void SavePlayerData()
    {
        // Lấy giá trị từ TMP_Text
        int healthValue = int.Parse(healthInfoText.text);
        int manaValue = int.Parse(manaInfoText.text);
        int damageValue = int.Parse(damageInfoText.text);

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
