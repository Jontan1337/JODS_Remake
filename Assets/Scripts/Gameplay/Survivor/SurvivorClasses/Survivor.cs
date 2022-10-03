using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public abstract class Survivor : NetworkBehaviour
{
    public GameObject abilityObject;

    public bool optionOne = false;
    public bool optionOneFirstChoice = false;
    public bool optionOneSecondChoice = false;
    public bool optionTwo = false;
    public bool optionTwoFirstChoice = false;
    public bool optionTwoSecondChoice = false;

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



