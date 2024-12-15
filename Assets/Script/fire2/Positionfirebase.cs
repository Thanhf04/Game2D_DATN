using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference reference;  // Reference to Firebase Realtime Database

    [Header("GameObject to Save/Load Position")]
    public GameObject gameObjectToSaveLoad;  // GameObject cần lưu và tải vị trí

    private string username;  // Tên người dùng để lưu và tải dữ liệu theo

    // Được gọi khi script bắt đầu, khởi tạo Firebase
    void Start()
    {
        // Lấy username từ PlayerPrefs hoặc bạn có thể lấy từ một nơi khác
        username = PlayerPrefs.GetString("username", "");

        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase Initialized!");
        });
    }

    // Lưu vị trí của GameObject lên Firebase
    public void SavePosition()
    {
        if (gameObjectToSaveLoad == null)
        {
            Debug.LogError("GameObject không hợp lệ!");
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không hợp lệ!");
            return;
        }

        // Lấy vị trí của GameObject
        Vector3 position = gameObjectToSaveLoad.transform.position;

        // Tạo đối tượng chứa dữ liệu vị trí
        PositionData positionData = new PositionData
        {
            x = position.x,
            y = position.y,
            z = position.z
        };

        // Chuyển đối tượng thành JSON
        string json = JsonUtility.ToJson(positionData);

        // Lưu dữ liệu vào Firebase
        reference.Child("players").Child(username).Child("position").SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Vị trí đã được lưu lên Firebase!");
                }
                else
                {
                    Debug.LogError("Lỗi khi lưu vị trí lên Firebase: " + task.Exception);
                }
            });
    }

    // Tải vị trí của GameObject từ Firebase
    public void LoadPosition()
    {
        if (gameObjectToSaveLoad == null)
        {
            Debug.LogError("GameObject không hợp lệ!");
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không hợp lệ!");
            return;
        }

        // Tải dữ liệu vị trí từ Firebase
        reference.Child("players").Child(username).Child("position").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Xử lý lỗi
                Debug.LogError("Lỗi khi tải dữ liệu vị trí từ Firebase");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // Chuyển dữ liệu từ Firebase thành đối tượng PositionData
                    PositionData positionData = JsonUtility.FromJson<PositionData>(snapshot.GetRawJsonValue());
                    gameObjectToSaveLoad.transform.position = new Vector3(positionData.x, positionData.y, positionData.z);
                    Debug.Log("Vị trí đã được tải từ Firebase!");
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy dữ liệu vị trí của người chơi trong Firebase.");
                }
            }
        });
    }
}

// Lớp dữ liệu vị trí
[System.Serializable]
public class PositionData
{
    public float x;
    public float y;
    public float z;
}
