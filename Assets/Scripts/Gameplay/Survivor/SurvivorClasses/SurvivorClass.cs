using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public abstract class SurvivorClass : NetworkBehaviour
{
    public Action OnAbilityActivated;
    public bool abilityActivatedSuccesfully = false;
    public bool abilityIsToggled = false;

    public GameObject abilityObject;
    public virtual void ActiveAbility()
    {
        // Override in class script.
    }
}



