using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    private GameObject equipment;
    public string barName;

    [SerializeField]
    private EquipmentType equipmentType;

    public GameObject Equipment
    {
        get => equipment;
        private set => equipment = value;
    }
    //public EquipmentType EquipmentType => equipmentType;

    //public bool Equip(GameObject equipment, EquipmentType equipmentType)
    //{
    //    if (equipmentType == EquipmentType)
    //    {
    //        Equipment = equipment;
    //        return true;
    //    }
    //    return false;
    //}

    public bool Drop(Transform dropTransform)
    {
        return true;
    }
}
