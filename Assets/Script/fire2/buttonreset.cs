using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;  // Để sử dụng UI Button

public class DeletePlayerLevel : MonoBehaviour
{
    private DatabaseReference databaseReference;

    // Khai báo Button để liên kết với UI
    public Button deleteButton;

    void Start()
    {
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase đã được khởi tạo thành công.");
            }
            else
            {
                Debug.LogError("Lỗi khi khởi tạo Firebase.");
            }
        });

        // Đăng ký sự kiện click cho nút delete
        deleteButton.onClick.AddListener(DeletePlayerLevelData);
    }

    void DeletePlayerLevelData()
    {
        // Lấy username từ PlayerPrefs (dữ liệu lưu trong PlayerPrefs)
        string username = PlayerPrefs.GetString("username", "");  // "username" là khóa đã lưu trong PlayerPrefs

        if (!string.IsNullOrEmpty(username))
        {
            // Xóa trường "level" của người chơi tại đường dẫn "players/{username}/level"
            databaseReference.Child("player").Child(username).Child("currentHealth").RemoveValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Trường 'level' của người chơi đã được xóa.");
                }
                else
                {
                    Debug.LogError("Lỗi khi xóa trường 'level'.");
                }
            });
        }
        else
        {
            Debug.LogWarning("Không có username lưu trong PlayerPrefs.");
        }
    }
}
