using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public abstract class SurvivorClass : NetworkBehaviour
{
    public GameObject abilityObject;

    public abstract void ActiveAbility();
}



