
using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDataTest : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text energyText;
    public TMP_Text expText;
    public TMP_Text goldText;
    public TMP_Text diamondText;
    public Slider healthSlider;
    public Slider energySlider;
    public Slider expSlider;
    public GameObject player;
    public Button teleportButton;  // Biến tham chiếu đến nút

    public int gold = 0;
    public int diamond = 0;

    private DatabaseReference databaseReference;
    private string username;
    private Vector3 lastSavedPosition;
    private bool isFirebaseInitialized = false;
    private bool isPlayerMoving = false;  // Biến theo dõi liệu người chơi có đang di chuyển hay không

    private bool isDataLoaded = false;
    private void TeleportPlayer()
    {
        if (player != null)
        {
            player.transform.position = new Vector3(-1, 0, 1);  // Di chuyển về vị trí (-1, 0, 1)
        }
    }

    private async void Start()
    {
        // Kiểm tra các phụ thuộc của Firebase
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            // Firebase đã sẵn sàng, có thể sử dụng các tính năng Firebase
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            isFirebaseInitialized = true;
            Debug.Log("Firebase initialized successfully.");

            // Sau khi Firebase đã được khởi tạo, tiếp tục thực hiện các thao tác khác
            username = PlayerPrefs.GetString("username", "");
            if (string.IsNullOrEmpty(username))
            {
                Debug.LogError("Username is empty. Cannot load player data.");
                return;
            }

            if (player == null)
            {
                Debug.LogError("Player GameObject is null. Cannot track position.");
                return;
            }

            if (healthSlider != null) healthSlider.onValueChanged.AddListener(UpdateHealth);
            if (energySlider != null) energySlider.onValueChanged.AddListener(UpdateEnergy);
            if (expSlider != null) expSlider.onValueChanged.AddListener(UpdateExp);

            lastSavedPosition = player.transform.position;

            // Kiểm tra và tải dữ liệu từ Firebase

            if (teleportButton != null)
            {
                teleportButton.onClick.AddListener(TeleportPlayer);  // Gọi phương thức TeleportPlayer khi nhấn nút
            }
        }
        else
        {
            // Nếu không thể khởi tạo Firebase, hiển thị lỗi
            Debug.LogError("Firebase không thể khởi tạo: " + dependencyStatus);
        }
    }

    private void UpdateHealth(float value) => SavePlayerData("HealthCurrent", (int)value);
    private void UpdateEnergy(float value) => SavePlayerData("EnergyCurrent", (int)value);
    private void UpdateExp(float value) => SavePlayerData("Exp", (int)value);
    private void UpdateGold(int value) => SavePlayerData("Gold", value);
    private void UpdateDiamond(int value) => SavePlayerData("Diamond", value);

    private async void SavePlayerData(string field, int value)
    {
        if (!isFirebaseInitialized || string.IsNullOrEmpty(username) || databaseReference == null)
        {
            Debug.LogError("SavePlayerData failed: Firebase not initialized or Username is empty.");
            return;
        }
        await databaseReference.Child("players").Child(username).Child(field).SetValueAsync(value);
    }

    private async void SavePlayerPosition(Vector3 position)
    {
        if (!isFirebaseInitialized || string.IsNullOrEmpty(username) || databaseReference == null || player == null) return;

        var positionDict = new Dictionary<string, object>
        {
            { "PositionX", position.x },
            { "PositionY", position.y },
            { "PositionZ", position.z }
        };

        string currentScene = SceneManager.GetActiveScene().name;  // Lấy tên scene hiện tại
        await databaseReference.Child("players").Child(username).Child("Positions").Child(currentScene).SetValueAsync(positionDict);
    }

    private async void SavePlayerScene(string sceneName)
    {
        if (!isFirebaseInitialized || string.IsNullOrEmpty(username) || databaseReference == null) return;
        await databaseReference.Child("players").Child(username).Child("Scene").SetValueAsync(sceneName);
    }

    private async Task CheckAndLoadPlayerData(string username)
    {
        if (!isFirebaseInitialized || string.IsNullOrEmpty(username) || databaseReference == null) return;

        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (!snapshot.Exists)
        {
            Debug.LogWarning("No data found for player, creating default data...");
            CreateDefaultPlayerData(username);
        }
        else
        {
            Debug.Log("Player data found. Loading data...");
            await LoadPlayerDataFromFirebase(username);
            await LoadPlayerPositionFromFirebase(username);
        }
    }

    private void CreateDefaultPlayerData(string username)
    {
        var defaultData = new Dictionary<string, object>
        {
            { "HealthCurrent", 100 },
            { "EnergyCurrent", 50 },
            { "Exp", 0 },
            { "Gold", 0 },
            { "Diamond", 0 },
            { "Position", new Dictionary<string, object>
                {
                    { "PositionX", 17 },
                    { "PositionY", 5 },
                    { "PositionZ", 0 }
                }
            }
        };

        databaseReference.Child("players").Child(username).SetValueAsync(defaultData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Default data created successfully.");
            }
            else
            {
                Debug.LogError("Failed to create default data.");
            }
        });
    }

    private async Task LoadPlayerDataFromFirebase(string username)
    {
        if (!isFirebaseInitialized || string.IsNullOrEmpty(username) || databaseReference == null) return;

        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists)
        {
            if (snapshot.Child("HealthCurrent").Exists)
            {
                int health = int.Parse(snapshot.Child("HealthCurrent").Value.ToString());
                if (healthSlider != null) healthSlider.value = health;
                if (healthText != null) healthText.text = "Health: " + health;
            }
            if (snapshot.Child("EnergyCurrent").Exists)
            {
                int energy = int.Parse(snapshot.Child("EnergyCurrent").Value.ToString());
                if (energySlider != null) energySlider.value = energy;
                if (energyText != null) energyText.text = "Energy: " + energy;
            }
            if (snapshot.Child("Exp").Exists)
            {
                int exp = int.Parse(snapshot.Child("Exp").Value.ToString());
                if (expSlider != null) expSlider.value = exp;
                if (expText != null) expText.text = "EXP: " + exp;
            }
            if (snapshot.Child("Gold").Exists)
            {
                int gold = int.Parse(snapshot.Child("Gold").Value.ToString());
                this.gold = gold;
                if (goldText != null) goldText.text = "Gold: " + gold;
            }
            if (snapshot.Child("Diamond").Exists)
            {
                int diamond = int.Parse(snapshot.Child("Diamond").Value.ToString());
                this.diamond = diamond;
                if (diamondText != null) diamondText.text = "Diamond: " + diamond;
            }
        }
    }

    private async Task LoadPlayerPositionFromFirebase(string username)
    {
        if (!isFirebaseInitialized || databaseReference == null || string.IsNullOrEmpty(username) || player == null)
        {
            Debug.LogError("Firebase is not initialized or Database reference, username, or player object is null.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name; // Lấy tên scene hiện tại
        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists && snapshot.Child("Positions").Child(currentScene).Exists)
        {
            float positionX = float.Parse(snapshot.Child("Positions").Child(currentScene).Child("PositionX").Value.ToString());
            float positionY = float.Parse(snapshot.Child("Positions").Child(currentScene).Child("PositionY").Value.ToString());
            float positionZ = float.Parse(snapshot.Child("Positions").Child(currentScene).Child("PositionZ").Value.ToString());

            player.transform.position = new Vector3(positionX, positionY, positionZ);
        }
        else
        {
            Debug.LogWarning("No position data found for player: " + username + " in scene: " + currentScene);
        }
    }

    // Kiểm tra sự điều khiển của người chơi mỗi frame
    private void Update()
    {
        if (!isDataLoaded && isFirebaseInitialized)
        {
            LoadPlayerDataFromFirebase(username);
            LoadPlayerPositionFromFirebase(username);
            isDataLoaded = true;
            CheckAndLoadPlayerData(username);
        }

        // Kiểm tra nếu người chơi đang di chuyển
        isPlayerMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // Chỉ lưu vị trí khi người chơi thực sự điều khiển nhân vật
        if (isPlayerMoving && player != null)
        {
            Vector3 currentPosition = player.transform.position;

            if (Vector3.Distance(currentPosition, lastSavedPosition) > 0.1f) // Chỉ lưu khi có sự thay đổi vị trí
            {
                SavePlayerPosition(currentPosition);
                lastSavedPosition = currentPosition;
            }
        }

        // Lưu tên scene hiện tại vào Firebase khi chuyển màn hình
        SavePlayerScene(SceneManager.GetActiveScene().name);
    }
}
