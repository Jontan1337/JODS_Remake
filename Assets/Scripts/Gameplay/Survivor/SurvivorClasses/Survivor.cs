using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public abstract class Survivor : NetworkBehaviour
{
    public GameObject abilityObject;

    public bool optionA = false;
    public bool optionA1 = false;
    public bool optionA2 = false;
    public bool optionB = false;
    public bool optionB1 = false;
    public bool optionB2 = false;

    [SerializeField, SyncVar] public int abilityDamage;

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



