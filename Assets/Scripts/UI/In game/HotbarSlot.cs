using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] private Image hotbarItemImage = null;
    [SerializeField] private TMP_Text textItemName = null;
    [SerializeField] private TMP_Text textItemType = null;

    public void SetItemImage(Sprite sprite)
    {
        hotbarItemImage.sprite = sprite;
    }
    public void ToggleItemImage()
    {
        hotbarItemImage.enabled = !hotbarItemImage.enabled;
    }
    public void SetItemName(string itemName)
    {
        textItemName.text = itemName;
    }
    public void SetItemType(string itemType)
    {
        textItemType.text = itemType;
    }
}
