using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public abstract class SurvivorClass : NetworkBehaviour
{
    public bool abilityActivatedSuccesfully = false;
    public bool abilityIsToggled = false;

    public GameObject abilityObject;

    public abstract void ActiveAbility();
}



