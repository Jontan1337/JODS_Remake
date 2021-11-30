using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool weaponSlot;
    [Space]
    [SerializeField] private bool purchasable;
    [Space]
    [SerializeField] private WeaponShop shop;
    [SerializeField] private Text priceText;
    [SerializeField] private Text PLACEHOLDERText;

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
            PLACEHOLDERText.text = "PLACEHOLDER TEXT: " + item.shopItemName
                + "\nREPLACE WITH IMAGE";
        }
    }

    public void OnButtonClick()
    {
        Debug.Log("TODO: Points system. Make a check here to see if player has enough points to buy the item");

        //If player has enough points
        //Buy the item
        shop.Cmd_BuyItem(shop.GetSlotIndex(this));
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
