using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject slotsHolder;

    [SerializeField]
    private ItemClass itemToAdd;

    [SerializeField]
    private ItemClass itemToRemove;

    [SerializeField]
    private SlotClass[] items;

    [SerializeField]
    private SlotClass[] startingItems;

    [SerializeField]
    private SlotClass movingSlot;

    [SerializeField]
    private SlotClass originalSlot;

    [SerializeField]
    private SlotClass tempSlot;

    public Image itemCursor;

    [SerializeField]
    private GameObject[] slots;
    public bool isMoving;

    // use item
    [Header("Use Item")]
    [SerializeField]
    private Button Btn_Health;

    [SerializeField]
    private Button Btn_Mana;

    [SerializeField]
    private ItemClass healthItem;

    [SerializeField]
    private ItemClass manaItem;

    [SerializeField]
    public Slider healthSlider;

    [SerializeField]
    public Slider manaSlider;

    [SerializeField]
    private TextMeshProUGUI healthButtonText;

    [SerializeField]
    private TextMeshProUGUI manaButtonText;
    private float itemCooldownTime = 2f;
    private bool isHealthOnCooldown;
    private bool isManaOnCooldown;

    // Start is called before the first frame update
    void Start()
    {
        Btn_Health.onClick.AddListener(() => UseHealth(healthItem));
        Btn_Mana.onClick.AddListener(() => UseMana(manaItem));
        healthButtonText.text = "";
        manaButtonText.text = "";

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

        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }

        originalSlot = new SlotClass();
        movingSlot = new SlotClass();
        tempSlot = new SlotClass();

        RefreshUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isMoving)
            {
                EndMove();
            }
            else
            {
                BeginMove();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isMoving)
            {
                // EndMove();
            }
            else
            {
                BeginSplit();
            }
        }

        if (isMoving)
        {
            itemCursor.enabled = true;
            itemCursor.transform.position = Input.mousePosition;
            itemCursor.sprite = movingSlot.GetItem().itemIcon;
        }
        else
        {
            itemCursor.enabled = false;
            itemCursor.sprite = null;
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i]
                    .GetItem()
                    .itemIcon;

                if (!items[i].GetItem().isStackable)
                {
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }
                else
                {
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = items[i]
                        .GetQuantity()
                        .ToString();
                    if (items[i].GetItem() == healthItem)
                    {
                        UpdateButtonQuantity(Btn_Health, items[i].GetItem());
                    }
                    else if (items[i].GetItem() == manaItem)
                    {
                        UpdateButtonQuantity(Btn_Mana, items[i].GetItem());
                    }
                }
            }
            catch
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

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

        FirebaseInventoryManager1 firebaseInventory = FindObjectOfType<FirebaseInventoryManager1>();
        if (firebaseInventory != null)
        {
            firebaseInventory.AddItemToFirebase(item, quantity);
        }

        RefreshUI();
    }

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

            FirebaseInventoryManager1 firebaseInventory =
                FindObjectOfType<FirebaseInventoryManager1>();
            if (firebaseInventory != null)
            {
                firebaseInventory.RemoveItemFromFirebase(item, quantity);
            }
        }

        RefreshUI();
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

    private SlotClass GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 64)
            {
                return items[i];
            }
        }
        return null;
    }

    private void BeginMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return;

        movingSlot.AddItem(originalSlot.GetItem(), originalSlot.GetQuantity());
        originalSlot.RemoveItem();

        isMoving = true;
        RefreshUI();
    }

    private void BeginSplit()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return;

        if (originalSlot.GetQuantity() <= 1)
            return;

        movingSlot.AddItem(
            originalSlot.GetItem(),
            Mathf.CeilToInt(originalSlot.GetQuantity() / 2f)
        );
        originalSlot.SubQuantity(Mathf.CeilToInt(originalSlot.GetQuantity() / 2f));

        isMoving = true;
        RefreshUI();
    }

    private void EndMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
        {
            AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
        }
        else
        {
            if (originalSlot.GetItem() != null)
            {
                if (originalSlot.GetItem() == movingSlot.GetItem())
                {
                    if (originalSlot.GetItem().isStackable)
                    {
                        int itemMaxStack = originalSlot.GetItem().maxStackQuantity;
                        int count = originalSlot.GetQuantity() + movingSlot.GetQuantity();

                        if (count > itemMaxStack)
                        {
                            int remain = count - itemMaxStack;
                            originalSlot.SetQuantity(itemMaxStack);
                            movingSlot.SetQuantity(remain);

                            isMoving = true;
                            RefreshUI();
                            return;
                        }
                        else
                        {
                            originalSlot.AddQuantity(movingSlot.GetQuantity());
                            movingSlot.RemoveItem();
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    tempSlot.AddItem(originalSlot.GetItem(), originalSlot.GetQuantity());
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());
                    tempSlot.RemoveItem();

                    RefreshUI();
                }
            }
            else
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.RemoveItem();
            }
        }

        isMoving = false;
        RefreshUI();
    }

    public void UseHealth(ItemClass item)
    {
        SlotClass slot = ContainsItem(item);
        if (slot == null || slot.GetQuantity() <= 0)
        {
            Debug.Log("Không có vật phẩm Health để sử dụng!");
            return;
        }

        if (item is ConsumableClass consumable)
        {
            if (PlayerStats.Instance.hp < PlayerStats.Instance.maxHp)
            {
                PlayerStats.Instance.hp = Mathf.Min(
                    PlayerStats.Instance.hp + 50,
                    PlayerStats.Instance.maxHp
                );
                healthSlider.value = PlayerStats.Instance.hp;
                PlayerStats.Instance.SaveStats();
                RemoveItem(item, 1);

                FirebaseInventoryManager1 firebaseInventory =
                    FindObjectOfType<FirebaseInventoryManager1>();
                if (firebaseInventory != null)
                {
                    firebaseInventory.RemoveItemFromFirebase(item, 1);
                }

                UpdateButtonQuantity(Btn_Health, item);
                StartCoroutine(ItemCooldown(Btn_Health, healthButtonText, true));
            }
            else
            {
                Debug.Log("Máu của bạn đã đầy!");
            }
        }
    }

    public void UseMana(ItemClass item)
    {
        SlotClass slot = ContainsItem(item);
        if (slot == null || slot.GetQuantity() <= 0)
        {
            Debug.Log("Không có vật phẩm Mana để sử dụng!");
            return;
        }

        if (item is ConsumableClass consumable)
        {
            if (PlayerStats.Instance.mana < PlayerStats.Instance.maxMana)
            {
                PlayerStats.Instance.mana = Mathf.Min(
                    PlayerStats.Instance.mana + 50,
                    PlayerStats.Instance.maxMana
                );
                manaSlider.value = PlayerStats.Instance.mana;
                PlayerStats.Instance.SaveStats();

                RemoveItem(item, 1);

                FirebaseInventoryManager1 firebaseInventory =
                    FindObjectOfType<FirebaseInventoryManager1>();
                if (firebaseInventory != null)
                {
                    firebaseInventory.RemoveItemFromFirebase(item, 1);
                }

                UpdateButtonQuantity(Btn_Mana, item);
                StartCoroutine(ItemCooldown(Btn_Mana, manaButtonText, false));
            }
            else
            {
                Debug.Log("Mana của bạn đã đầy!");
            }
        }
    }

    private void UpdateButtonQuantity(Button button, ItemClass item)
    {
        SlotClass slot = ContainsItem(item);
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (slot != null)
        {
            int quantity = slot.GetQuantity();
            if (buttonText != null)
            {
                if (quantity > 0)
                {
                    buttonText.text = quantity.ToString();
                }
                else
                {
                    buttonText.text = "0";
                }
            }
        }
        else
        {
            if (buttonText != null)
            {
                buttonText.text = "0";
            }
        }
    }

    private IEnumerator ItemCooldown(Button button, TextMeshProUGUI buttonText, bool isHealth)
    {
        float remainingTime = itemCooldownTime;
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            button.interactable = false;
            buttonText.text = Mathf.Ceil(remainingTime).ToString();
            yield return null;
        }

        button.interactable = true;
        buttonText.text = "";

        if (isHealth)
        {
            isHealthOnCooldown = false;
        }
        else
        {
            isManaOnCooldown = false;
        }
    }
}
