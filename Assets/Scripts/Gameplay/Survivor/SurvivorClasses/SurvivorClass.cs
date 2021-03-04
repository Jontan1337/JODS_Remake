using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class SurvivorClass : NetworkBehaviour
{
    public bool abilityActivatedSuccesfully = false;
    public GameObject abilityObject;
    public virtual void ActiveAbility()
    {

    }
}



