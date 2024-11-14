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
        goldText.text = " " + value.ToString();
        SavePlayerData("Gold", value); // Lưu giá trị Gold vào Firebase
    }

    private void UpdateDiamond(int value)
    {
        diamondText.text = " " + value.ToString();
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
        // Kiểm tra xem username có hợp lệ không
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Username is null or empty.");
            return;
        }

        // Kiểm tra nếu databaseReference đã được khởi tạo
        if (databaseReference == null)
        {
            Debug.LogError("Database reference is null. Firebase may not have been initialized properly.");
            return;
        }

        try
        {
            var playerRef = databaseReference.Child("players").Child(username);
            var snapshot = await playerRef.GetValueAsync();

            // Kiểm tra xem snapshot có tồn tại dữ liệu không
            if (snapshot.Exists)
            {
                // Lấy và cập nhật giá trị Health
                if (snapshot.Child("HealthCurrent").Value != null)
                {
                    int health = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());
                    healthSlider.value = health;
                    healthText.text = "Health: " + health.ToString();
                }
                else
                {
                    Debug.LogWarning("Health data not found for player: " + username);
                }

                // Lấy và cập nhật giá trị Energy
                if (snapshot.Child("EnergyCurrent").Value != null)
                {
                    int energy = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());
                    energySlider.value = energy;
                    energyText.text = "Energy: " + energy.ToString();
                }
                else
                {
                    Debug.LogWarning("Energy data not found for player: " + username);
                }

                // Lấy và cập nhật giá trị EXP
                if (snapshot.Child("Exp").Value != null)
                {
                    int exp = int.Parse(snapshot.Child("Exp").Value.ToString());
                    expSlider.value = exp;
                    expText.text = "EXP: " + exp.ToString();
                }
                else
                {
                    Debug.LogWarning("Exp data not found for player: " + username);
                }

                // Lấy và cập nhật giá trị Gold
                if (snapshot.Child("Gold").Value != null)
                {
                    int gold = int.Parse(snapshot.Child("Gold").Value.ToString());
                    this.gold = gold; // Cập nhật giá trị Gold
                    goldText.text = "Gold: " + gold.ToString(); // Hiển thị Gold
                }
                else
                {
                    Debug.LogWarning("Gold data not found for player: " + username);
                }

                // Lấy và cập nhật giá trị Diamond
                if (snapshot.Child("Diamond").Value != null)
                {
                    int diamond = int.Parse(snapshot.Child("Diamond").Value.ToString());
                    this.diamond = diamond; // Cập nhật giá trị Diamond
                    diamondText.text = "Diamond: " + diamond.ToString(); // Hiển thị Diamond
                }
                else
                {
                    Debug.LogWarning("Diamond data not found for player: " + username);
                }

                Debug.Log("Player data loaded from Firebase.");
            }
            else
            {
                Debug.LogWarning("No data found for player: " + username);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading player data: " + ex.Message);
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
                    Debug.Log($"Loaded position: X={positionX}, Y={positionY}, Z={positionZ}");

                    // Cập nhật vị trí của nhân vật trong game
                    player.transform.position = new Vector3(positionX, positionY, positionZ);
                    Debug.Log("Player position loaded from Firebase: " + player.transform.position);
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

    // Tải dữ liệu từ InventoryCanvas (giả sử bạn có các item trong inventory)
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
                }

                // Nếu bạn có một hệ thống UI cho inventory, bạn có thể cập nhật UI ở đây
                // Ví dụ: update inventory UI hoặc tạo item trong game
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
}
