using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;  // Thêm thư viện TMP_Text

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference reference;  // Tham chiếu đến Firebase Database
    public TMP_Text goldText;  // Tham chiếu đến TMP_Text trong UI

    private int currentGold = 0;  // Biến lưu trữ số vàng hiện tại
    private string userName;  // Biến lưu trữ tên người dùng từ PlayerPrefs

    void Start()
    {
        // Lấy username từ PlayerPrefs
        userName = PlayerPrefs.GetString("username", "");

        // Kiểm tra và khởi tạo Firebase khi ứng dụng bắt đầu
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully.");

                // Tải dữ liệu vàng khi vào game
                LoadTextFromFirebase();
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
        });
    }

    void Update()
    {
        // Lắng nghe sự kiện nhấn phím Y để lưu nội dung lên Firebase
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SaveTextToFirebase();
        }

        // Lắng nghe sự kiện nhấn phím Z để tải lại dữ liệu từ Firebase
        if (Input.GetKeyDown(KeyCode.Z))
        {
            LoadTextFromFirebase();
        }
    }

    // Lưu nội dung văn bản từ TMP_Text lên Firebase (với việc cộng thêm vàng)
    public void SaveTextToFirebase()
    {
        // Kiểm tra xem TMP_Text có tồn tại và có chứa văn bản không
        if (goldText != null && !string.IsNullOrEmpty(goldText.text))
        {
            if (string.IsNullOrEmpty(userName))
            {
                Debug.LogError("Username không hợp lệ.");
                return;
            }

            // Lấy giá trị vàng từ TMP_Text và lưu vào Firebase
            int newGoldAmount = int.Parse(goldText.text);

            // Lưu tổng số vàng lên Firebase
            reference.Child("players").Child(userName).Child("gold").SetValueAsync(newGoldAmount).ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Nội dung đã được lưu thành công vào Firebase. Tổng vàng: " + newGoldAmount);
                    currentGold = newGoldAmount;  // Cập nhật giá trị vàng hiện tại
                }
                else
                {
                    Debug.LogError("Lưu nội dung vào Firebase thất bại: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("TMP_Text không tồn tại hoặc không có nội dung.");
        }
    }

    // Tải nội dung văn bản từ Firebase và hiển thị vào TMP_Text
    public void LoadTextFromFirebase()
    {
        if (string.IsNullOrEmpty(userName))
        {
            Debug.LogError("Username không hợp lệ.");
            return;
        }

        // Lấy dữ liệu từ Firebase Realtime Database
        reference.Child("players").Child(userName).Child("gold").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    currentGold = int.Parse(snapshot.Value.ToString());  // Lấy giá trị vàng hiện tại từ Firebase
                    goldText.text = currentGold.ToString();  // Cập nhật TMP_Text với giá trị vàng
                    Debug.Log("Văn bản tải thành công từ Firebase: " + currentGold);
                }
                else
                {
                    // Nếu không có dữ liệu vàng, đặt số vàng mặc định là 0
                    currentGold = 0;
                    goldText.text = currentGold.ToString();
                    Debug.Log("Không tìm thấy dữ liệu vàng. Đặt số vàng mặc định là 0.");
                }
            }
            else
            {
                Debug.LogError("Tải văn bản từ Firebase thất bại: " + task.Exception);
            }
        });
    }

    // Gọi hàm này khi người chơi mua vật phẩm
    public void PurchaseItem(int itemCost)
    {
        if (currentGold >= itemCost)
        {
            // Trừ vàng khi mua vật phẩm
            currentGold -= itemCost;

            // Cập nhật lại TMP_Text
            goldText.text = currentGold.ToString();

            // Lưu số vàng mới vào Firebase
            SaveTextToFirebase();
            Debug.Log("Mua vật phẩm thành công, số vàng còn lại: " + currentGold);
        }
        else
        {
            Debug.Log("Không đủ vàng để mua vật phẩm!");
        }
    }
}
