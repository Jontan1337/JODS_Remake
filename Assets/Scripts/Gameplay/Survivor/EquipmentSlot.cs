using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class EquipmentSlot : NetworkBehaviour
{
    public string slotName;

    [SerializeField]
    private GameObject equipment;
    [SerializeField]
    private EquipmentType equipmentType;
    [SerializeField]
    private KeyCode keyCode;
    [SerializeField]
    private int keyNumber;

    [Header("References")]
    [SerializeField]
    private TMP_Text textMesh;
    [SerializeField]
    private Image slotImage;

    [Header("Visual settings")]
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color deselectedColor;

    private Equipment playerEquipment;

    public GameObject Equipment
    {
        get => equipment;
        private set
        {
            equipment = value;
            if (Equipment != null)
            {
                textMesh.SetText(Equipment.name);
            }
            else
            {
                textMesh.SetText("Empty");
            }

        }
    }

    public EquipmentType EquipmentType
    {
        get => equipmentType;
        set => equipmentType = value;
    }

    private void Awake()
    {
        keyNumber = transform.GetSiblingIndex() + 1;
        keyCode = KeyCode.Alpha0 + keyNumber;

        playerEquipment = GetComponentInParent<Equipment>();
    }

    public override void OnStartAuthority()
    {
        print("OnStartLocalPlayer");
        JODSInput.Controls.Survivor.Hotbarselecting.performed += number => SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()));
        print(JODSInput.Controls.Survivor.Hotbarselecting.type);
    }

    private void SelectSlot(int index)
    {
        if (index == keyNumber)
        {
            playerEquipment.Svr_SelectSlot(keyNumber-1);
            Selected();
        }
        else
        {
            Deselected();
        }
    }

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        Debug.Log("OnSerialize!");

        writer.WriteGameObject(equipment);
        writer.WriteInt32((int)equipmentType);

        return true;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        Debug.Log("OnDeserialize!");

        equipment = reader.ReadGameObject();
        equipmentType = (EquipmentType)reader.ReadInt32();
    }
    #endregion

    [Server]
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
        if (equipment != null)
        {
            Debug.Log("Drop");
            return true;
        }
        return false;
    }

    private void Selected()
    {
        slotImage.color = selectedColor;
    }
    private void Deselected()
    {
        slotImage.color = deselectedColor;
    }
}
