using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool weaponSlot = false;
    [Space]
    [SerializeField] private bool purchasable = false;
    [Space]
    [SerializeField] private WeaponShop shop = null;
    [SerializeField] private Text priceText = null;
    [SerializeField] private Image itemSprite = null;

    private Button button;

    private ShopItem item;
    public ShopItem Item
    {
        get 
        { 
            return item; 
        }
        set 
        { 
            item = value;
            priceText.text = item.shopItemPrice.ToString();
            itemSprite.sprite = item.shopItemPrefab.GetComponent<EquipmentItem>().UISilhouette;
        }
    }

    public void OnButtonClick()
    {
        shop.BuyItem(shop.GetSlotIndex(this));
    }

    public void EnableShopButton(bool enable)
    {
        button = GetComponent<Button>();
        button.interactable = enable;
        purchasable = enable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (weaponSlot && purchasable)
        {
            shop.ShowWeaponInfo(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (weaponSlot && purchasable)
        {
            shop.ShowWeaponInfo(null);
        }
    }
}
