using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;
    [SerializeField] private SlotClass[] items;
    [SerializeField] private SlotClass[] startingItems;

    [SerializeField] private SlotClass movingSlot;
    [SerializeField] private SlotClass originalSlot;
    [SerializeField] private SlotClass tempSlot;

    public Image itemCursor;

    [SerializeField] private GameObject[] slots;
    public bool isMoving;

    // use item
    [Header("Use Item")]
    Player player;
    [SerializeField] private Button Btn_Health;
    [SerializeField] private Button Btn_Mana;
    [SerializeField] private ItemClass healthItem;
    [SerializeField] private ItemClass manaItem;
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider manaSlider;
    [SerializeField] private TextMeshProUGUI healthButtonText; // Tham chiếu đến TextMeshPro trên nút Health
    [SerializeField] private TextMeshProUGUI manaButtonText;   // Tham chiếu đến TextMeshPro trên nút Mana
    private float itemCooldownTime = 2f;
    private bool isHealthOnCooldown = false;
    private bool isManaOnCooldown = false;


    // Start is called before the first frame update
    void Start()
    {
        //use item 
        Btn_Health.onClick.AddListener(() => UseHealth(healthItem));
        Btn_Mana.onClick.AddListener(() => UseMana(manaItem));
        healthButtonText.text = "";
        manaButtonText.text = "";
        //
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


        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null)
            {
                return;
                return;
            }
        }
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
                //EndMove();
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

    private void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;
                if (!items[i].GetItem().isStackable)
                {
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }
                else
                {
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = items[i].GetQuantity() + "";
                    //use item
                    if (items[i].GetItem() == healthItem) // Nếu item là healthItem
                    {
                        UpdateButtonQuantity(Btn_Health, items[i].GetItem());
                    }
                    else if (items[i].GetItem() == manaItem) // Nếu item là manaItem
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

        RefreshUI();
    }

    public void RemoveItem(ItemClass item, int quanity)
    {
        SlotClass temp = ContainsItem(item);
        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
            {
                temp.SubQuantity(quanity);
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
        }
        else
        {
            return;
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

        if (originalSlot == null || originalSlot.GetItem() == null) return;

        movingSlot.AddItem(
            originalSlot.GetItem(),
            originalSlot.GetQuantity());
        originalSlot.RemoveItem();

        isMoving = true;
        RefreshUI();
        return;
    }

    private void BeginSplit()
    {
        originalSlot = GetClosestSlot();

        if (originalSlot == null || originalSlot.GetItem() == null) return;
        if (originalSlot.GetQuantity() <= 1)
        {
            return;
        }

        movingSlot.AddItem(originalSlot.GetItem(), Mathf.CeilToInt(originalSlot.GetQuantity() / 2f));

        originalSlot.SubQuantity(Mathf.CeilToInt(originalSlot.GetQuantity() / 2f));

        isMoving = true;
        RefreshUI();
        return;
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
                //If slot is the same item
                if (originalSlot.GetItem() == movingSlot.GetItem())
                {
                    //If slot item is stackable
                    if (originalSlot.GetItem().isStackable)
                    {
                        int itemMaxStack = originalSlot.GetItem().maxStackQuantity; //Apple: 20
                        int count = originalSlot.GetQuantity() + movingSlot.GetQuantity();// 25

                        if (count > itemMaxStack)
                        {
                            int remain = count - itemMaxStack; //5
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
                    //Swap
                    tempSlot.AddItem(originalSlot.GetItem(), originalSlot.GetQuantity());
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());
                    tempSlot.RemoveItem();

                    RefreshUI();
                    return;
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
        return;
    }
    // use item
    public void UseHealth(ItemClass item)
    {
        SlotClass slot = ContainsItem(item);
        if (slot != null && slot.GetQuantity() > 0)
        {
            if (item is ConsumableClass consumable)
            {
                if (player.Health < player.MaxHealth)
                {
                    player.Health = Mathf.Min(player.Health + 50, player.MaxHealth);
                    healthSlider.value = player.Health;
                    RemoveItem(item, 1);
                    UpdateButtonQuantity(Btn_Health, item);
                    RefreshUI();
                    StartCoroutine(ItemCooldown(Btn_Health, healthButtonText, true));
                }
            }

            RefreshUI();
        }
    }
    public void UseMana(ItemClass item)
    {
        SlotClass slot = ContainsItem(item);
        if (slot != null && slot.GetQuantity() > 0)
        {
            if (item is ConsumableClass consumable)
            {
                if (player.Mana < player.MaxMana)
                {
                    player.Mana = Mathf.Min(player.Mana + 50, player.MaxMana);
                    manaSlider.value = player.Mana;
                    RemoveItem(item, 1);
                    UpdateButtonQuantity(Btn_Mana, item);
                    RefreshUI();
                    StartCoroutine(ItemCooldown(Btn_Mana, manaButtonText, true));

                }
            }
            RefreshUI();
        }
    }
    private void UpdateButtonQuantity(Button button, ItemClass item)
    {
        // Kiểm tra số lượng còn lại của item
        SlotClass slot = ContainsItem(item);

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (slot != null)
        {
            // Lấy số lượng còn lại
            int quantity = slot.GetQuantity();

            if (buttonText != null)
            {
                // Cập nhật số lượng item trên button
                if (quantity > 0)
                {
                    buttonText.text = quantity.ToString(); // Hiển thị số lượng còn lại
                }

            }
        }
        else
        {
            // Nếu không tìm thấy item, đặt số lượng là 0
            if (buttonText != null)
            {
                buttonText.text = "0"; // Đặt số lượng là 0 khi item không có trong túi
            }
        }
    }
    private IEnumerator ItemCooldown(Button button, TextMeshProUGUI buttonText, bool isHealth)
    {
        float remainingTime = itemCooldownTime;

        // Trong khi còn thời gian hồi chiêu
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;  // Giảm thời gian còn lại
            button.interactable = false; // Tắt tương tác với nút
            buttonText.text = Mathf.Ceil(remainingTime).ToString(); // Cập nhật thời gian còn lại lên nút

            yield return null; // Chờ đến frame tiếp theo
        }

        // Sau khi hết thời gian hồi chiêu
        button.interactable = true; // Bật lại nút
        buttonText.text = ""; // Hoặc có thể là "Use" tùy vào tình huống

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