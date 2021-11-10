﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RootMotion.FinalIK;
using System;

public class SurvivorAnimationManager : NetworkBehaviour
{
	public Animator anim;
	public FullBodyBipedIK fullBodyIK;

	public HandPoser rightHandPoser;
	public HandPoser leftHandPoser;

	private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    // Start is called before the first frame update
    void Start()
	{
		if (!hasAuthority) return;
		anim = GetComponent<Animator>();
		GetComponent<SurvivorSetup>().onSpawnItem += GetReferences;
	}

    private void GetReferences(GameObject item)
    {
		if (item.TryGetComponent(out ItemName itemName))
		{
            if (itemName.itemName == ItemNames.Equipment)
            {
				if (item.TryGetComponent(out PlayerEquipment playerEquipment))
                {
					playerEquipment.onServerEquippedItemChange += OnEquippedItemChange;
                }
            }
		}
	}

    private void OnEquippedItemChange(GameObject oldItem, GameObject newItem)
    {
        if (newItem)
        {
			if (newItem.TryGetComponent(out HandIKEffectors handIKEffectors))
			{
				SetIKRightHandEffector(handIKEffectors.rightHandEffector);
				SetIKLeftHandEffector(handIKEffectors.leftHandEffector);
				return;
			}
        }
		SetIKRightHandEffector(null);
		SetIKLeftHandEffector(null);
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

	public void SetIKRightHandEffector(Transform effector)
    {
		if (effector != null)
        {
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
	public void SetIKLeftHandEffector(Transform effector)
    {

		if (effector != null)
		{
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
