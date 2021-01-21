using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Equipment : NetworkBehaviour
{
    public EquipmentSlot selectedEquipmentBar = null;

    public List<EquipmentSlot> EquipmentBars = new List<EquipmentSlot>();

    private int equipmentBarsCount = 0;
    private void Awake()
    {
        equipmentBarsCount = EquipmentBars.Count;
    }

    [Server]
    public void Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        // If selected equipment bar is empty, equip item in that bar,
        // else look for an available bar.
        if (selectedEquipmentBar.Equipment == null)
        {
            // If selected bar can't equip, then equip on first 
            if (!selectedEquipmentBar.Equip(equipment, equipmentType))
            {
                selectedEquipmentBar = GetAvailableBar(equipmentType);
            }

        }
        else
        {
            for (int i = 0; i < equipmentBarsCount-1; i++)
            {
                //if (EquipmentBars[i].Equip(equipment, equipmentType))
                //{

                //}
            }
        }
    }

    // Finds and returns the first bar that has no equipment.
    // If none are available, it returns the currently selected bar.
    private EquipmentSlot GetAvailableBar(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentBarsCount-1; i++)
        {
            EquipmentSlot currentBar = EquipmentBars[i];
            //if (currentBar.Equipment != null 
            //    && currentBar.EquipmentType == equipmentType) return EquipmentBars[i];
        }
        return selectedEquipmentBar;
    }
}
