using UnityEngine;
using Mirror;

public class EquipmentSlot : NetworkBehaviour
{
    public string slotName;

    [SerializeField]
    private GameObject equipment;
    [SerializeField]
    private EquipmentType equipmentType;

    public GameObject Equipment
    {
        get => equipment;
        private set => equipment = value;
    }
    public EquipmentType EquipmentType => equipmentType;

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        Debug.Log("OnSerialize!");

        writer.WriteGameObject(equipment);
        writer.WriteString(equipmentType.ToString());

        return true;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        Debug.Log("OnDeserialize!");

        reader.ReadGameObject();
        reader.ReadString();
    }
    #endregion

    //[Server]
    public bool Svr_Equip(GameObject equipment, EquipmentType equipmentType)
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
        Debug.Log("Drop");
        return true;
    }
}
