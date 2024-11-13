using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class TestSavePlayerData : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Feedback text để hiển thị kết quả

    private FirebaseDatabase database;
    private string username = "testUser";  // Tên người chơi để lưu dữ liệu

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            database = FirebaseDatabase.GetInstance(app);
        });
    }

    // Hàm để lưu dữ liệu test lên Firebase
    public void SaveTestData()
    {
        var playerData = new System.Collections.Generic.Dictionary<string, object>
        {
            { "diamond", 100 },
            { "gold", 50 },
            { "exp", 30 },
            { "healthMax", 200 },
            { "energyMax", 100 },
            { "position", new System.Collections.Generic.Dictionary<string, object>
                {
                    { "x", 10f },
                    { "y", 0f },
                    { "z", 5f }
                }
            }
        };

        database.RootReference.Child("users").Child(username).Child("PlayerData").SetValueAsync(playerData)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    feedbackText.text = "Dữ liệu đã được lưu thành công!";
                    Debug.Log("Dữ liệu đã được lưu thành công!");
                }
                else
                {
                    feedbackText.text = "Lỗi khi lưu dữ liệu!";
                    Debug.LogError("Lỗi khi lưu dữ liệu: " + task.Exception);
                }
            });
    }

    // Hàm để tải dữ liệu người chơi
    public void LoadTestData()
    {
        var userRef = database.RootReference.Child("users").Child(username).Child("PlayerData");

        userRef.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    feedbackText.text = "Dữ liệu đã tải thành công!";
                    Debug.Log("Dữ liệu tải thành công.");
                }
                else
                {
                    feedbackText.text = "Không có dữ liệu!";
                    Debug.Log("Không tìm thấy dữ liệu.");
                }
            }
            else
            {
                feedbackText.text = "Lỗi khi tải dữ liệu!";
                Debug.LogError("Lỗi khi tải dữ liệu: " + task.Exception);
            }
        });
    }
}
