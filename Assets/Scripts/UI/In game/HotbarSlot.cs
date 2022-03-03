using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text textItemName;
    [SerializeField] private TMP_Text textItemType;

    public void SetItemName(string itemName)
    {
        textItemName.text = itemName;
    }
    public void SetItemType(string itemType)
    {
        textItemType.text = itemType;
    }
}
