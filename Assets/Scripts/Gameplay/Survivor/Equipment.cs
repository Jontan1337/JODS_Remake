using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Equipment : NetworkBehaviour
{
    [SerializeField]
    private EquipmentSlot selectedEquipmentSlot;

    [SerializeField]
    private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    [Header("Equipment slot setup settings")]
    [SerializeField, Tooltip("The parent transform, where the equipment slots should be instantiated.")]
    private Transform equipmentSlotsParent;
    [SerializeField, Tooltip("The prefab of the equipment slots.")]
    private GameObject equipmentSlotPrefab;
    [SerializeField, Tooltip("A list of the equipment types, the player should have.")]
    private List<EquipmentType> equipmentSlotsTypes = new List<EquipmentType>();

    private int equipmentSlotsCount = 0;
    private Action onSelectedEquipmentSlotChange;

    public EquipmentSlot SelectedEquipmentSlot
    {
        get => selectedEquipmentSlot;
        private set
        {
            selectedEquipmentSlot = value;
            onSelectedEquipmentSlotChange?.Invoke();
        }
    }
    public List<EquipmentSlot> EquipmentSlots
    {
        get => equipmentSlots;
        private set => equipmentSlots = value;
    }

    public override void OnStartAuthority()
    {
        Cmd_EquipmentSlotsSetup();
    }

    public override void OnStartClient()
    {
        equipmentSlotsCount = EquipmentSlots.Count;
        if (hasAuthority)
        {
            Cmd_SelectSlot(0);
        }
    }

    [Server]
    public void Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        // If selected equipment bar is empty, equip item in that bar,
        // else look for an available bar.
        if (selectedEquipmentSlot.Equipment == null)
        {
            // If selected bar can't equip, then equip on first 
            if (!selectedEquipmentSlot.Svr_Equip(equipment, equipmentType))
            {
                selectedEquipmentSlot = equipmentSlots[0];
            }
        }
        else
        {
            selectedEquipmentSlot = Svr_GetAvailableSlot(equipmentType);
        }

        selectedEquipmentSlot.Svr_Equip(equipment, equipmentType);
    }

    // Finds and returns the first bar that has no equipment.
    // If none are available, it returns the currently selected bar.
    [Server]
    private EquipmentSlot Svr_GetAvailableSlot(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount-1; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.Equipment == null && currentSlot.EquipmentType == equipmentType)
                return EquipmentSlots[i];
        }
        return SelectedEquipmentSlot;
    }

    [Command]
    public void Cmd_SelectSlot(int slotIndex)
    {
        Svr_SelectSlot(slotIndex);
    }

    [Server]
    public void Svr_SelectSlot(int slotIndex)
    {
        SelectedEquipmentSlot = equipmentSlots[slotIndex];
    }

    [Command]
    private void Cmd_EquipmentSlotsSetup()
    {
        foreach (EquipmentType type in equipmentSlotsTypes)
        {
            GameObject hotbarSlot = Instantiate(equipmentSlotPrefab, equipmentSlotsParent);
            NetworkServer.Spawn(hotbarSlot, gameObject);
            hotbarSlot.GetComponent<EquipmentSlot>().EquipmentType = type;
            equipmentSlots.Add(hotbarSlot.GetComponent<EquipmentSlot>());
        }
    }
}