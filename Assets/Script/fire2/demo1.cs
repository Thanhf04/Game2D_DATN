using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;

// Lớp đại diện cho một item trong game
public class InventoryItem
{
    public string itemID;
    public string itemName;
    public int itemQuantity;

    // Constructor để khởi tạo một InventoryItem
    public InventoryItem(string id, string name, int quantity)
    {
        itemID = id;
        itemName = name;
        itemQuantity = quantity;
    }
}

// Lớp đại diện cho một Slot trong inventory
public class InventorySlot
{
    private InventoryItem item;
    private int quantity;

    // Constructor để khởi tạo một InventorySlot với InventoryItem và số lượng
    public InventorySlot(InventoryItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    // Phương thức lấy InventoryItem trong slot
    public InventoryItem GetItem()
    {
        return item;
    }

    // Phương thức lấy số lượng của item trong slot
    public int GetQuantity()
    {
        return quantity;
    }

    // Phương thức thêm InventoryItem vào slot
    public void AddItem(InventoryItem newItem, int newQuantity)
    {
        this.item = newItem;
        this.quantity = newQuantity;
    }
}

public class FirebaseInventoryManager : MonoBehaviour
{
    private FirebaseFirestore firestore;

    void Start()
    {
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            firestore = FirebaseFirestore.GetInstance(app);
        });
    }

    // Lưu dữ liệu inventory lên Firebase
    public void SaveInventoryToFirebase(InventorySlot[] items, string playerID)
    {
        DocumentReference docRef = firestore.Collection("players").Document(playerID); // Sử dụng playerID thực tế

        // Chuyển inventory thành một dictionary để lưu vào Firebase
        Dictionary<string, object> inventoryData = new Dictionary<string, object>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() != null)
            {
                inventoryData["slot_" + i] = new Dictionary<string, object>
                {
                    { "itemID", items[i].GetItem().itemID },
                    { "quantity", items[i].GetQuantity() }
                };
            }
        }

        // Lưu dữ liệu vào Firestore
        docRef.SetAsync(inventoryData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Inventory saved successfully to Firebase!");
            }
            else
            {
                Debug.LogError("Failed to save inventory to Firebase.");
            }
        });
    }

    // Tải dữ liệu inventory từ Firebase
    public void LoadInventoryFromFirebase(string playerID)
    {
        DocumentReference docRef = firestore.Collection("players").Document(playerID);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Dictionary<string, object> inventoryData = snapshot.ToDictionary();
                    foreach (var slot in inventoryData)
                    {
                        Debug.Log("Slot: " + slot.Key);
                        Dictionary<string, object> itemData = slot.Value as Dictionary<string, object>;
                        if (itemData != null)
                        {
                            string itemID = itemData["itemID"].ToString();
                            int quantity = int.Parse(itemData["quantity"].ToString());

                            // Hiển thị thông tin item
                            Debug.Log($"ItemID: {itemID}, Quantity: {quantity}");
                        }
                    }
                }
                else
                {
                    Debug.LogError("No inventory data found for player: " + playerID);
                }
            }
            else
            {
                Debug.LogError("Failed to load inventory from Firebase.");
            }
        });
    }
}

// Lớp TestFirebaseInventory để kiểm tra việc lưu và tải dữ liệu từ Firebase
public class TestFirebaseInventory : MonoBehaviour
{
    public FirebaseInventoryManager firebaseInventoryManager;

    void Start()
    {
        // Tạo một vài item mẫu
        InventoryItem sword = new InventoryItem("1", "Sword", 10);
        InventoryItem shield = new InventoryItem("2", "Shield", 15);

        // Tạo inventory
        InventorySlot[] inventory = new InventorySlot[]
        {
            new InventorySlot(sword, 1), // 1 Sword
            new InventorySlot(shield, 2)  // 2 Shields
        };

        // Lưu inventory vào Firebase
        firebaseInventoryManager.SaveInventoryToFirebase(inventory, "player1");  // "player1" là ID người chơi

        // Tải lại inventory từ Firebase
        firebaseInventoryManager.LoadInventoryFromFirebase("player1");
    }
}
