using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Update the InventoryFirebaseManager to fix UI refresh issues and handle item display correctly.

public class InventoryFirebaseManager : MonoBehaviour
{
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;
    [SerializeField] private SlotClass[] items;
    [SerializeField] private SlotClass[] startingItems;
    [SerializeField] private ItemClass healthItem;
    [SerializeField] private ItemClass manaItem;

    private DatabaseReference reference;
    private string username;
    public Image itemCursor;
    private bool isMoving;

    // UI references for use item buttons
    [Header("Use Item")]
    [SerializeField] private Button Btn_Health;
    [SerializeField] private Button Btn_Mana;
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider manaSlider;
    [SerializeField] private TextMeshProUGUI healthButtonText;
    [SerializeField] private TextMeshProUGUI manaButtonText;
    private float itemCooldownTime = 2f;
    private bool isHealthOnCooldown = false;
    private bool isManaOnCooldown = false;
    private Dichuyennv1 player1;  // Reference to player object

    void Start()
    {
        // Automatically assign player1 if not already assigned
        player1 = FindObjectOfType<Dichuyennv1>();
        if (player1 == null)
        {
            Debug.LogError("Player1 is not assigned! Make sure the player object with Dichuyennv1 script is in the scene.");
            return;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;

            // Get username from previous login (assumed)
            username = "user123"; // Replace with actual login logic

            // Load inventory data on start
            LoadInventoryData();
        });

        // Setup buttons for using items
        Btn_Health.onClick.AddListener(() => UseHealth(healthItem));
        Btn_Mana.onClick.AddListener(() => UseMana(manaItem));
        healthButtonText.text = "";
        manaButtonText.text = "";

        // Setup initial inventory slots
        int slotCount = slotsHolder.transform.childCount;
        items = new SlotClass[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            items[i] = new SlotClass();
        }

        // Set starting items
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }

        RefreshUI();
    }

    // Save inventory data to Firebase
    public void SaveInventoryData()
    {
        // Create inventory data object
        InventoryData data = new InventoryData(items, healthItem, manaItem);

        // Convert inventory data to JSON format
        string json = JsonUtility.ToJson(data);

        // Save to Firebase under the user's username
        reference.Child("inventory").Child(username).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Inventory data saved to Firebase!");
            }
            else
            {
                Debug.LogError("Failed to save inventory: " + task.Exception);
            }
        });
    }

    // Load inventory data from Firebase
    public void LoadInventoryData()
    {
        reference.Child("inventory").Child(username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    InventoryData data = JsonUtility.FromJson<InventoryData>(json);

                    // Update the inventory with the loaded data
                    items = data.items;
                    healthItem = data.healthItem;
                    manaItem = data.manaItem;

                    RefreshUI();
                    Debug.Log("Inventory data loaded!");
                }
                else
                {
                    Debug.LogWarning("No inventory data found for this user.");
                }
            }
            else
            {
                Debug.LogError("Failed to load inventory: " + task.Exception);
            }
        });
    }

    // Update UI to reflect inventory data
    private void RefreshUI()
    {
        // Iterate through each slot and update the UI accordingly
        for (int i = 0; i < items.Length; i++)
        {
            try
            {
                // Get the item in the slot
                var item = items[i].GetItem();
                var slot = slotsHolder.transform.GetChild(i).gameObject;
                Image itemImage = slot.transform.GetChild(0).GetComponent<Image>(); // Image for item icon
                TextMeshProUGUI quantityText = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); // Quantity text

                // Update item image and quantity text based on the inventory item
                if (item != null)
                {
                    itemImage.sprite = item.itemIcon;
                    itemImage.enabled = true; // Show the item icon
                    quantityText.text = item.isStackable ? items[i].GetQuantity().ToString() : ""; // Display quantity if stackable
                }
                else
                {
                    itemImage.sprite = null;
                    itemImage.enabled = false; // Hide the item icon if there's no item in this slot
                    quantityText.text = ""; // Clear quantity text
                }
            }
            catch
            {
                // Handle potential errors during UI update (e.g., missing slots or items)
                Debug.LogError("Error updating slot UI");
            }
        }
    }

    // Add item to inventory and refresh UI
    public void AddItem(ItemClass item, int quantity)
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

        // Save the updated inventory to Firebase after adding an item
        SaveInventoryData();

        RefreshUI();
    }

    // Remove item from inventory and refresh UI
    public void RemoveItem(ItemClass item, int quantity)
    {
        SlotClass temp = ContainsItem(item);
        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
            {
                temp.SubQuantity(quantity);
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

            // Save the updated inventory to Firebase after removing an item
            SaveInventoryData();
        }

        RefreshUI();
    }

    // Helper method to check if an item already exists in inventory
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

    // Use Health item
    public void UseHealth(ItemClass item)
    {
        if (item is ConsumableClass consumable)
        {
            if (player1.currentHealth < player1.maxHealth)
            {
                player1.currentHealth = Mathf.Min(player1.currentHealth + 50, player1.maxHealth);
                healthSlider.value = player1.currentHealth;
                RemoveItem(item, 1);
                RefreshUI();
            }
        }
    }

    // Use Mana item
    public void UseMana(ItemClass item)
    {
        if (item is ConsumableClass consumable)
        {
            if (player1.currentMana < player1.maxMana)
            {
                player1.currentMana = Mathf.Min(player1.currentMana + 50, player1.maxMana);
                manaSlider.value = player1.currentMana;
                RemoveItem(item, 1);
                RefreshUI();
            }
        }
    }
}

[System.Serializable]
public class InventoryData
{
    public SlotClass[] items;
    public ItemClass healthItem;
    public ItemClass manaItem;

    public InventoryData(SlotClass[] items, ItemClass healthItem, ItemClass manaItem)
    {
        this.items = items;
        this.healthItem = healthItem;
        this.manaItem = manaItem;
    }
}
