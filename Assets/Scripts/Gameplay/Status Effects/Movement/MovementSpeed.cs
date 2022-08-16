using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeed : StatusEffect
{
    private ModifierManager modifierManager;    
    private MovementSpeedSO movementSpeed;

    public MovementSpeed(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        modifierManager = obj.GetComponent<ModifierManager>();
        movementSpeed = (MovementSpeedSO) effect;

    }
    public override void Svr_OnEffectApplied()
    {
        modifierManager.MovementSpeed += movementSpeed.speedModifier;

    }
    public override void Svr_ApplyEffect(int? amount)
    {

    }

    public override void Svr_End()
    {
        modifierManager.MovementSpeed -= movementSpeed.speedModifier;
    }

    public override void Svr_Tick()
    {
        base.Svr_Tick();
    }
}
