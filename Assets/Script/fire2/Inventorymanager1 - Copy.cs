//using Firebase.Database;
//using Firebase.Extensions;
//using UnityEngine;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//public class InventoryManager1 : MonoBehaviour
//{
//    private DatabaseReference databaseReference;
//    private string username;

//    // Các vật phẩm trong kho
//    [SerializeField] private SlotClass[] items; // Các slot chứa item trong inventory
//    [SerializeField] private SlotClass[] startingItems; // Các item khởi tạo khi bắt đầu game

//    void Start()
//    {
//        // Khởi tạo Firebase Database
//        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

//        // Lấy tên đăng nhập đã lưu (từ phần đăng nhập Firebase)
//        username = PlayerPrefs.GetString("username");

//        // Nếu đã có username thì tải dữ liệu
//        if (!string.IsNullOrEmpty(username))
//        {
//            LoadInventoryFromFirebase(username);
//        }
//    }

//    // Lưu toàn bộ inventory lên Firebase
//    public async Task SaveInventoryToFirebase()
//    {
//        // Tạo một Dictionary để lưu trữ dữ liệu
//        var inventoryDict = new Dictionary<string, object>();

//        // Lưu thông tin về các vật phẩm trong items
//        for (int i = 0; i < items.Length; i++)
//        {
//            // Kiểm tra nếu slot này chứa item hợp lệ
//            if (items[i].GetItem() != null)
//            {
//                var itemData = new Dictionary<string, object>
//                {
//                    { "ItemID", items[i].GetItem().itemID },   // Lưu ItemID
//                    { "Quantity", items[i].GetQuantity() }      // Lưu số lượng vật phẩm
//                };

//                inventoryDict[$"item_{i}"] = itemData;  // Lưu từng item theo chỉ mục
//            }
//            else
//            {
//                // Nếu slot trống, lưu thông tin slot trống
//                inventoryDict[$"item_{i}"] = new Dictionary<string, object>
//                {
//                    { "ItemID", "" },
//                    { "Quantity", 0 }
//                };
//            }
//        }

//        // Lưu dữ liệu lên Firebase
//        await databaseReference.Child("players").Child(username).Child("inventory").SetValueAsync(inventoryDict);
//        Debug.Log("Inventory data saved to Firebase.");
//    }

//    // Tải toàn bộ inventory từ Firebase
//    public void LoadInventoryFromFirebase(string username)
//    {
//        databaseReference.Child("players").Child(username).Child("inventory").GetValueAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsFaulted)
//            {
//                Debug.LogError("Error loading inventory data from Firebase: " + task.Exception);
//                return;
//            }

//            DataSnapshot snapshot = task.Result;
//            if (snapshot.Exists)
//            {
//                // Duyệt qua tất cả các item được lưu trong Firebase
//                foreach (var childSnapshot in snapshot.Children)
//                {
//                    string itemKey = childSnapshot.Key;
//                    var itemData = childSnapshot.Value as Dictionary<string, object>;

//                    if (itemData != null)
//                    {
//                        string itemID = itemData["ItemID"].ToString();
//                        int quantity = int.Parse(itemData["Quantity"].ToString());

//                        // Tìm item theo ID và cập nhật số lượng
//                        SlotClass slot = GetSlotByItemID(itemID);
//                        if (slot != null)
//                        {
//                            if (!string.IsNullOrEmpty(itemID)) // Kiểm tra xem có phải là item hợp lệ
//                            {
//                                slot.AddItem(GetItemByID(itemID), quantity); // Cập nhật item và số lượng
//                            }
//                        }
//                    }
//                }

//                Debug.Log("Inventory data loaded from Firebase.");
//            }
//            else
//            {
//                Debug.LogWarning("No inventory data found for user: " + username);
//                // Nếu không có dữ liệu, bạn có thể thêm các item mặc định hoặc bắt đầu lại
//            }
//        });
//    }

//    // Lấy SlotClass từ ItemID
//    private SlotClass GetSlotByItemID(string itemID)
//    {
//        // Duyệt qua tất cả các slot trong inventory và tìm slot chứa Item có ID phù hợp
//        foreach (var slot in items)
//        {
//            if (slot.GetItem() != null && slot.GetItem().itemID == itemID)
//            {
//                return slot;
//            }
//        }
//        return null; // Nếu không tìm thấy thì trả về null
//    }

//    // Lấy ItemClass từ ItemID
//    private ItemClass GetItemByID(string itemID)
//    {
//        // Giả sử bạn có một danh sách các item đã được định nghĩa sẵn
//        // Bạn có thể làm theo cách này để tìm kiếm item từ ID
//        foreach (var slot in startingItems)
//        {
//            if (slot.GetItem() != null && slot.GetItem().itemID == itemID)
//            {
//                return slot.GetItem(); // Trả về item với ItemID phù hợp
//            }
//        }
//        return null; // Nếu không tìm thấy, trả về null
//    }
//}
