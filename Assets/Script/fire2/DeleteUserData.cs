using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;

public class DeleteUserData : MonoBehaviour
{
    private DatabaseReference databaseReference;

    // Đặt tên của button và các trường cần thiết
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
        deleteButton.onClick.AddListener(DeleteUserData1);
    }

    void DeleteUserData1()
    {
        // Lấy username từ PlayerPrefs (dữ liệu lưu trong PlayerPrefs)
        string username = PlayerPrefs.GetString("username", "");  // "username" là khóa đã lưu trong PlayerPrefs

        if (!string.IsNullOrEmpty(username))
        {
            // Xóa toàn bộ dữ liệu của người chơi tại đường dẫn "players/{username}"
            databaseReference.Child("players").Child(username).RemoveValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Tất cả dữ liệu của người chơi đã được xóa.");
                }
                else
                {
                    Debug.LogError("Lỗi khi xóa dữ liệu của người chơi.");
                }
            });
        }
        else
        {
            Debug.LogWarning("Không có username lưu trong PlayerPrefs.");
        }
    }
}
