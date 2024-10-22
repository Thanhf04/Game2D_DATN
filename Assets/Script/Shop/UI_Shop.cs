using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform ShopItem;

    private void Awake()
    {
        container = transform.Find("Container");
        ShopItem = container.Find("ShopItem");
        ShopItem.gameObject.SetActive(false);
    }
    private void Start()
    {
        CreateItemButton(Model_Shop.ItemType.HP_1, Model_Shop.GetSprite(Model_Shop.ItemType.HP_1), "Bình hồi HP", Model_Shop.GetCost(Model_Shop.ItemType.HP_1), 100);
        CreateItemButton(Model_Shop.ItemType.MP_1, Model_Shop.GetSprite(Model_Shop.ItemType.MP_1), "Bình hồi MP", Model_Shop.GetCost(Model_Shop.ItemType.MP_1), 150);
    }
    private void CreateItemButton(Model_Shop.ItemType itemType, Sprite itemSprite, string itemName, int itemPrice, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(ShopItem, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 100f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, shopItemHeight - positionIndex);

        shopItemTransform.Find("Title_Item").GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find("PriceText_Item").GetComponent<TextMeshProUGUI>().SetText(itemPrice.ToString());
        shopItemTransform.Find("Image_Item").GetComponent<Image>().sprite = itemSprite;

        Button buttonBuyItem = shopItemTransform.GetComponent<Button>();
        buttonBuyItem.onClick.AddListener(() =>
        {
            TryItem(itemType);
        });

    }

    public void TryItem(Model_Shop.ItemType itemType)
    {
        if (UI_Coin.Instance.SubTractCoins(Model_Shop.GetCost(itemType), itemType))
        {
            UI_Coin.Instance.CoinChanged?.Invoke(UI_Coin.Instance.GetCurrentCoins());
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
