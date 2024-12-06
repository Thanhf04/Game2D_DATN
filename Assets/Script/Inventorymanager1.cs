using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

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
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
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

        RefreshUI();
    }

    private void RefreshUI()
    {
        // Kiểm tra lại mỗi slot và cập nhật UI
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                SlotClass slot = items[i]; // Lấy Slot hiện tại từ `items`

                // Cập nhật hình ảnh item
                if (slot.GetItem() != null)
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    slots[i].transform.GetChild(0).GetComponent<Image>().sprite = slot.GetItem().itemIcon;
                }
                else
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                }

                // Kiểm tra xem item có stackable hay không
                TextMeshProUGUI quantityText = slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                if (slot.GetItem() != null && slot.GetItem().isStackable)
                {
                    quantityText.text = slot.GetQuantity().ToString(); // Hiển thị số lượng
                }
                else
                {
                    quantityText.text = ""; // Nếu không stackable, thì không hiển thị số lượng
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error refreshing UI: " + ex.Message);
                // Reset UI cho slot khi có lỗi
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
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

                    // Debug log để kiểm tra các item
                    Debug.Log("Loaded inventory items:");
                    foreach (var slot in items)
                    {
                        if (slot.GetItem() != null)
                        {
                            Debug.Log("Item: " + slot.GetItem().itemName + ", Quantity: " + slot.GetQuantity());
                        }
                        else
                        {
                            Debug.Log("Empty slot at index: " + System.Array.IndexOf(items, slot));
                        }
                    }

                    // Cập nhật UI trong InventoryManager
                    // Đây là phần bạn cần chú ý để gọi lại RefreshUI
                    RefreshUI();

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
        SlotClass slot = ContainsItem(item);
        if (slot != null && slot.GetItem().isStackable)
        {
            slot.AddQuantity(quantity);
        }
        else
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null)
                {
                    items[i].AddItem(item, quantity);
                    break;
                }
            }
        }

        // Cập nhật Firebase
        SaveInventoryToFirebase(username);
        RefreshUI();
    }

    // Đồng bộ hóa RemoveItem từ InventoryManager
    public void RemoveItemFromFirebase(ItemClass item, int quantity)
    {
        SlotClass slot = ContainsItem(item);
        if (slot != null)
        {
            if (slot.GetQuantity() > quantity)
            {
                slot.SubQuantity(quantity);
            }
            else
            {
                int slotToRemoveIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }

                items[slotToRemoveIndex].RemoveItem();
            }

            // Cập nhật Firebase
            SaveInventoryToFirebase(username);
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
