﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform ShopItem;
    private InventoryManager inventoryManager;
    private Dictionary<Model_Shop.ItemType, ConsumableClass> itemTypeToConsumable;

    [SerializeField]
    private UI_Coin uiCoin;

    private void Awake()
    {
        if (uiCoin == null)
        {
            Debug.LogError("UI_Coin không được tìm thấy trong scene!");
        }
        container = transform.Find("Container");
        ShopItem = container.Find("ShopItem");
        ShopItem.gameObject.SetActive(false);

        inventoryManager = FindObjectOfType<InventoryManager>();

        // Ánh xạ Model_Shop.ItemType với các ConsumableClass
        itemTypeToConsumable = new Dictionary<Model_Shop.ItemType, ConsumableClass>
        {
            { Model_Shop.ItemType.HP_1, Resources.Load<ConsumableClass>("Items/HP_1") },
            { Model_Shop.ItemType.MP_1, Resources.Load<ConsumableClass>("Items/MP_1") }
        };
    }

    private void Start()
    {
        CreateItemButton(
            Model_Shop.ItemType.HP_1,
            Model_Shop.GetSprite(Model_Shop.ItemType.HP_1),
            "Bình hồi HP",
            Model_Shop.GetCost(Model_Shop.ItemType.HP_1),
            75
        );
        CreateItemButton(
            Model_Shop.ItemType.MP_1,
            Model_Shop.GetSprite(Model_Shop.ItemType.MP_1),
            "Bình hồi MP",
            Model_Shop.GetCost(Model_Shop.ItemType.MP_1),
            130
        );
    }

    private void CreateItemButton(
        Model_Shop.ItemType itemType,
        Sprite itemSprite,
        string itemName,
        int itemPrice,
        int positionIndex
    )
    {
        Transform shopItemTransform = Instantiate(ShopItem, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 100f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, shopItemHeight - positionIndex);

        shopItemTransform.Find("Title_Item").GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform
            .Find("PriceText_Item")
            .GetComponent<TextMeshProUGUI>()
            .SetText(itemPrice.ToString());
        shopItemTransform.Find("Image_Item").GetComponent<Image>().sprite = itemSprite;

        Button buttonBuyItem = shopItemTransform.GetComponent<Button>();
        buttonBuyItem.onClick.AddListener(() =>
        {
            TryItem(itemType);
        });
    }

    public void TryItem(Model_Shop.ItemType itemType)
    {
        if (uiCoin.SubTractCoins(Model_Shop.GetCost(itemType), itemType))
        {
            uiCoin.CoinChanged?.Invoke(uiCoin.GetCurrentCoins());

            // Thêm item vào inventory sau khi mua
            if (itemTypeToConsumable.TryGetValue(itemType, out ConsumableClass consumableItem))
            {
                inventoryManager.AddItem(consumableItem, 1);
            }
            else
            {
                Debug.LogError("Không tìm thấy item trong từ điển ánh xạ.");
            }
        }
    }
}
