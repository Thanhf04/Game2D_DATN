using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerDataTest : MonoBehaviour
{
    public TMP_Text healthText; // Hiển thị giá trị health
    public TMP_Text energyText; // Hiển thị giá trị energy
    public TMP_Text expText;    // Hiển thị giá trị exp
    public TMP_Text goldText;   // Hiển thị giá trị gold
    public TMP_Text diamondText; // Hiển thị giá trị diamond
    public Slider healthSlider; // Slider cho health
    public Slider energySlider; // Slider cho energy
    public Slider expSlider;    // Slider cho exp
    public GameObject player;   // Nhân vật trong game
    public GameObject inventoryCanvas;  // GameObject chứa UI cho inventory hoặc túi đồ

    // Các biến để lưu trữ giá trị
    public int gold = 0;         // Biến gold
    public int diamond = 0;      // Biến diamond

    private DatabaseReference databaseReference;
    private string username;
    private Vector3 lastSavedPosition;

    private async void Start()
    {
        // Kiểm tra và khởi tạo Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase khởi tạo thành công.");
            }
            else
            {
                Debug.LogError("Firebase không thể khởi tạo: " + task.Result);
            }
        });

        // Lấy username từ PlayerPrefs
        username = PlayerPrefs.GetString("username", "");

        // Gán các listener cho các slider
        healthSlider.onValueChanged.AddListener(UpdateHealth);
        energySlider.onValueChanged.AddListener(UpdateEnergy);
        expSlider.onValueChanged.AddListener(UpdateExp);

        // Lấy dữ liệu người chơi từ Firebase sau khi đăng nhập
        if (!string.IsNullOrEmpty(username))
        {
            // Tải tất cả dữ liệu người chơi từ Firebase
            await LoadPlayerDataFromFirebase(username);

            // Tải vị trí người chơi từ Firebase sau khi tải các dữ liệu khác
            await LoadPlayerPositionFromFirebase(username);

            // Tải dữ liệu từ inventoryCanvas nếu có
            await LoadInventoryDataFromFirebase(username);
        }
        else
        {
            Debug.LogWarning("Username is empty. Cannot load player data.");
        }

        // Lưu vị trí ban đầu của người chơi
        lastSavedPosition = player.transform.position;

        // Bắt đầu theo dõi sự thay đổi vị trí của nhân vật
        StartCoroutine(CheckAndSavePlayerPosition());
    }

    // Hàm kiểm tra và lưu vị trí nhân vật
    private System.Collections.IEnumerator CheckAndSavePlayerPosition()
    {
        while (true)
        {
            // Kiểm tra nếu vị trí đã thay đổi
            if (player.transform.position != lastSavedPosition)
            {
                // Lưu vị trí mới vào Firebase
                SavePlayerPosition(player.transform.position);

                // Cập nhật vị trí đã lưu
                lastSavedPosition = player.transform.position;
            }

            // Dừng trong một thời gian trước khi kiểm tra lại
            yield return new WaitForSeconds(1f);  // Kiểm tra mỗi giây
        }
    }

    // Cập nhật các giá trị khi slider thay đổi
    private void UpdateHealth(float value)
    {
        healthText.text = " " + value.ToString("F0");
        SavePlayerData("HealthCurrent", (int)value);
    }

    private void UpdateEnergy(float value)
    {
        energyText.text = ": " + value.ToString("F0");
        SavePlayerData("EnergyCurrent", (int)value);
    }

    private void UpdateExp(float value)
    {
        expText.text = ": " + value.ToString("F0");
        SavePlayerData("Exp", (int)value);
    }

    private void UpdateGold(int value)
    {
        gold = value;  // Cập nhật giá trị gold
        goldText.text = " " + gold.ToString();  // Cập nhật UI
        Debug.Log($"Gold value changed: {gold}");
        SavePlayerData("Gold", gold); // Lưu giá trị Gold vào Firebase
    }

    private void UpdateDiamond(int value)
    {
        diamond = value;  // Cập nhật giá trị diamond
        diamondText.text = " " + diamond.ToString();  // Cập nhật UI
        Debug.Log($"Diamond value changed: {diamond}");
        SavePlayerData("Diamond", diamond); // Lưu giá trị Diamond vào Firebase
    }

    // Lưu dữ liệu người chơi vào Firebase
    private async void SavePlayerData(string field, int value)
    {
        if (string.IsNullOrEmpty(username)) return;

        try
        {
            // Lưu dữ liệu vào Firebase
            await databaseReference.Child("players").Child(username).Child(field).SetValueAsync(value);
            Debug.Log($"{field} saved to Firebase: {value}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving {field}: {ex.Message}");
        }
    }

    // Lưu vị trí của người chơi vào Firebase
    private async void SavePlayerPosition(Vector3 position)
    {
        if (string.IsNullOrEmpty(username)) return;

        try
        {
            // Tạo Dictionary chứa các thành phần Position (X, Y, Z)
            var positionDict = new Dictionary<string, object>
            {
                { "PositionX", position.x },
                { "PositionY", position.y },
                { "PositionZ", position.z }
            };

            // Lưu thông tin vị trí vào Firebase
            await databaseReference.Child("players").Child(username).Child("Position").SetValueAsync(positionDict);
            Debug.Log("Player position saved to Firebase: " + position);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving player position: {ex.Message}");
        }
    }

    // Lưu thông tin Inventory vào Firebase
    private async void SaveInventoryData()
    {
        if (string.IsNullOrEmpty(username)) return;

        try
        {
            // Lấy tất cả các Canvas con (item) trong inventoryCanvas
            List<string> inventoryItems = new List<string>();
            foreach (Transform item in inventoryCanvas.transform)
            {
                if (item.gameObject.activeSelf) // Chỉ lưu những item đang được kích hoạt
                {
                    // Lấy tên hoặc ID của item để lưu trữ
                    inventoryItems.Add(item.gameObject.name);
                }
            }

            // Lưu thông tin inventory vào Firebase
            await databaseReference.Child("players").Child(username).Child("Inventory").SetValueAsync(inventoryItems);
            Debug.Log("Inventory data saved to Firebase.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving inventory data: {ex.Message}");
        }
    }

    // Tải lại thông tin Inventory từ Firebase
    private async Task LoadInventoryDataFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            if (snapshot.Child("Inventory").Exists)
            {
                // Lấy danh sách các item từ inventory
                List<object> inventoryItems = (List<object>)snapshot.Child("Inventory").Value;

                // In ra danh sách các item để kiểm tra
                foreach (var item in inventoryItems)
                {
                    Debug.Log("Inventory Item: " + item.ToString());
                    // Bạn có thể tạo lại các Canvas hoặc Item từ Firebase ở đây
                    // Ví dụ, tạo lại Canvas cho từng item trong inventory
                    GameObject itemCanvas = new GameObject(item.ToString()); // Tạo một Canvas mới cho item
                    itemCanvas.transform.SetParent(inventoryCanvas.transform); // Đặt item vào Inventory

                    // Bạn có thể thêm các thành phần như Image, Text vào itemCanvas tùy thuộc vào yêu cầu
                }

                Debug.Log("Inventory loaded from Firebase.");
            }
            else
            {
                Debug.LogWarning("No inventory data found for player: " + username);
            }
        }
        else
        {
            Debug.LogWarning("No data found for player: " + username);
        }
    }

    // Tải vị trí của người chơi từ Firebase
    private async Task LoadPlayerPositionFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            if (snapshot.Child("Position").Exists)
            {
                // Lấy giá trị Position từ Firebase
                var positionChild = snapshot.Child("Position");
                if (positionChild.Exists)
                {
                    float positionX = float.Parse(positionChild.Child("PositionX").Value.ToString());
                    float positionY = float.Parse(positionChild.Child("PositionY").Value.ToString());
                    float positionZ = float.Parse(positionChild.Child("PositionZ").Value.ToString());

                    // Debug: In ra các giá trị vị trí
                    Debug.Log($"Position loaded from Firebase: {positionX}, {positionY}, {positionZ}");

                    // Cập nhật vị trí của người chơi
                    player.transform.position = new Vector3(positionX, positionY, positionZ);
                }
            }
            else
            {
                Debug.LogWarning("No position data found for player: " + username);
            }
        }
        else
        {
            Debug.LogWarning("No data found for player: " + username);
        }
    }

    // Phương thức tải dữ liệu người chơi từ Firebase (thêm vào nếu cần)
    private async Task LoadPlayerDataFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            // Đọc các dữ liệu như Health, Energy, Exp, Gold, Diamond từ Firebase
            if (snapshot.Child("HealthCurrent").Exists)
                healthSlider.value = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());

            if (snapshot.Child("EnergyCurrent").Exists)
                energySlider.value = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());

            if (snapshot.Child("Exp").Exists)
                expSlider.value = int.Parse(snapshot.Child("Exp").Value.ToString());

            if (snapshot.Child("Gold").Exists)
                gold = int.Parse(snapshot.Child("Gold").Value.ToString());

            if (snapshot.Child("Diamond").Exists)
                diamond = int.Parse(snapshot.Child("Diamond").Value.ToString());

            // Cập nhật UI cho Gold và Diamond
            goldText.text = gold.ToString();
            diamondText.text = diamond.ToString();

            Debug.Log("Player data loaded from Firebase.");
        }
        else
        {
            Debug.LogWarning("No data found for player: " + username);
        }
    }
}
