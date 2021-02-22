using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Medkit : NetworkBehaviour, IInteractable, IEquippable
{
    public int uses;
    public bool big;

    [Header("Settings")]
    [SerializeField]
    private string itemName = "Medkit name";
    [SerializeField]
    private EquipmentType equipmentType = EquipmentType.Meds;

    public bool IsInteractable { get; set; }

    public string ObjectName { get; set; }

    public string Name => itemName;

    public GameObject Item => gameObject;

    public EquipmentType EquipmentType => equipmentType;

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        Debug.Log($"{interacter} interacted with {gameObject}");

        // Equipment should be on a child object of the player.
        Equipment equipment = interacter.GetComponentInChildren<Equipment>();

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
