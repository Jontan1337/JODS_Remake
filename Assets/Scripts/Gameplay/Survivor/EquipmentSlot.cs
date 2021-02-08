using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class EquipmentSlot : NetworkBehaviour
{
    public string slotName;

    [Header("Item info")]
    [SerializeField, SyncVar]
    private GameObject equipmentItem;
    [SerializeField]
    private EquipmentType equipmentType;

    [Header("Item index")]
    [SerializeField]
    private KeyCode keyCode;
    [SerializeField]
    private int keyNumber;

    [Header("Hotbar Slot Visuals")]
    [SerializeField]
    private GameObject uiSlot;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color deselectedColor;
    [SerializeField]
    private TMP_Text textMesh;
    [SerializeField]
    private Image slotImage;

    private Equipment playerEquipment;

    public GameObject UISlot
    {
        get => uiSlot;
        set
        {
            uiSlot = value;
            slotImage = UISlot.GetComponent<Image>();
            textMesh = UISlot.GetComponentInChildren<TMP_Text>();
        }
    }

    #region Properties
    public GameObject EquipmentItem
    {
        get => equipmentItem;
        private set
        {
            if (isServer)
            {
                equipmentItem = value;
            }
            if (hasAuthority)
            {
                if (EquipmentItem != null)
                {
                    textMesh.SetText(EquipmentItem.name);
                }
                else
                {
                    textMesh.SetText("Empty");
                }
            }
        }
    }
    public EquipmentType EquipmentType { get => equipmentType; set => equipmentType = value; }
    public KeyCode KeyCode { get => keyCode; private set => keyCode = value; }
    public int KeyNumber { get => keyNumber; private set => keyNumber = value; }
    public Color SelectedColor { get => selectedColor; private set => selectedColor = value; }
    public Color DeselectedColor { get => deselectedColor; private set => deselectedColor = value; }
    public TMP_Text TextMesh { get => textMesh; private set => textMesh = value; }
    public Image SlotImage { get => slotImage; private set => slotImage = value; }
    #endregion

    private void Awake()
    {
        keyNumber = transform.GetSiblingIndex() + 1;
        keyCode = KeyCode.Alpha0 + keyNumber;
    }

    public override void OnStartAuthority()
    {
        playerEquipment = GetComponentInParent<Equipment>();
    }

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {

        if (initialState)
        {
            //writer.WriteEquipmentSlot(this);
        }
        else
        {
            writer.WriteGameObject(equipmentItem);
            return true;
        }
        return false;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        Debug.Log("OnDeserialize!");

        if (initialState)
        {
            GameObject item = reader.ReadGameObject();
            this.EquipmentItem = item;
            //EquipmentSlot equipmentSlot = reader.ReadEquipmentSlot();
            //this.EquipmentType = equipmentSlot.EquipmentType;
            //this.KeyCode = equipmentSlot.KeyCode;
            //this.KeyNumber = equipmentSlot.KeyNumber;
            //this.SelectedColor = equipmentSlot.SelectedColor;
            //this.DeselectedColor = equipmentSlot.DeselectedColor;
            //this.TextMesh = equipmentSlot.TextMesh;
            //this.SlotImage = equipmentSlot.SlotImage;
        }
        else
        {
            GameObject item = reader.ReadGameObject();
            this.EquipmentItem = item;
        }
    }
    #endregion

    [Server]
    public bool Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (equipmentType == EquipmentType)
        {
            EquipmentItem = equipment;
            return true;
        }
        return false;
    }

    [Server]
    public bool Svr_Drop(Transform dropTransform)
    {
        if (EquipmentItem != null)
        {
            Debug.Log("Drop", this);
            EquipmentItem = null;
            return true;
        }
        return false;
    }

    [TargetRpc]
    public void Rpc_Select(NetworkConnection conn)
    {
        if (slotImage)
            slotImage.color = selectedColor;
    }
    [TargetRpc]
    public void Rpc_Deselect(NetworkConnection conn)
    {
        if (slotImage)
            slotImage.color = deselectedColor;
    }
}

public static class ReadWriteEquipmentSlot
{
    public static void WriteEquipmentSlot(this NetworkWriter writer, EquipmentSlot equipmentSlot)
    {
        NetworkIdentity networkIdentity = equipmentSlot.GetComponent<NetworkIdentity>();
        writer.WriteNetworkIdentity(networkIdentity);
    }
    public static EquipmentSlot ReadEquipmentSlot(this NetworkReader reader)
    {
        NetworkIdentity networkIdentity = reader.ReadNetworkIdentity();
        EquipmentSlot equipmentSlot = networkIdentity.GetComponent<EquipmentSlot>();
        return equipmentSlot;
    }
}
