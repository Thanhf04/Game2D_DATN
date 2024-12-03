using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerDataTest : MonoBehaviour
{
    public TMP_Text healthText;    // Hiển thị giá trị health
    public TMP_Text energyText;    // Hiển thị giá trị energy
    public TMP_Text expText;       // Hiển thị giá trị exp
    public TMP_Text goldText;      // Hiển thị giá trị gold
    public TMP_Text diamondText;   // Hiển thị giá trị diamond
    public Slider healthSlider;    // Slider cho health
    public Slider energySlider;    // Slider cho energy
    public Slider expSlider;       // Slider cho exp
    public GameObject player;      // Nhân vật trong game
    public GameObject inventoryCanvas;  // GameObject chứa UI cho inventory hoặc túi đồ

    // Các biến để lưu trữ giá trị
    public int gold = 0;            // Biến gold
    public int diamond = 0;         // Biến diamond
    public SlotClass[] items;       // Mảng các Slot chứa các Item trong inventory
    public ItemClass itemToAdd;     // Item để thêm vào
    public ItemClass itemToRemove;  // Item để loại bỏ

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
        healthText.text = "Health: " + value.ToString("F0");
        SavePlayerData("HealthCurrent", (int)value);
    }

    private void UpdateEnergy(float value)
    {
        energyText.text = "Energy: " + value.ToString("F0");
        SavePlayerData("EnergyCurrent", (int)value);
    }

    private void UpdateExp(float value)
    {
        expText.text = "EXP: " + value.ToString("F0");
        SavePlayerData("Exp", (int)value);
    }

    private void UpdateGold(int value)
    {
        goldText.text = "Gold: " + value.ToString();
        SavePlayerData("Gold", value); // Lưu giá trị Gold vào Firebase
    }

    private void UpdateDiamond(int value)
    {
        diamondText.text = "Diamond: " + value.ToString();
        SavePlayerData("Diamond", value); // Lưu giá trị Diamond vào Firebase
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

    // Lưu thông tin từ inventoryCanvas (giả sử bạn có các thông tin về túi đồ)
    private async void SaveInventoryData()
    {
        if (string.IsNullOrEmpty(username)) return;

        // Lấy dữ liệu từ inventoryCanvas (ví dụ: các vật phẩm trong túi đồ)
        List<string> inventoryItems = new List<string> { "Item1", "Item2", "Item3" }; // Ví dụ, lấy các item từ inventoryCanvas

        try
        {
            // Lưu thông tin túi đồ vào Firebase
            await databaseReference.Child("players").Child(username).Child("Inventory").SetValueAsync(inventoryItems);
            Debug.Log("Inventory data saved to Firebase.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving inventory data: {ex.Message}");
        }
    }

    // Tải lại tất cả dữ liệu người chơi từ Firebase
    private async Task LoadPlayerDataFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            // Lấy và cập nhật giá trị Health
            int health = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());
            healthSlider.value = health;
            healthText.text = "Health: " + health.ToString();

            // Lấy và cập nhật giá trị Energy
            int energy = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());
            energySlider.value = energy;
            energyText.text = "Energy: " + energy.ToString();

            // Lấy và cập nhật giá trị EXP
            int exp = int.Parse(snapshot.Child("Exp").Value.ToString());
            expSlider.value = exp;
            expText.text = " " + exp.ToString();

            // Lấy và cập nhật giá trị Gold
            int gold = int.Parse(snapshot.Child("Gold").Value.ToString());
            UpdateGold(gold);

            // Lấy và cập nhật giá trị Diamond
            int diamond = int.Parse(snapshot.Child("Diamond").Value.ToString());
            UpdateDiamond(diamond);

            Debug.Log("Player data loaded successfully from Firebase.");
        }
        else
        {
            Debug.LogError("Player data not found in Firebase.");
        }
    }

    // Tải thông tin vị trí người chơi từ Firebase
    private async Task LoadPlayerPositionFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username).Child("Position");
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            float posX = float.Parse(snapshot.Child("PositionX").Value.ToString());
            float posY = float.Parse(snapshot.Child("PositionY").Value.ToString());
            float posZ = float.Parse(snapshot.Child("PositionZ").Value.ToString());

            Vector3 position = new Vector3(posX, posY, posZ);
            player.transform.position = position;

            Debug.Log("Player position loaded from Firebase: " + position);
        }
        else
        {
            Debug.LogError("Player position data not found in Firebase.");
        }
    }

    // Tải dữ liệu từ inventoryCanvas (giả sử bạn đã lưu nó trong Firebase)
    private async Task LoadInventoryDataFromFirebase(string username)
    {
        var playerRef = databaseReference.Child("players").Child(username).Child("Inventory");
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            List<string> inventoryItems = new List<string>();
            foreach (var item in snapshot.Children)
            {
                inventoryItems.Add(item.Value.ToString());
            }

            // Bạn có thể sử dụng dữ liệu inventoryItems này để cập nhật UI của inventoryCanvas
            Debug.Log("Inventory data loaded: " + string.Join(", ", inventoryItems));
        }
        else
        {
            Debug.LogError("Inventory data not found in Firebase.");
        }
    }
}
