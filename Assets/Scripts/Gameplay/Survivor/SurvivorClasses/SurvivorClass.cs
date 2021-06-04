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


    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteBoolean(abilityActivatedSuccesfully);
            writer.WriteBoolean(abilityIsToggled);
            return true;
        }
        else
        {
            writer.WriteBoolean(abilityActivatedSuccesfully);
            writer.WriteBoolean(abilityIsToggled);
            writer.WriteGameObject(abilityObject);
            return true;
        }
    }
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            abilityActivatedSuccesfully = reader.ReadBoolean();
            abilityIsToggled = reader.ReadBoolean();
        }
        else
        {
            abilityActivatedSuccesfully = reader.ReadBoolean();
            abilityIsToggled = reader.ReadBoolean();
            abilityObject = reader.ReadGameObject();
        }
    }

    public abstract void ActiveAbility();
}



