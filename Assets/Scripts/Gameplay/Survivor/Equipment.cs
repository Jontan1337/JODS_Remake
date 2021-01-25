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

    private int equipmentSlotsCount = 0;
    private Action onSelectedEquipmentSlotChange;

    public EquipmentSlot SelectedEquipmentBar
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

    private void Awake()
    {
        equipmentSlotsCount = EquipmentSlots.Count;
    }

    //[Server]
    public void Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        // If selected equipment bar is empty, equip item in that bar,
        // else look for an available bar.
        if (selectedEquipmentSlot.Equipment == null)
        {
            // If selected bar can't equip, then equip on first 
            if (!selectedEquipmentSlot.Svr_Equip(equipment, equipmentType))
            {
                selectedEquipmentSlot = Svr_GetAvailableSlot(equipmentType);
            }
        }
        else
        {
            for (int i = 0; i < equipmentSlotsCount-1; i++)
            {
                //if (EquipmentBars[i].Equip(equipment, equipmentType))
                //{

                //}
            }
        }
    }

    // Finds and returns the first bar that has no equipment.
    // If none are available, it returns the currently selected bar.
    //[Server]
    private EquipmentSlot Svr_GetAvailableSlot(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount-1; i++)
        {
            EquipmentSlot currentBar = EquipmentSlots[i];
            if (currentBar.Equipment != null
                && currentBar.EquipmentType == equipmentType) return EquipmentSlots[i];
        }
        return selectedEquipmentSlot;
    }
}
