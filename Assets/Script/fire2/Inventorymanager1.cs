//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using Firebase;
//using Firebase.Extensions;
//using Firebase.Auth;
//using Firebase.Firestore;
//using TMPro;
//using System.Collections.Generic;

//public class InventoryManager12 : MonoBehaviour
//{
//    [SerializeField] private GameObject slotsHolder;  // Container cho các slot
//    [SerializeField] private ItemClass itemToAdd;     // Vật phẩm để thêm vào
//    [SerializeField] private ItemClass itemToRemove;  // Vật phẩm để xóa
//    [SerializeField] private SlotClass[] items;       // Mảng các slot
//    [SerializeField] private SlotClass[] startingItems;  // Mảng các item ban đầu

//    [SerializeField] private GameObject[] slots;

//    private FirebaseFirestore db;
//    private FirebaseAuth auth;
//    private string userId; // Unique ID của người chơi

//    // use item
//    [Header("Use Item")]
//    [SerializeField] private Button Btn_Health;
//    [SerializeField] private Button Btn_Mana;
//    [SerializeField] private ItemClass healthItem;
//    [SerializeField] private ItemClass manaItem;
//    [SerializeField] public Slider healthSlider;
//    [SerializeField] public Slider manaSlider;
//    [SerializeField] private TextMeshProUGUI healthButtonText;
//    [SerializeField] private TextMeshProUGUI manaButtonText;
//    private float itemCooldownTime = 2f;
//    private bool isHealthOnCooldown = false;
//    private bool isManaOnCooldown = false;
//    Dichuyennv1 player1;

//    void Start()
//    {
//        db = FirebaseFirestore.DefaultInstance;
//        auth = FirebaseAuth.DefaultInstance;

//        // Kiểm tra xem người dùng đã đăng nhập chưa
//        if (auth.CurrentUser != null)
//        {
//            userId = auth.CurrentUser.UserId; // Lấy userId của người dùng đã đăng nhập
//            LoadInventoryFromFirebase(); // Tải dữ liệu từ Firestore khi đăng nhập thành công
//        }
//        else
//        {
//            Debug.LogError("User not logged in");
//        }

//        // Đảm bảo rằng bạn đã cấu hình nút và các thông tin khác
//        Btn_Health.onClick.AddListener(() => UseHealth(healthItem));
//        Btn_Mana.onClick.AddListener(() => UseMana(manaItem));
//        healthButtonText.text = "";
//        manaButtonText.text = "";

//        slots = new GameObject[slotsHolder.transform.childCount];
//        items = new SlotClass[slots.Length];

//        for (int i = 0; i < slots.Length; i++)
//        {
//            slots[i] = slotsHolder.transform.GetChild(i).gameObject;
//        }

//        // Đặt slot trống ban đầu
//        for (int i = 0; i < items.Length; i++)
//        {
//            items[i] = new SlotClass();
//        }
//    }

//    // Tải dữ liệu từ Firebase
//    private void LoadInventoryFromFirebase()
//    {
//        DocumentReference docRef = db.Collection("users").Document(userId); // Sử dụng userId làm tên tài liệu
//        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsCompleted && task.Result.Exists)
//            {
//                Dictionary<string, object> userData = task.Result.ToDictionary();
//                // Load dữ liệu vật phẩm từ Firestore vào items
//                for (int i = 0; i < slots.Length; i++)
//                {
//                    if (userData.ContainsKey($"item_{i}_id"))
//                    {
//                        string itemId = userData[$"item_{i}_id"].ToString();
//                        int quantity = int.Parse(userData[$"item_{i}_quantity"].ToString());
//                        ItemClass item = GetItemById(itemId);
//                        items[i].AddItem(item, quantity);
//                    }
//                }
//            }
//            RefreshUI(); // Cập nhật lại giao diện UI
//        });
//    }

//    // Lưu dữ liệu vào Firebase
//    public void SaveInventoryToFirebase()
//    {
//        DocumentReference docRef = db.Collection("users").Document(userId); // Sử dụng userId làm tên tài liệu
//        WriteBatch batch = db.StartBatch();

//        for (int i = 0; i < slots.Length; i++)
//        {
//            if (items[i].GetItem() != null)
//            {
//                batch.Set(docRef.Collection("inventory").Document($"slot_{i}"), new Dictionary<string, object>
//                {
//                    { "item_id", items[i].GetItem().itemId },
//                    { "quantity", items[i].GetQuantity() }
//                });
//            }
//            else
//            {
//                // Nếu ô đồ trống, xóa dữ liệu
//                batch.Delete(docRef.Collection("inventory").Document($"slot_{i}"));
//            }
//        }

//        batch.CommitAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsCompleted)
//            {
//                Debug.Log("Inventory saved to Firebase!");
//            }
//        });
//    }

//    // Lấy item từ ID
//    private ItemClass GetItemById(string itemId)
//    {
//        // Tạo item từ ID, sử dụng lớp con nếu cần
//        ItemClass item = new ConsumableClass(); // Ví dụ sử dụng ConsumableClass
//        item.itemId = itemId; // Thiết lập itemId nếu cần
//        return item;
//    }

//    // Cập nhật giao diện UI
//    private void RefreshUI()
//    {
//        for (int i = 0; i < slots.Length; i++)
//        {
//            try
//            {
//                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
//                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;
//                if (!items[i].GetItem().isStackable)
//                {
//                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
//                }
//                else
//                {
//                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = items[i].GetQuantity() + "";
//                }
//            }
//            catch
//            {
//                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
//                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
//                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
//            }
//        }
//    }

//    // Hàm sử dụng item
//    public void UseHealth(ItemClass item)
//    {
//        if (item is ConsumableClass consumable)
//        {
//            if (player1.currentHealth < player1.maxHealth)
//            {
//                player1.currentHealth = Mathf.Min(player1.currentHealth + 50, player1.maxHealth);
//                healthSlider.value = player1.currentHealth;
//                RemoveItem(item, 1);
//                RefreshUI();
//                StartCoroutine(ItemCooldown(Btn_Health, healthButtonText, true));
//            }
//        }
//    }

//    public void UseMana(ItemClass item)
//    {
//        if (item is ConsumableClass consumable)
//        {
//            if (player1.currentMana < player1.maxMana)
//            {
//                player1.currentMana = Mathf.Min(player1.currentMana + 50, player1.maxMana);
//                manaSlider.value = player1.currentMana;
//                RemoveItem(item, 1);
//                RefreshUI();
//                StartCoroutine(ItemCooldown(Btn_Mana, manaButtonText, true));
//            }
//        }
//    }

//    // Giới hạn việc gọi sử dụng item
//    private IEnumerator ItemCooldown(Button button, TextMeshProUGUI buttonText, bool isHealth)
//    {
//        float remainingTime = itemCooldownTime;
//        button.interactable = false;
//        while (remainingTime > 0)
//        {
//            buttonText.text = $"{remainingTime:F1}";
//            remainingTime -= Time.deltaTime;
//            yield return null;
//        }
//        buttonText.text = "";
//        button.interactable = true;
//    }

//    // Xóa item khỏi inventory
//    public void RemoveItem(ItemClass item, int quantity)
//    {
//        for (int i = 0; i < items.Length; i++)
//        {
//            if (items[i].GetItem() != null && items[i].GetItem().itemId == item.itemId)
//            {
//                items[i].RemoveItem(quantity);  // RemoveItem đã sửa thành có tham số quantity
//                RefreshUI();
//                break;
//            }
//        }
//    }
//}
