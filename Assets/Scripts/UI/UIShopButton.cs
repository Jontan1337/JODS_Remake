using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool weaponSlot;
    [Space]
    [SerializeField] private WeaponShop shop;
    [SerializeField] private Text priceText;
    [SerializeField] private Text PLACEHOLDERText;
    [Space]
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


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (weaponSlot)
        {
            shop.ShowWeaponInfo(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (weaponSlot)
        {
            shop.ShowWeaponInfo(null);
        }
    }
}
