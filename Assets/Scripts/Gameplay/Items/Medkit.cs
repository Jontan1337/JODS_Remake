using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Medkit : NetworkBehaviour, IInteractable, IEquippable, IBindable
{
    public int uses;
    public bool big;

    [Header("Settings")]
    [SerializeField]
    private string itemName = "Medkit name";
    [SerializeField]
    private EquipmentType equipmentType = EquipmentType.Meds;
    [SerializeField]
    private bool isInteractable;

    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public string ObjectName { get; set; }

    public string Name => itemName;

    public GameObject Item => gameObject;

    public EquipmentType EquipmentType => equipmentType;

    public void Bind()
    {
        JODSInput.Controls.Survivor.LMB.performed += OnHeal;
    }

    public void Unbind()
    {
        JODSInput.Controls.Survivor.LMB.performed -= OnHeal;
    }

    private void OnHeal(InputAction.CallbackContext context) => UseMedkit(0);

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        Debug.Log($"{interacter} interacted with {gameObject}");

        // Equipment should be on a child object of the player.
        PlayerEquipment equipment = interacter.GetComponentInChildren<PlayerEquipment>();

        if (equipment != null)
        {
            equipment?.Svr_Equip(gameObject, equipmentType);
            IsInteractable = false;
        }
        else
        {
            Debug.LogWarning($"{interacter} does not have an Equipment component");
        }
    }

    [Server]
    public void Svr_GiveAuthority(NetworkConnection conn)
    {
        netIdentity.AssignClientAuthority(conn);
    }
    [Server]
    public void Svr_RemoveAuthority()
    {
        netIdentity.RemoveClientAuthority();
    }

    public float UseMedkit(float healthPoints)
	{
        if (big)
        {
            if (healthPoints != 100)
            {
                healthPoints = healthPoints >= 31 ? 100 : healthPoints + 69;
                uses--;
            }
            if (uses <= 0)
            {
                gameObject.SetActive(false);
            }
            return healthPoints;
        }
        else
        {
            if (healthPoints != 100)
            {
                healthPoints = healthPoints >= 58 ? 100 : healthPoints + 42;
                uses--;
            }
            if (uses <= 0)
            {
                gameObject.SetActive(false);
            }
            return healthPoints;
        }	
	}

}
