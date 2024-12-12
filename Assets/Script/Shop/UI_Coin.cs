using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;

public class UI_Coin : MonoBehaviour
{
    [SerializeField] private int StartingCoins = 0; // Số coin bắt đầu
    private int currentCoins = 0;

    [Header("UI References")]
    public GameObject panelNotification;
    public GameObject panelShop;
    public Button btn_Close;

    public delegate void CoinChangedDelegate(int newCoinCount);
    public CoinChangedDelegate CoinChanged;

    // Firebase references
    private DatabaseReference reference;

    private void Awake()
    {
        currentCoins = StartingCoins;

        // Gán sự kiện cho nút đóng
        if (btn_Close != null)
        {
            btn_Close.onClick.AddListener(ClosePanel);
        }

        // Khởi tạo Firebase
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result == DependencyStatus.Available)
            {
                // Firebase khởi tạo thành công
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Khởi tạo reference Firebase sau khi khởi tạo xong Firebase
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                // Kiểm tra userName từ PlayerPrefs
                string userName = PlayerPrefs.GetString("username", "");
                if (string.IsNullOrEmpty(userName))
                {
                    Debug.LogError("Username is empty. Cannot load player data.");
                    return;
                }

                // Tải dữ liệu coin từ Firebase theo username
                LoadCoinsFromFirebase(userName);
            }
            else
            {
                Debug.LogError("Firebase dependencies are not available.");
            }
        });
    }

    // Thêm coin và lưu lên Firebase
    public void AddCoins(int amount)
    {
        if (reference == null)
        {
            Debug.LogError("Firebase reference is not initialized.");
            return;
        }

        string userName = PlayerPrefs.GetString("username", "");
        if (string.IsNullOrEmpty(userName))
        {
            Debug.LogError("Username is empty. Cannot add coins.");
            return;
        }

        currentCoins += amount;
        CoinChanged?.Invoke(currentCoins); // Gọi sự kiện khi thay đổi số coin

        // Lưu số coin lên Firebase
        SaveCoinsToFirebase(userName);
    }

    // Trừ coin khi mua đồ và lưu lên Firebase
    public bool SubTractCoins(int amount, Model_Shop.ItemType itemType)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            CoinChanged?.Invoke(currentCoins);
            Debug.Log($"Buy Item Success | Coin: {currentCoins} | Item: {itemType}");

            string userName = PlayerPrefs.GetString("username", "");

            // Kiểm tra userName và reference trước khi lưu
            if (!string.IsNullOrEmpty(userName) && reference != null)
            {
                SaveCoinsToFirebase(userName); // Lưu số coin sau khi trừ
            }
            else
            {
                Debug.LogError("Username is empty or Firebase reference is not initialized.");
            }

            return true;
        }
        else if (currentCoins <= 0)
        {
            Debug.Log("You don't have enough coins.");
            panelNotification.SetActive(true);
            panelShop.SetActive(false);
            return false;
        }
        else
        {
            Debug.Log("Buy item failed.");
            panelNotification.SetActive(true);
            panelShop.SetActive(false);
            return false;
        }
    }

    // Lấy số coin hiện tại
    public int GetCurrentCoins()
    {
        return currentCoins;
    }

    // Đóng panel thông báo
    public void ClosePanel()
    {
        panelNotification.SetActive(false);
        panelShop.SetActive(true);
    }

    // Lưu số coins lên Firebase theo username
    private void SaveCoinsToFirebase(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            Debug.LogError("Username is empty. Cannot save coins.");
            return;
        }

        // Kiểm tra reference trước khi sử dụng
        if (reference != null)
        {
            reference.Child("players").Child(userName).Child("coins").SetValueAsync(currentCoins)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log($"Coins saved successfully for user: {userName}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to save coins for user: {userName}");
                    }
                });
        }
        else
        {
            Debug.LogError("Firebase reference is not initialized.");
        }
    }

    // Tải số coins từ Firebase theo username
    // Tải số coins từ Firebase theo username
    private void LoadCoinsFromFirebase(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            Debug.LogError("Username is empty. Cannot load coins.");
            return;
        }

        // Kiểm tra reference trước khi sử dụng
        if (reference != null)
        {
            reference.Child("players").Child(userName).Child("coins").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // Chắc chắn rằng snapshot chứa giá trị hợp lệ
                        if (int.TryParse(snapshot.Value.ToString(), out int loadedCoins))
                        {
                            currentCoins = loadedCoins;
                            Debug.Log($"Coins Loaded: {currentCoins}");
                        }
                        else
                        {
                            Debug.LogError("Failed to parse coin value from Firebase.");
                            currentCoins = 100;  // Gán giá trị mặc định là 100 nếu không parse được
                        }
                    }
                    else
                    {
                        Debug.Log("No coins data found, using default (100 coins).");
                        currentCoins = 100;  // Gán giá trị mặc định là 100 nếu không có dữ liệu
                    }

                    CoinChanged?.Invoke(currentCoins); // Gọi sự kiện để cập nhật UI
                }
                else
                {
                    Debug.LogError("Failed to load coins from Firebase.");
                    currentCoins = 100;  // Gán giá trị mặc định là 100 nếu tải không thành công
                    CoinChanged?.Invoke(currentCoins); // Gọi sự kiện để cập nhật UI
                }
            });
        }
        else
        {
            Debug.LogError("Firebase reference is not initialized.");
        }
    }
}
