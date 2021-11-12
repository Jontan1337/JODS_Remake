using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RootMotion.FinalIK;
using System;

public class SurvivorAnimationManager : NetworkBehaviour
{
	public Animator anim;
	public FullBodyBipedIK fullBodyIK;

	public Transform rightHandEffector;
	public Transform leftHandEffector;

	public HandPoser rightHandPoser;
	public HandPoser leftHandPoser;

	private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private void Awake()
	{
		if (hasAuthority)
        {
			anim = GetComponent<Animator>();
        }
        if (isServer)
        {
        }
		GetComponent<SurvivorSetup>().onServerSpawnItem += GetReferences;
	}

    private void GetReferences(GameObject item)
    {
		if (item.TryGetComponent(out ItemName itemName))
		{
            if (itemName.itemName == ItemNames.Equipment)
            {
				if (item.TryGetComponent(out PlayerEquipment playerEquipment))
                {
					playerEquipment.onServerEquippedItemChange += OnServerEquippedItemChange;
                }
            }
		}
	}

    private void OnServerEquippedItemChange(GameObject oldItem, GameObject newItem)
    {
        if (newItem)
        {
			if (newItem.TryGetComponent(out HandIKEffectors handIKEffectors))
			{
                Rpc_SetIKRightHandEffector(handIKEffectors);
                Rpc_SetIKLeftHandEffector(handIKEffectors);
				return;
			}
		}
        Rpc_SetIKRightHandEffector(null);
        Rpc_SetIKLeftHandEffector(null);
    }

    public void HasWeaponAnimation(bool enable)
	{
		anim.SetBool("HasWeapon", enable);

		//Do some IK stuff here to get the hands to fit onto the weapon.
		Debug.LogWarning("IK stuff here");
	}

	public void SetFloat(string param, float value)
	{
		anim.SetFloat(param, value);
	}
	public void SetBool(string param, bool value)
	{
		anim.SetBool(param, value);
	}

	[ClientRpc]
	public void Rpc_SetIKRightHandEffector(HandIKEffectors handIKEffectors)
    {
		print("RPC IK stuff");
		if (handIKEffectors != null)
        {
			Transform effector = handIKEffectors.rightHandEffector;
			fullBodyIK.solver.rightHandEffector.target = effector;
			fullBodyIK.solver.rightHandEffector.positionWeight = 1f;
			fullBodyIK.solver.rightHandEffector.rotationWeight = 1f;
			rightHandPoser.poseRoot = effector;
			rightHandPoser.localPositionWeight = 1f;
			rightHandPoser.localRotationWeight = 1f;
		}
		else
        {
			fullBodyIK.solver.rightHandEffector.target = null;
			fullBodyIK.solver.rightHandEffector.positionWeight = 0f;
			fullBodyIK.solver.rightHandEffector.rotationWeight = 0f;
			rightHandPoser.poseRoot = null;
			rightHandPoser.localPositionWeight = 0f;
			rightHandPoser.localRotationWeight = 0f;
        }
    }
	[ClientRpc]
	public void Rpc_SetIKLeftHandEffector(HandIKEffectors handIKEffectors)
	{
		if (handIKEffectors != null)
		{
			Transform effector = handIKEffectors.leftHandEffector;
			fullBodyIK.solver.leftHandEffector.target = effector;
			fullBodyIK.solver.leftHandEffector.positionWeight = 1f;
			fullBodyIK.solver.leftHandEffector.rotationWeight = 1f;
			leftHandPoser.poseRoot = effector;
			leftHandPoser.localPositionWeight = 1f;
			leftHandPoser.localRotationWeight = 1f;
		}
		else
		{
			fullBodyIK.solver.leftHandEffector.target = null;
            fullBodyIK.solver.leftHandEffector.positionWeight = 0f;
			fullBodyIK.solver.leftHandEffector.rotationWeight = 0f;
			leftHandPoser.poseRoot = null;
			leftHandPoser.localPositionWeight = 0f;
			leftHandPoser.localRotationWeight = 0f;
		}
    }
}
