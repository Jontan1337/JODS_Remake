using UnityEngine;
using Mirror;

public class EquipmentSlot : NetworkBehaviour
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
    public EquipmentType EquipmentType => equipmentType;

    //private void Awake()
    //{
        
    //}

    //public void SendMessage(short )
    //{
    //    NetworkWriter nw = new NetworkWriter();

    //    nw.WriteBytes();
    //}

    public bool Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (equipmentType == EquipmentType)
        {
            Equipment = equipment;
            return true;
        }
        return false;
    }

    public bool Drop(Transform dropTransform)
    {
        return true;
    }
}
