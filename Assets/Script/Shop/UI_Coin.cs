using Firebase;
using Firebase.Database;
using System.Threading.Tasks;  // Thêm dòng này để sử dụng Task
using UnityEngine;
using UnityEngine.UI;

public class UI_Coin : MonoBehaviour
{
    [SerializeField] private int StartingCoins = 0; // Số coin bắt đầu
    private int currentCoins = 0;

    [Header("UI References")]
    public GameObject panelNotification;
    public GameObject panelShop;
    public Button btn_Close;

    // Firebase reference
    private DatabaseReference databaseReference;
    private string username; // Tên người dùng để lưu trữ và tải dữ liệu từ Firebase

    public delegate void CoinChangedDelegate(int newCoinCount);
    public CoinChangedDelegate CoinChanged;

    private void Awake()
    {
        currentCoins = StartingCoins;

        // Gán sự kiện cho nút đóng
        if (btn_Close != null)
        {
            btn_Close.onClick.AddListener(ClosePanel);
        }

        // Kiểm tra và khởi tạo Firebase
        InitializeFirebase();
    }

    private async void Start()
    {
        // Lấy username từ PlayerPrefs
        username = PlayerPrefs.GetString("username", "");

        // Kiểm tra nếu username là rỗng
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is not set in PlayerPrefs. Cannot load coins.");
            return; // Dừng quá trình nếu không có username hợp lệ
        }

        // Load coins từ Firebase khi game bắt đầu
        await LoadCoinsFromFirebase();

        // Nếu chưa có dữ liệu coin, sẽ thêm 100 coin vào
        if (currentCoins == 0)
        {
            AddCoins(100);
        }
    }


    private void InitializeFirebase()
    {
        // Kiểm tra trạng thái Firebase và khởi tạo Firebase Database
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    // Lưu số coin vào Firebase
    private async void SaveCoinsToFirebase()
    {
        if (databaseReference != null && !string.IsNullOrEmpty(username))
        {
            await databaseReference.Child("players").Child(username).Child("Coins").SetValueAsync(currentCoins);
        }
        else
        {
            Debug.LogError("Firebase reference or username is invalid.");
        }
    }

    // Tải số coin từ Firebase
    private async Task LoadCoinsFromFirebase()
    {
        if (databaseReference == null || string.IsNullOrEmpty(username))
        {
            Debug.LogError("Database reference or username is null. Cannot load coins.");
            return;
        }

        var playerRef = databaseReference.Child("players").Child(username);
        var snapshot = await playerRef.GetValueAsync();

        if (snapshot.Exists && snapshot.Child("Coins").Exists)
        {
            currentCoins = int.Parse(snapshot.Child("Coins").Value.ToString());
            CoinChanged?.Invoke(currentCoins); // Cập nhật UI khi tải dữ liệu
        }
        else
        {
            Debug.LogWarning("No coin data found for player: " + username);
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        CoinChanged?.Invoke(currentCoins); // Gọi sự kiện khi thay đổi số coin
        SaveCoinsToFirebase(); // Lưu số coin mới vào Firebase
    }

    public bool SubTractCoins(int amount, Model_Shop.ItemType itemType)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            CoinChanged?.Invoke(currentCoins);
            SaveCoinsToFirebase(); // Lưu số coin mới vào Firebase

            Debug.Log($"Buy Item Success | Coin: {currentCoins} | Item: {itemType}");
            return true;
        }
        else if (currentCoins <= 0)
        {
            Debug.Log("You don't have money");
            panelNotification.SetActive(true);
            panelShop.SetActive(false);
            return false;
        }
        else
        {
            Debug.Log("Buy item fail");
            panelNotification.SetActive(true);
            panelShop.SetActive(false);
            return false;
        }
    }

    public int GetCurrentCoins()
    {
        return currentCoins;
    }

    public void ClosePanel()
    {
        panelNotification.SetActive(false);
        panelShop.SetActive(true);
    }
}
