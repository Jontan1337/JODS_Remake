using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public EquipmentBar selectedEquipmentBar = null;

    public List<EquipmentBar> EquipmentBars = new List<EquipmentBar>();

    private int equipmentBarsCount = 0;
    private void Awake()
    {
        equipmentBarsCount = EquipmentBars.Count;
    }

    public void Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (selectedEquipmentBar.equipment == null)
        {
            selectedEquipmentBar.Equip(equipment, equipmentType);
        }
        else
        {
            for (int i = 0; i < equipmentBarsCount; i++)
            {
                if (EquipmentBars[i].Equip(equipment, equipmentType))
                {

                }
            }
        }
    }
}
