using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Equipment : NetworkBehaviour
{
    [SerializeField, SyncVar]
    private EquipmentSlot selectedEquipmentSlot;
    [SerializeField]
    private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    [SerializeField, Tooltip("A list of the equipment types, the player should have.")]
    private List<EquipmentType> equipmentSlotsTypes = new List<EquipmentType>();
    [Header("Equipment slot setup settings")]
    [SerializeField, Tooltip("The parent transform, where the equipment slots should be instantiated.")]
    private Transform equipmentSlotsParent;
    [SerializeField, Tooltip("The parent transform, where the equipment slots should be instantiated.")]
    private Transform equipmentSlotsUIParent;
    [SerializeField, Tooltip("The prefab of the equipment slots.")]
    private GameObject equipmentSlotPrefab;
    [SerializeField, Tooltip("The prefab of the equipment slots.")]
    private GameObject equipmentSlotUIPrefab;

    private int equipmentSlotsCount = 0;

    private const string slotsUIParentName = "CanvasInGame/Hotbar";

    private Action onChangeSelectedEquipmentSlot;


    public EquipmentSlot SelectedEquipmentSlot
    {
        get => selectedEquipmentSlot;
        private set
        {
            selectedEquipmentSlot = value;
            onChangeSelectedEquipmentSlot?.Invoke();
        }
    }
    public List<EquipmentSlot> EquipmentSlots
    {
        get => equipmentSlots;
        private set
        {
            equipmentSlots = value;
        }
    }

    private void OnTransformParentChanged()
    {
        if (hasAuthority)
        {
            // When parent changed, check if it's correctly set to player Survivor
            // and then find UI hotbar under Survivors canvas.
            Debug.Log($"My parent {transform.parent}", this);
            if (transform.parent.name.Contains("Survivor"))
            {
                equipmentSlotsUIParent = transform.parent.Find(slotsUIParentName);
            }
        }
    }

    #region NetworkBehaviour Callbacks
    public override void OnStartServer()
    {
        NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
    }
    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Hotbarselecting.performed += number => Cmd_SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()) - 1);
        Cmd_EquipmentSlotsSetup();
    }
    public override void OnStartClient()
    {
        equipmentSlotsParent = transform;
    }

    public override void OnStopServer()
    {
        NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
    }
    public override void OnStopAuthority()
    {
        JODSInput.Controls.Survivor.Hotbarselecting.performed -= number => Cmd_SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()) - 1);
    }
    #endregion

    [TargetRpc]
    private void Rpc_UpdateEquipmentSlots(NetworkConnection conn, EquipmentSlot newEquipment, int value)
    {
        if (value == 1)
        {
            equipmentSlots.Add(newEquipment);
        }
        else if (value == -1)
        {
            equipmentSlots.Remove(newEquipment);
        }
    }

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            if (selectedEquipmentSlot)
            {
                writer.WriteEquipmentSlot(selectedEquipmentSlot);
            }
            //writer.WriteList(equipmentSlots);
            //writer.WriteList(equipmentSlotsTypes);
            //writer.WriteTransform(equipmentSlotsParent);
            //writer.WriteTransform(equipmentSlotsUIParent);
            //writer.WriteGameObject(equipmentSlotPrefab);
            //writer.WriteGameObject(equipmentSlotUIPrefab);
            //writer.WriteInt32(equipmentSlotsCount);

            //if (SelectedEquipmentSlot)
            //{
            //    initialized = true;
            //}
            return true;
        }
        else
        {
            return false;
        }

    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            selectedEquipmentSlot = reader.ReadEquipmentSlot();
        }
    }
    #endregion

    #region Late Joiner Synchronization
    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        Rpc_UpdateSelectedSlot(conn, SelectedEquipmentSlot);
    }

    [TargetRpc]
    private void Rpc_UpdateSelectedSlot(NetworkConnection target, EquipmentSlot value)
    {
        SelectedEquipmentSlot = value;
    }
    #endregion

    [Server]
    public void Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        // If selected equipment bar is empty, equip item in that bar,
        // else look for an available bar.
        if (selectedEquipmentSlot.EquipmentItem == null)
        {
            // If selected slot can't equip, then equip on first availbable slot.
            if (!selectedEquipmentSlot.Svr_Equip(equipment, equipmentType))
            {
                Svr_SelectSlot(equipmentSlots.IndexOf(Svr_GetAvailableSlot(equipmentType)));
                if (!selectedEquipmentSlot.Svr_Equip(equipment, equipmentType))
                {
                    Svr_SelectSlot(equipmentSlots.IndexOf(Svr_GetFirstSlotOfType(equipmentType)));
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            Svr_SelectSlot(equipmentSlots.IndexOf(Svr_GetAvailableSlot(equipmentType)));
            if (!selectedEquipmentSlot.Svr_Equip(equipment, equipmentType))
            {
                Svr_SelectSlot(equipmentSlots.IndexOf(Svr_GetFirstSlotOfType(equipmentType)));
            }
        }

        selectedEquipmentSlot.Svr_Equip(equipment, equipmentType);
    }

    // Finds and returns the first bar that has no equipment.
    // If none are available, it returns the currently selected bar.
    [Server]
    private EquipmentSlot Svr_GetAvailableSlot(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentItem == null && currentSlot.EquipmentType == equipmentType)
                return currentSlot;
        }
        return SelectedEquipmentSlot;
    }
    [Server]
    private EquipmentSlot Svr_GetFirstSlotOfType(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentType == equipmentType)
                return currentSlot;
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
        if (slotIndex+1 <= equipmentSlotsCount && slotIndex+1 >= 0)
        {
            if (equipmentSlots[slotIndex] != null)
            {
                SelectedEquipmentSlot?.Rpc_Deselect(connectionToClient);
                SelectedEquipmentSlot = equipmentSlots[slotIndex];
                SelectedEquipmentSlot.Rpc_Select(connectionToClient);
            }
        }
    }

    [Command]
    private void Cmd_DropEquipmentItem()
    {
        SelectedEquipmentSlot.Svr_Drop(transform);
    }

    [Command]
    private void Cmd_EquipmentSlotsSetup()
    {
        foreach (EquipmentType type in equipmentSlotsTypes)
        {
            // Setup the networked prefab that holds the item.
            GameObject hotbarSlot = Instantiate(equipmentSlotPrefab);
            NetworkServer.Spawn(hotbarSlot, gameObject);
            hotbarSlot.transform.parent = equipmentSlotsParent;
            EquipmentSlot tempSlot = hotbarSlot.GetComponent<EquipmentSlot>();
            tempSlot.EquipmentType = type;

            // Setup the local UI prefab that shows the item slot.
            Rpc_CreateUISlots(connectionToClient, tempSlot);
            equipmentSlots.Add(tempSlot);
            if (!hasAuthority && isServer)
                Rpc_UpdateEquipmentSlots(connectionToClient, tempSlot, 1);
        }
        equipmentSlotsCount = EquipmentSlots.Count;
        Svr_SelectSlot(0);
    }

    // Setup local player UI hotbar.
    [TargetRpc]
    private void Rpc_CreateUISlots(NetworkConnection conn, EquipmentSlot tempSlot)
    {
        tempSlot.gameObject.transform.parent = equipmentSlotsParent;
        GameObject hotbarSlotUI = Instantiate(equipmentSlotUIPrefab, equipmentSlotsUIParent);
        tempSlot.UISlot = hotbarSlotUI;
    }
}