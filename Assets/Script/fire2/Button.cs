using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Để sử dụng SceneManager

public class DeleteUserData : MonoBehaviour
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
        deleteButton.onClick.AddListener(DeleteUserDataAction);
    }

    void DeleteUserDataAction()
    {
        // Lấy username từ PlayerPrefs (dữ liệu lưu trong PlayerPrefs)
        string username = PlayerPrefs.GetString("username", "");  // "username" là khóa đã lưu trong PlayerPrefs

        if (!string.IsNullOrEmpty(username))
        {
            // Xóa dữ liệu của player1 tại đường dẫn "players/player1"
            databaseReference.Child("players").Child(username).RemoveValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Dữ liệu của player1 đã được xóa.");

                    // Xóa dữ liệu của player2 tại đường dẫn "players/player2"
                    databaseReference.Child("player").Child(username).RemoveValueAsync().ContinueWithOnMainThread(subTask => {
                        if (subTask.IsCompleted)
                        {
                            Debug.Log("Dữ liệu của player2 đã được xóa.");

                            // Sau khi xóa dữ liệu, chuyển màn hình về scene "Player1"
                            LoadNewScene(); // Hàm chuyển màn hình
                        }
                        else
                        {
                            Debug.LogError("Lỗi khi xóa dữ liệu của player2.");
                        }
                    });
                }
                else
                {
                    Debug.LogError("Lỗi khi xóa dữ liệu của player1.");
                }
            });
        }
        else
        {
            Debug.LogWarning("Không có username lưu trong PlayerPrefs.");
        }
    }

    // Hàm chuyển màn hình
    void LoadNewScene()
    {
        // Chuyển đến scene "Player1"
        SceneManager.LoadScene("Player1");
    }
}
