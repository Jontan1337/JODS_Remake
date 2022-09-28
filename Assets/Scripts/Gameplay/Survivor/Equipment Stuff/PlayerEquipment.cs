﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;

public class PlayerEquipment : NetworkBehaviour, IInitializable<SurvivorSetup>
{
    [Tooltip("A list of the equipment types, the player should have.")]
    public List<EquipmentType> equipmentSlotsTypes = new List<EquipmentType>();
    [Space]
    public Transform playerHands;

    [SerializeField, SyncVar]
    private GameObject itemInHands;
    [SerializeField, SyncVar]
    private EquipmentItem equipmentItem;
    [SerializeField, SyncVar]
    private EquipmentSlot selectedEquipmentSlot;
    // Maybe change to SyncList?
    [SerializeField]
    private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    [Header("Player Preferences")]
    [SerializeField]
    private ItemPickupBehaviour itemPickupBehaviour = ItemPickupBehaviour.EquipAny;

    [Header("Equipment slot setup settings")]
    [SerializeField, Tooltip("The parent transform, where the equipment slots should be instantiated.")]
    private Transform equipmentSlotsParent;
    [SerializeField, Tooltip("The parent transform, where the UI equipment slots should be instantiated.")]
    private Transform weaponEquipmentSlotsUIParent;
    private Transform extraEquipmentSlotsUIParent;
    [SerializeField, Tooltip("The prefab of the equipment slots.")]
    private GameObject equipmentSlotPrefab = null;
    [SerializeField, Tooltip("The prefab of the UI equipment slots.")]
    private GameObject equipmentSlotUIPrefab = null;

    private int equipmentSlotsCount = 0;
    private Coroutine COMoveToHands;
    private SurvivorSetup playerSetup;
    private SurvivorStatManager characterStatManager;

    private const string inGameUIPath = "UI/Canvas - In Game";

    #region ServerOnly Fields

    private Action<GameObject> onServerItemPickedUp;
    public Action<GameObject, GameObject> onServerEquippedItemChange;

    #endregion

    #region ClientOnly Fields

    private Action<GameObject> onClientItemPickedUp; // Not used remove perhaps?
    public Action<GameObject, GameObject> onClientEquippedItemChange;

    #endregion

    public GameObject ItemInHands
    {
        get => itemInHands;
        private set
        {
            if (!isServer) return;

            GameObject oldEquippedItem = null;
            if (itemInHands)
            {
                oldEquippedItem = itemInHands;
            }
            itemInHands = value;
            if (itemInHands)
            {
                EquipmentItem = itemInHands.GetComponent<EquipmentItem>();
            }
            else
            {
                EquipmentItem = null;
            }
            // Tell EquipmentControl to unbind old item and bind new item.
            Svr_InvokeEquippedItemChange(oldEquippedItem, itemInHands);
        }
    }
    public EquipmentItem EquipmentItem
    {
        get => equipmentItem;
        set
        {
            if (!isServer) return;

            EquipmentItem oldEquipmentItem = equipmentItem;
            if (oldEquipmentItem)
            {
                oldEquipmentItem.onServerDropItem -= Svr_OnItemDropped;
            }
            equipmentItem = value;
            if (equipmentItem)
            {
                equipmentItem.onServerDropItem += Svr_OnItemDropped;
            }
        }
    }

    public EquipmentSlot SelectedEquipmentSlot
    {
        get => selectedEquipmentSlot;
        private set
        {
            if (!isServer) return;
            if (selectedEquipmentSlot == value) return;
            if (selectedEquipmentSlot)
            {
                // Stop listening if the old selected slot's item changes.
                selectedEquipmentSlot.onServerItemChange -= Svr_SelectedSlotItemChange;
                if (selectedEquipmentSlot.EquipmentItem)
                {
                    selectedEquipmentSlot.EquipmentItem.GetComponent<EquipmentItem>().Svr_Unequip();
                }
            }
            selectedEquipmentSlot = value;
            if (selectedEquipmentSlot)
            {
                // Listen if the new selected slot's item changes.
                selectedEquipmentSlot.onServerItemChange += Svr_SelectedSlotItemChange;
                if (selectedEquipmentSlot.EquipmentItem)
                {
                    selectedEquipmentSlot.EquipmentItem.GetComponent<EquipmentItem>().Svr_Equip();
                }
                ItemInHands = selectedEquipmentSlot.EquipmentItem;
            }
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
    public bool IsInitialized { get; private set; }

    public void Init(SurvivorSetup initializer)
    {
        if (IsInitialized) return;

        playerSetup = initializer;

        equipmentSlotsTypes = playerSetup.equipmentSlotsTypes;
        if (isServer)
        {
            onServerItemPickedUp += Svr_OnItemPickedUp;
            characterStatManager = GetComponentInParent<SurvivorStatManager>();
            characterStatManager.onDownChanged.AddListener(delegate (bool isDown) { Svr_OnDownChanged(isDown); } );
        }
        if (hasAuthority)
        {
            Cmd_EquipmentSlotsSetup();
            // This is run in Start instead of OnStartServer because
            // OnStartServer gets called before Start when equipment object
            // is not set as child to player yet.
            initializer.onClientSpawnItem += GetReferences;
            GameSettings.onItemPickupBehaviourChanged += Cmd_OnItemPickupBehaviourChanged;
        }

        IsInitialized = true;
    }

    private void OnTransformParentChanged()
    {
        // When parent changed, check if it's correctly set to player Survivor
        // and then find UI hotbar under Survivors canvas.

        if (transform.parent.name.Contains("Survivor"))
        {
            weaponEquipmentSlotsUIParent = transform.parent.Find($"{inGameUIPath}/Equipment Hotbar/Equipment Hotbar Weapons");
            extraEquipmentSlotsUIParent = transform.parent.Find($"{inGameUIPath}/Equipment Hotbar/Equipment Hotbar Extra");
        }
    }

    [Command]
    private void Cmd_OnItemPickupBehaviourChanged(ItemPickupBehaviour pickupBehaviour)
    {
        itemPickupBehaviour = pickupBehaviour;
    }

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

    #region NetworkBehaviour Callbacks
    public override void OnStartServer()
    {
        //if (NetworkTest.Instance != null)
        //{
        //    NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
        //}
        //else
        //{
        //    Lobby.RelayOnServerSynchronize += Svr_UpdateVars;
        //}
    }

    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Hotbarselecting.performed += number => Cmd_SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()) - 1);

    }
    public override void OnStartClient()
    {
        equipmentSlotsParent = transform;
    }

    public override void OnStopServer()
    {
        if (NetworkTest.Instance != null)
        {
            NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
        }
        else
        {
            Lobby.RelayOnServerSynchronize -= Svr_UpdateVars;
        }
    }
    public override void OnStopAuthority()
    {
        transform.root.GetComponent<SurvivorSetup>().onClientSpawnItem -= GetReferences;
        JODSInput.Controls.Survivor.Hotbarselecting.performed -= number => Cmd_SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()) - 1);
    }
    #endregion

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteGameObject(itemInHands);
            writer.WriteEquipmentItem(equipmentItem);
            writer.WriteEquipmentSlot(selectedEquipmentSlot);
            return true;
        }
        else
        {
            writer.WriteList(equipmentSlotsTypes);
            writer.WriteTransform(playerHands);
            writer.WriteGameObject(itemInHands);
            return true;
        }
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            itemInHands = reader.ReadGameObject() ?? null;
            equipmentItem = reader.ReadEquipmentItem() ?? null;
            selectedEquipmentSlot = reader.ReadEquipmentSlot() ?? null;
        }
        else
        {
            equipmentSlotsTypes = reader.ReadList<EquipmentType>();
            playerHands = reader.ReadTransform();
            itemInHands = reader.ReadGameObject();
        }
    }
    #endregion

    #region Late Joiner Synchronization
    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        if (SelectedEquipmentSlot)
            Rpc_UpdateSelectedSlot(conn, SelectedEquipmentSlot);

        if (ItemInHands)
            Rpc_UpdateEquippedItem(conn, ItemInHands);

        if (playerHands)
            Rpc_UpdatePlayerHands(conn, playerHands);
    }

    [TargetRpc]
    private void Rpc_UpdateSelectedSlot(NetworkConnection target, EquipmentSlot value)
    {
        selectedEquipmentSlot = value;
    }
    [TargetRpc]
    private void Rpc_UpdateEquippedItem(NetworkConnection target, GameObject value)
    {
        itemInHands = value;
    }
    [TargetRpc]
    private void Rpc_UpdatePlayerHands(NetworkConnection target, Transform value)
    {
        playerHands = value;
    }
    #endregion

    [Server]
    private void Svr_OnDownChanged(bool isDown)
    {
        if (isDown)
        {
            EquipmentItem?.Svr_Unequip();
        }
        else
        {
            EquipmentItem?.Svr_Equip();
        }
    }

    [Server]
    public void Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (!SelectedEquipmentSlot)
        {
            Svr_SelectEquipmentSlot(0);
        }
        EquipmentItem newEquipmentItem = equipment.GetComponent<EquipmentItem>();
        // If selected equipment hotbar slot is empty, equip item in that hotbar slot,
        // else look for an available hotbar slot.
        if (SelectedEquipmentSlot.EquipmentType != equipmentType || SelectedEquipmentSlot.EquipmentItem != null)
        {
            EquipmentSlot newSelectedSlot = Svr_GetSlot(equipmentType);
            if (newSelectedSlot != null)
            {
                // If newSelectedSlot is not null, then select it.
                // otherwise use the one that's already selected.
                switch (itemPickupBehaviour)
                {
                    case ItemPickupBehaviour.EquipAny:
                        Svr_SelectEquipmentSlot(equipmentSlots.IndexOf(newSelectedSlot));
                        break;
                    case ItemPickupBehaviour.EquipWeapon:
                        if (equipmentType == EquipmentType.Weapon)
                        {
                            Svr_SelectEquipmentSlot(equipmentSlots.IndexOf(newSelectedSlot));
                        }
                        break;
                    case ItemPickupBehaviour.EquipNone:
                        Svr_ReplaceSlotItem(newSelectedSlot, equipment);
                        MoveToHands(equipment);
                        return;
                    default:
                        break;
                }
            }
        }

        if (equipmentType == SelectedEquipmentSlot.EquipmentType)
        {
            if (SelectedEquipmentSlot.EquipmentItem)
            {
                EquipmentItem.Svr_Unequip();
                Svr_RemoveItem(SelectedEquipmentSlot.EquipmentItem);
            }
            Svr_PickupAndAssignItem(equipment);
            newEquipmentItem.Svr_Equip();
        }

        // EquipmentType none is meant for equipment
        // that doesn't fit any of the slots.
        if (equipmentType == EquipmentType.None)
        {
            if (equipment.TryGetComponent(out IEquippable equippable))
            {
                // Unequip current item and put the new item in hands.
                // Unbind current weapon
                // Unequip current weapon
                // Deselect item slot
                newEquipmentItem.Svr_Pickup(playerHands, connectionToClient);
                Svr_EquipNewItem(newEquipmentItem);
                ItemInHands = equipment;
                Svr_DeselectEquipmentSlot();
            }
        }
        Svr_InvokeItemPickedUp(equipment);
        //Rpc_InvokeItemPickedUp(EquippedItem);
    }

    [Server]
    public void Svr_EquipNewItem(EquipmentItem equipmentItem)
    {
        if (EquipmentItem)
        {
            EquipmentItem.Svr_Unequip();
        }
        if (equipmentItem)
        {
            equipmentItem.Svr_Equip();
        }
    }

    [Server]
    private EquipmentSlot Svr_GetSlot(EquipmentType equipmentType)
    {
        // Try to get empty slot of same type.
        EquipmentSlot tempAvailableSlot = Svr_GetAvailableSlot(equipmentType);
        if (tempAvailableSlot != null)
        {
            return tempAvailableSlot;
        }
        // If no empty slot of same type was found.
        // Try to get first slot of same type.
        if (selectedEquipmentSlot.EquipmentType != equipmentType)
        {
            EquipmentSlot tempSlotOfType = Svr_GetFirstSlotOfType(equipmentType);
            if (tempSlotOfType != null)
            {
                return tempSlotOfType;
            }
        }
        return null;
    }

    // Finds and returns the first bar that has no equipment.
    // If none are available, it returns the currently selected bar.
    // Returns null if no matching and empty slot is available.
    [Server]
    private EquipmentSlot Svr_GetAvailableSlot(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentItem == null && currentSlot.EquipmentType == equipmentType)
                return currentSlot;
        }
        return null;
    }
    // Returns the first slot of the same type even if that slot already has an item.
    // Returns null if no matching slot type was found.
    [Server]
    private EquipmentSlot Svr_GetFirstSlotOfType(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentType == equipmentType)
                return currentSlot;
        }
        return null;
    }

    [Server]
    private void Svr_InvokeEquippedItemChange(GameObject oldItem, GameObject newItem)
    {
        onServerEquippedItemChange?.Invoke(oldItem, newItem);
        Rpc_InvokeEquippedItemChange(connectionToClient, oldItem, newItem);
    }
    //[ClientRpc]
    //private void Rpc_AnimateEquipmentChange()
    //{
    //    // Refactor and move this code somewhere else.
    //    playerHands.DOLocalMove(new Vector3(0f, -0.3f, 0f), 0.5f).SetEase(Ease.Linear);
    //    playerHands.DOLocalRotate(new Vector3(45f, 0f, -10f), 0.5f).SetEase(Ease.Linear)
    //        .OnComplete(OnComplete);

    //    void OnComplete()
    //    {
    //        playerHands.DOLocalMove(new Vector3(0f, 0f, 0f), 0.5f);
    //        playerHands.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.5f);
    //    }
    //}
    [TargetRpc]
    private void Rpc_InvokeEquippedItemChange(NetworkConnection target, GameObject oldItem, GameObject newItem)
    {
        onClientEquippedItemChange?.Invoke(oldItem, newItem);
    }

    [Server]
    private void Svr_InvokeItemPickedUp(GameObject newItem)
    {
        onServerItemPickedUp?.Invoke(newItem);
    }
    [Server]
    private void Svr_OnItemPickedUp(GameObject newItem)
    {
        OnItemPickedUp(newItem);
        Rpc_OnItemPickedUp(newItem);
    }
    [ClientRpc]
    private void Rpc_OnItemPickedUp(GameObject newItem)
    {
        if (isServer) return;
        OnItemPickedUp(newItem);
    }
    private void OnItemPickedUp(GameObject newItem)
    {
        COMoveToHands = StartCoroutine(SmoothMoveToHands(newItem));
    }

    [Server]
    private void Svr_OnItemDropped(GameObject item)
    {
        OnItemDropped(item);
        Rpc_OnItemDropped(item);
        if (SelectedEquipmentSlot)
        {
            SelectedEquipmentSlot.Svr_UnassignItem();
        }
        else
        {
            Svr_SelectEquipmentSlot(0);
        }
    }
    [ClientRpc]
    private void Rpc_OnItemDropped(GameObject item)
    {
        if (isServer) return;
        OnItemDropped(item);
    }
    private void OnItemDropped(GameObject item)
    {
        if (COMoveToHands != null)
        {
            StopCoroutine(COMoveToHands);
            COMoveToHands = null;
        }
    }

    [Server]
    private void Svr_ReplaceSlotItem(EquipmentSlot equipmentSlot, GameObject newItem)
    {
        // Check if selected equipmentslot already has an item.
        if (equipmentSlot.EquipmentItem)
        {
            equipmentSlot.EquipmentItem.TryGetComponent(out EquipmentItem equipmentSlotsEquipmentItem);
            // Drop the previous item.
            equipmentSlotsEquipmentItem.Svr_Drop();
            // Remove the item in the equipmentslot.
            equipmentSlot.Svr_UnassignItem();
        }
        // Equip the new item in the equipment slot.
        equipmentSlot.Svr_AssignItem(newItem);
        newItem.TryGetComponent(out EquipmentItem newEquipmentItem);
        // Assign the new item to this player and parent to playerhands.
        newEquipmentItem?.Svr_Pickup(playerHands, connectionToClient);
        // Hide the new item since we're not using it, only equipping it.
        newEquipmentItem?.Svr_ShowItem(false);
    }

    [Server]
    private void Svr_SelectedSlotItemChange(GameObject oldItem, GameObject newItem)
    {
        ItemInHands = newItem;
        Rpc_SelectedSlotItemChange(oldItem, newItem);
    }

    [ClientRpc]
    private void Rpc_SelectedSlotItemChange(GameObject oldItem, GameObject newItem)
    {

    }

    [Command]
    public void Cmd_SelectSlot(int slotIndex)
    {
        Svr_SelectEquipmentSlot(slotIndex);
    }
    [Server]
    public void Svr_SelectEquipmentSlot(int slotIndex)
    {
        if (slotIndex + 1 <= equipmentSlotsCount && slotIndex + 1 >= 0)
        {
            if (equipmentSlots[slotIndex] != null)
            {
                SelectedEquipmentSlot?.Rpc_Deselect(connectionToClient);
                SelectedEquipmentSlot = equipmentSlots[slotIndex];
                SelectedEquipmentSlot.Rpc_Select(connectionToClient);
            }
        }
    }
    [Server]
    public void Svr_DeselectEquipmentSlot()
    {
        SelectedEquipmentSlot?.Rpc_Deselect(connectionToClient);
        SelectedEquipmentSlot = null;
    }

    private IEnumerator SmoothMoveToHands(GameObject newItem)
    {
        while (!Equals(newItem.transform.position, playerHands.position)
                && !Equals(newItem.transform.rotation, playerHands.rotation))
        {
            newItem.transform.position = Vector3.Lerp(newItem.transform.position, playerHands.position, Time.deltaTime * 20);
            newItem.transform.rotation = Quaternion.Lerp(newItem.transform.rotation, playerHands.rotation, Time.deltaTime * 20);

            yield return null;

            if (!newItem)
            {
                yield break;
            }

            if (Vector3.Distance(newItem.transform.position, playerHands.position) < 0.08f
                && Vector3.Distance(newItem.transform.eulerAngles, playerHands.eulerAngles) < 0.08f)
            {
                newItem.transform.position = playerHands.position;
                newItem.transform.rotation = playerHands.rotation;
                yield break;
            }
        }
    }
    private void MoveToHands(GameObject newItem)
    {
        newItem.transform.position = playerHands.position;
        newItem.transform.rotation = playerHands.rotation;
    }

    [Server]
    private void Svr_PickupAndAssignItem(GameObject item)
    {
        if (item.TryGetComponent(out EquipmentItem equipmentItem))
        {
            equipmentItem.Svr_Pickup(playerHands, connectionToClient);
        }
        SelectedEquipmentSlot.Svr_AssignItem(item);
    }

    [Command]
    private void Cmd_RemoveItem(GameObject item)
    {
        Svr_RemoveItem(item);
    }
    [Server]
    private void Svr_RemoveItem(GameObject item)
    {
        if (item)
        {
            EquipmentItem.Svr_Drop();
        }
    }

    [Server]
    public void Svr_DropAllItems()
    {
        foreach (EquipmentSlot slot in equipmentSlots)
        {
            if (slot.EquipmentItem == null) continue;
            // Doesn't always drop currently equipped weapon... Need fix.
            slot.EquipmentItem.GetComponent<EquipmentItem>().Svr_Drop();
            //Svr_OnItemDropped(slot.EquipmentItem);
            Svr_ClearHotbarSlot(slot);
        }
    }

    [Server]
    private void Svr_ClearHotbarSlot(EquipmentSlot equipmentSlot)
    {
        equipmentSlot.Svr_UnassignItem();
    }

    [Command]
    private void Cmd_EquipmentSlotsSetup()
    {
        Svr_EquipmentSlotsSetup();
    }

    [Server]
    private void Svr_EquipmentSlotsSetup()
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
            Rpc_CreateUISlots(connectionToClient, tempSlot, (int)tempSlot.EquipmentType);
            equipmentSlots.Add(tempSlot);
            if (!hasAuthority && isServer)
                Rpc_UpdateEquipmentSlots(connectionToClient, tempSlot, 1);
        }
        equipmentSlotsCount = EquipmentSlots.Count;
        Svr_SelectEquipmentSlot(0);
    }

    // Setup local player UI hotbar.
    [TargetRpc]
    private void Rpc_CreateUISlots(NetworkConnection conn, EquipmentSlot tempSlot, int equipmentType)
    {
        tempSlot.gameObject.transform.parent = equipmentSlotsParent;
        GameObject hotbarSlotUI = Instantiate(equipmentSlotUIPrefab);
        switch ((EquipmentType)equipmentType)
        {
            case EquipmentType.Weapon:
                hotbarSlotUI.transform.SetParent(weaponEquipmentSlotsUIParent, false);
                break;
            case EquipmentType.Special:
            case EquipmentType.Meds:
                hotbarSlotUI.transform.SetParent(extraEquipmentSlotsUIParent, false);
                break;
        }
        tempSlot.UISlot = hotbarSlotUI;
    }

    private void GetReferences(GameObject item)
    {
        if (item.TryGetComponent(out ItemName itemName))
        {
            switch (itemName.itemName)
            {
                case ItemNames.ItemContainer:
                    playerHands = item.transform;
                    Cmd_SetPlayerHands(playerHands);
                    break;
                default:
                    break;
            }
        }
    }

    [Command]
    private void Cmd_SetPlayerHands(Transform playerHands)
    {
        this.playerHands = playerHands;
        Rpc_SetPlayerHands(playerHands);
    }
    [ClientRpc]
    private void Rpc_SetPlayerHands(Transform playerHands)
    {
        this.playerHands = playerHands;
    }
}