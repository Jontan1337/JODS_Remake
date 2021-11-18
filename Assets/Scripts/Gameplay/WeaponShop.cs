﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ShopItem
{
    public string shopItemName;
    public GameObject shopItemPrefab;
    public int shopItemPrice;
}
public class WeaponShop : NetworkBehaviour, IInteractable
{
    [Header("Shop Selection")]
    [SerializeField] private List<ShopItem> currentWeaponSelection = new List<ShopItem>();
    [SerializeField] private List<ShopItem> currentEquipmentSelection = new List<ShopItem>();
    [Space]
    [SerializeField] private int minimumAmountOfMeleeWeapons = 1;
    [SerializeField] private int minimumAmountOfHealingItems = 2;

    private int currentSelectionMeleeWeapons = 0;
    private int currentSelectionHealingItems = 0;

    [Header("Weapon database")]
    [SerializeField] private List<ShopItem> rangedWeapons = new List<ShopItem>();
    [Space]
    [SerializeField] private List<ShopItem> meleeWeapons = new List<ShopItem>();
    [Space]
    [SerializeField] private List<ShopItem> healingEquipment = new List<ShopItem>();
    [Space]
    [SerializeField] private List<ShopItem> equipment = new List<ShopItem>();

    [Header("References")]
    [SerializeField] private UIShopButton[] weaponSlots = null;
    [SerializeField] private UIShopButton[] equipmentSlots = null;
    [Space]
    [SerializeField] private GameObject shopCanvas = null;
    [SerializeField] private Transform weaponSlotsContainer = null;
    [SerializeField] private Transform equipmentSlotsContainer = null;
    [SerializeField] private GameObject shopUISlotPrefab = null;
    [Space]
    [SerializeField] private GameObject weaponStats = null;
    [SerializeField] private Text weaponNameText = null;
    [SerializeField] private Text weaponDamageValueText = null;
    [SerializeField] private Text weaponDamageTypeValueText= null;
    [SerializeField] private Text weaponDamageTypeText= null;

    private bool inShop = false;

    [Header("Debug")]
    [SerializeField] private bool test = false;

    public bool IsInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public string ObjectName => throw new System.NotImplementedException();

    private void Start()
    {
        weaponStats.SetActive(false);
        shopCanvas.SetActive(false);
        if (test)
        {
            GenerateNewSelection();
        }
    }

    public void Svr_Interact(GameObject interacter)
    {
        print("tf is goign fonb");
        inShop = !inShop;
        shopCanvas.SetActive(inShop);
        Cursor.lockState =  inShop ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = inShop;
        if (inShop) { EnterShop(); }
        else { ExitShop(); }
    }

    private void EnterShop()
    {
        JODSInput.Controls.Disable();

        if (currentEquipmentSelection.Count > 0)
        {
            SetShopSlotValues(currentEquipmentSelection, false);
        }
        else { Debug.LogError("Something went wrong. Shop opened without any equipment to be purchased. " +
            "This should not be possible"); }


        if (currentWeaponSelection.Count > 0)
        {
            SetShopSlotValues(currentWeaponSelection, true);
        }
        else
        {
            Debug.LogError("Something went wrong. Shop opened without any weapons to be purchased. " +
         "This should not be possible");
        }
    }

    private void SetShopSlotValues(List<ShopItem> shopItems, bool weaponSlotItems)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            var item = shopItems[i];

            UIShopButton newSlot = weaponSlotItems ? weaponSlots[i] : equipmentSlots[i];
            newSlot.Item = item;
        }
    }

    private void ExitShop()
    {
        JODSInput.Controls.Enable();
    }

    public void ShowWeaponInfo(ShopItem item)
    {
        if (item == null)
        {
            weaponStats.SetActive(false);
            return;
        }
        else
        {
            weaponNameText.text = item.shopItemName;

            bool isMelee = item.shopItemPrefab.TryGetComponent(out MeleeWeapon meleeComponent);
            bool isRanged = item.shopItemPrefab.TryGetComponent(out RangedWeapon rangedComponent);

            weaponDamageTypeText.text = isMelee ? "Damage Type" : "Ammo Type";
            weaponDamageTypeValueText.text = isMelee ? meleeComponent.DamageType.ToString() : rangedComponent.AmmunitionType.ToString();

            weaponDamageValueText.text = isMelee ? meleeComponent.Damage.ToString() : rangedComponent.Damage.ToString();

            weaponStats.SetActive(true);
        }
    }



    private void GenerateNewSelection()
    {
        //Reset
        currentWeaponSelection.Clear();
        currentEquipmentSelection.Clear();

        currentSelectionHealingItems = 0;
        currentSelectionMeleeWeapons = 0;

        //Add new items
        while (currentWeaponSelection.Count < weaponSlots.Length)
        {
            //First add the required amount of melee weapons to the selection (default 1)
            if (currentSelectionMeleeWeapons < minimumAmountOfMeleeWeapons)
            {
                currentWeaponSelection.Add(meleeWeapons[Random.Range(0, meleeWeapons.Count)]);
                currentSelectionMeleeWeapons++;
                continue;
            }

            //Then generate random weapons

            // 25% chance to be a melee weapon
            // 75% chance to be a ranged weapon
            bool newMeleeWeapon = Random.Range(1, 5) == 4;

            currentWeaponSelection.Add(newMeleeWeapon ?
                meleeWeapons[Random.Range(0, meleeWeapons.Count)] :
                rangedWeapons[Random.Range(0, rangedWeapons.Count)]);
        }

        while (currentEquipmentSelection.Count < equipmentSlots.Length)
        {
            //First add the required amount of healing items to the selection (default 2)
            if (currentSelectionHealingItems < minimumAmountOfHealingItems)
            {
                currentEquipmentSelection.Add(healingEquipment[Random.Range(0, healingEquipment.Count)]);
                currentSelectionHealingItems++;
                continue;
            }

            //Then generate random equipment

            // 25% chance to be a healing item
            // 75% chance to be any other item
            bool newHealingItem = Random.Range(1, 5) == 4;

            currentEquipmentSelection.Add(newHealingItem ?
                healingEquipment[Random.Range(0, healingEquipment.Count)] :
                equipment[Random.Range(0, equipment.Count)]);
        }
    }



    private void OnValidate()
    {
        foreach (var item in rangedWeapons)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }        
        foreach (var item in meleeWeapons)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }        
        foreach (var item in healingEquipment)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }
        foreach (var item in equipment)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }
    }
}
