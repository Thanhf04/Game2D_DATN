using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseInventoryManager1 : MonoBehaviour
{
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private SlotClass[] items;
    [SerializeField] private GameObject[] slots;
    public bool isMoving;

    // Firebase Realtime Database reference
    private DatabaseReference reference;

    public string username;

    // Start is called before the first frame update
    void Start()
    {
        // Kiểm tra xem username đã được lưu trong PlayerPrefs chưa
        username = PlayerPrefs.GetString("username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username chưa được lưu trong PlayerPrefs.");
            return;
        }

        // Cài đặt Firebase và kiểm tra dependencies
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Firebase đã sẵn sàng
                FirebaseApp app = FirebaseApp.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                // Xử lý các vật phẩm ban đầu
                InitializeInventory();

                // Tải kho đồ từ Firebase
                LoadInventoryFromFirebase(username); // Đảm bảo gọi lại LoadInventoryFromFirebase
            }
            else
            {
                Debug.LogError("Firebase không sẵn sàng. Lỗi: " + task.Result.ToString());
            }
        });
    }

    private void InitializeInventory()
    {
        // Lấy các slot từ UI
        slots = new GameObject[slotsHolder.transform.childCount];
        items = new SlotClass[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotsHolder.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new SlotClass();
        }

        //RefreshUI();
    }

    public void RefreshUI()
    {
        // Kiểm tra lại mỗi slot và cập nhật UI
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                // Kiểm tra slot có tồn tại trong kho đồ
                if (i >= items.Length)
                {
                    Debug.LogWarning($"Không có item tại slot {i}");
                    continue; // Bỏ qua slot nếu không có item tương ứng
                }

                SlotClass slot = items[i]; // Lấy Slot hiện tại từ `items`

                // Kiểm tra item có tồn tại trong slot hay không
                if (slot.GetItem() != null)
                {
                    // Cập nhật hình ảnh item
                    Image itemImage = slots[i].transform.GetChild(0).GetComponent<Image>();
                    if (itemImage != null)
                    {
                        itemImage.enabled = true;
                        itemImage.sprite = slot.GetItem().itemIcon;
                    }

                    // Cập nhật số lượng nếu item là stackable
                    TextMeshProUGUI quantityText = slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (quantityText != null && slot.GetItem().isStackable)
                    {
                        quantityText.text = slot.GetQuantity().ToString(); // Hiển thị số lượng
                    }
                    else
                    {
                        quantityText.text = ""; // Nếu không stackable, thì không hiển thị số lượng
                    }
                }
                else
                {
                    // Nếu không có item, ẩn hình ảnh và số lượng
                    Image itemImage = slots[i].transform.GetChild(0).GetComponent<Image>();
                    if (itemImage != null)
                    {
                        itemImage.enabled = false;
                    }

                    TextMeshProUGUI quantityText = slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (quantityText != null)
                    {
                        quantityText.text = ""; // Xóa số lượng nếu không có item
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error refreshing UI for slot {i}: {ex.Message}");

                // Reset UI cho slot khi có lỗi
                Image itemImage = slots[i].transform.GetChild(0).GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.sprite = null;
                    itemImage.enabled = false;
                }

                TextMeshProUGUI quantityText = slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = "";
                }
            }
        }
    }


    // Lưu kho đồ lên Firebase
    public void SaveInventoryToFirebase(string username)
    {
        if (reference == null || string.IsNullOrEmpty(username)) return;

        string json = JsonUtility.ToJson(new InventoryData(items));
        reference.Child("players").Child(username).Child("inventory").SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Kho đồ đã được lưu lên Firebase.");
            }
            else
            {
                Debug.LogError("Lỗi khi lưu kho đồ lên Firebase.");
            }
        });
    }

    // Tải kho đồ từ Firebase
    // Tải kho đồ từ Firebase
    private void LoadInventoryFromFirebase(string username)
    {
        if (reference == null || string.IsNullOrEmpty(username)) return;

        reference.Child("players").Child(username).Child("inventory").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);

                    // Cập nhật kho đồ từ dữ liệu tải về
                    items = inventoryData.items;

                    // Cập nhật lại UI của InventoryManager
                    InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                    if (inventoryManager != null)
                    {
                        inventoryManager.SetInventoryItems(items);
                    }

                    Debug.Log("Kho đồ đã được tải từ Firebase.");
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy dữ liệu kho đồ cho người dùng này.");
                }
            }
            else
            {
                Debug.LogError("Lỗi khi tải kho đồ từ Firebase.");
            }
        });
    }

    // Đồng bộ hóa AddItem từ InventoryManager
    public void AddItemToFirebase(ItemClass item, int quantity)
    {
        // Tìm slot chứa item hoặc tạo slot mới nếu không có
        SlotClass slot = ContainsItem(item);
        if (slot != null && slot.GetItem().isStackable)
        {
            // Nếu vật phẩm đã tồn tại và là stackable, chỉ thêm số lượng vào mà không tạo thêm slot mới
            slot.AddQuantity(quantity);  // Cộng thêm số lượng vật phẩm vào kho
        }
        else
        {
            // Nếu không có slot trống, hãy tìm slot trống và thêm vật phẩm vào
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null)
                {
                    items[i].AddItem(item, quantity);  // Thêm vật phẩm vào kho (1 vật phẩm)
                    break;
                }
            }
        }

        // Đồng bộ hóa kho đồ lên Firebase ngay sau khi thay đổi
        SaveInventoryToFirebase(username);

        // Cập nhật lại giao diện UI để hiển thị kho đồ mới
        RefreshUI();
    }



    // Đồng bộ hóa RemoveItem từ InventoryManager
    public void RemoveItemFromFirebase(ItemClass item, int quantity)
    {
        SlotClass slot = ContainsItem(item);
        if (slot != null)
        {
            if (slot.GetQuantity() >= quantity)
            {
                slot.SubQuantity(quantity);  // Trừ đi số lượng vật phẩm khi sử dụng
            }
            else
            {
                // Nếu số lượng vật phẩm ít hơn số lượng yêu cầu, xóa hết vật phẩm khỏi slot
                int slotToRemoveIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }

                items[slotToRemoveIndex].RemoveItem();  // Xóa vật phẩm khỏi kho nếu không đủ số lượng
            }

            // Đồng bộ hóa kho đồ lên Firebase ngay sau khi thay đổi
            SaveInventoryToFirebase(username);

            // Cập nhật lại giao diện UI để hiển thị kho đồ mới
            RefreshUI();
        }
    }
 
    private SlotClass ContainsItem(ItemClass item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item)
            {
                return items[i];
            }
        }
        return null;
    }
}

// Lớp để lưu trữ kho đồ trong Firebase
[System.Serializable]
public class InventoryData
{
    public SlotClass[] items;

    public InventoryData(SlotClass[] items)
    {
        this.items = items;
    }
}