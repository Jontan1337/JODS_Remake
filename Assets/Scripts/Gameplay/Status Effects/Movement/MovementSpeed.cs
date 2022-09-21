using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeed : StatusEffect
{
    private ModifierManagerSurvivor modifierManager;    
    private MovementSpeedSO movementSpeed;

    public MovementSpeed(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        modifierManager = obj.GetComponent<ModifierManagerSurvivor>();
        movementSpeed = (MovementSpeedSO) effect;

    }
    public override void Svr_OnEffectApplied()
    {
        modifierManager.data.MovementSpeed += movementSpeed.speedModifier;

    }
    public override void Svr_ApplyEffect(int? amount)
    {

    }

    public override void Svr_End()
    {
        modifierManager.data.MovementSpeed -= movementSpeed.speedModifier;
    }

    public override void Svr_Tick()
    {
        base.Svr_Tick();
    }
}
