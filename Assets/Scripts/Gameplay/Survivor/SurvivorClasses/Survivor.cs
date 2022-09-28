﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public abstract class Survivor : NetworkBehaviour
{
    public GameObject abilityObject;

    [SerializeField, SyncVar] protected int abilityDamage;

    public abstract void ActiveAbility();

    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => Ability();
    }


    private void Ability()
    {
        if (GetComponentInParent<SurvivorClassStatManager>().AbilityIsReady)
        {
            ActiveAbility();
        }
        else
        {
            ActiveAbilitySecondary();
        }
    }

    public virtual void ActiveAbilitySecondary()
	{
        // Override this to do something when ability is on cooldown e.g. Taekwondo kick. 
	}


}


