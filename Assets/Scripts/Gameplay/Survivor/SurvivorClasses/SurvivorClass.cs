using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public abstract class SurvivorClass : NetworkBehaviour
{
    public GameObject abilityObject;

    public abstract void ActiveAbility();
    public virtual void ActiveAbilitySecondary()
	{
        // Override this to do something when ability is on cooldown e.g. Taekwondo kick. 
	}
}



