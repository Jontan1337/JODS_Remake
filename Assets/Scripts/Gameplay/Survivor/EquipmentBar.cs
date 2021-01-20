using UnityEngine;

public class EquipmentBar : MonoBehaviour
{
    public GameObject equipment;
    public string barName = "Bar Name";

    [SerializeField]
    private EquipmentType equipmentType;

    public bool Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (equipmentType == this.equipmentType)
        {
            this.equipment = equipment;
            return true;
        }
        return false;
    }
}
