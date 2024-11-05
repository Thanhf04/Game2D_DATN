using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public string username; // Tên người dùng
    public string avatar;   // Đường dẫn đến avatar
    public float health;    // Máu
    public float energy;    // Năng lượng
    public int gold;        // Vàng
    public int diamond;     // Kim cương
    public Vector3 position; // Vị trí
    public string skillID;  // ID kỹ năng
}

public class CharacterManager : MonoBehaviour
{
    public CharacterData characterData; // Dữ liệu nhân vật
    private DatabaseReference databaseReference; // Tham chiếu đến cơ sở dữ liệu
    private FirebaseAuth auth; // Tham chiếu đến Firebase Auth

    private async void Start()
    {
        // Khởi tạo Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync();

        // Khởi tạo Database Reference và Firebase Auth
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    public async void LoginUser(string username)
    {
        // Tạo dữ liệu nhân vật
        characterData = new CharacterData
        {
            username = username,
            avatar = "path/to/avatar.png", // Thay đường dẫn thực tế
            health = 100f,
            energy = 100f,
            gold = 500,
            diamond = 300,
            position = new Vector3(0, 0, 0),
            skillID = "skill_01"
        };

        await SaveCharacterData();
    }

    private async Task SaveCharacterData()
    {
        string jsonData = JsonUtility.ToJson(characterData);
        try
        {
            // Lưu dữ liệu lên Firebase
            var task = databaseReference.Child("characters").Child(characterData.username).SetRawJsonValueAsync(jsonData);
            await task;

            if (task.IsCompleted)
            {
                Debug.Log("Dữ liệu nhân vật đã được lưu lên Firebase!");
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

    private async void LoadCharacterData(string username)
    {
        try
        {
            var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
            if (snapshot.Exists)
            {
                characterData = JsonUtility.FromJson<CharacterData>(snapshot.GetRawJsonValue());
                Debug.Log($"Dữ liệu đã được tải: {characterData.username}, Máu: {characterData.health}");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy dữ liệu nhân vật!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi tải dữ liệu: " + ex.Message);
        }
    }
}
