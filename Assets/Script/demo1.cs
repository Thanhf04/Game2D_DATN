// FirebaseInventoryManagerAndTest.cs
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI; // Để làm việc với UI nếu cần
using System.Collections.Generic;

public class FirebaseInventoryManagerAndTest : MonoBehaviour
{
    private FirebaseFirestore firestore;

    // Các item mẫu và các slot có thể kéo thả trong Unity Inspector
    public InventoryItem sword;  // Item mẫu (ví dụ: sword)
    public InventoryItem shield; // Item mẫu (ví dụ: shield)

    // Các UI element nếu bạn muốn hiển thị inventory trong Unity (Có thể thay đổi tùy ý)
    public Text inventoryDisplay; // Hiển thị thông tin inventory (Text trong UI Unity)

    void Start()
    {
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            firestore = FirebaseFirestore.GetInstance(app);

            // Kiểm tra dữ liệu ban đầu
            TestFirebase();
        });
    }

    // Lưu dữ liệu inventory lên Firebase (khi người chơi mua đồ)
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

                            // Hiển thị UI nếu có Text display
                            if (inventoryDisplay != null)
                            {
                                inventoryDisplay.text += $"ItemID: {itemID}, Quantity: {quantity}\n";
                            }
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

    // Hàm kiểm tra việc lưu và tải inventory khi người chơi mua đồ
    private void TestFirebase()
    {
        // Tạo một vài item mẫu
        if (sword == null || shield == null)
        {
            Debug.LogError("Items are not assigned in the inspector.");
            return;
        }

        InventorySlot[] inventory = new InventorySlot[]
        {
            new InventorySlot(sword, 1), // 1 Sword
            new InventorySlot(shield, 2)  // 2 Shields
        };

        // Lưu inventory vào Firebase
        SaveInventoryToFirebase(inventory, "player1");  // "player1" là ID người chơi

        // Tải lại inventory từ Firebase
        LoadInventoryFromFirebase("player1");
    }

    // Hàm này sẽ được gọi khi người chơi mua đồ
    public void PlayerPurchaseItem(string playerID, InventoryItem purchasedItem, int quantity)
    {
        // Lấy inventory hiện tại của người chơi từ Firebase
        LoadInventoryFromFirebase(playerID);

        // Kiểm tra nếu item đã có trong inventory, nếu có thì tăng số lượng
        // Nếu chưa có thì tạo item mới và thêm vào inventory
        bool itemExists = false;
        InventorySlot[] currentInventory = GetInventoryFromFirebase(playerID); // Hàm này cần phải trả về inventory hiện tại từ Firebase

        foreach (var slot in currentInventory)
        {
            if (slot.GetItem().itemID == purchasedItem.itemID)
            {
                itemExists = true;
                // Tăng số lượng item
                slot.AddItem(slot.GetItem(), slot.GetQuantity() + quantity);
                break;
            }
        }

        if (!itemExists)
        {
            // Tạo một item mới và thêm vào inventory
            List<InventorySlot> updatedInventory = new List<InventorySlot>(currentInventory);
            updatedInventory.Add(new InventorySlot(purchasedItem, quantity));

            // Lưu lại inventory mới vào Firebase
            SaveInventoryToFirebase(updatedInventory.ToArray(), playerID);
        }
        else
        {
            // Lưu lại inventory đã cập nhật vào Firebase
            SaveInventoryToFirebase(currentInventory, playerID);
        }
    }

    // Giả sử có một hàm để lấy inventory từ Firebase
    private InventorySlot[] GetInventoryFromFirebase(string playerID)
    {
        // Đây là phần giả lập, bạn cần lấy inventory thực từ Firebase
        // Trả về một array của InventorySlot
        return new InventorySlot[0]; // Chỉ là một ví dụ, bạn cần thay thế bằng logic lấy inventory từ Firebase
    }
}
