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
    public override void OnEffectApplied()
    {
        modifierManager.MovementSpeed += movementSpeed.speedModifier;

    }
    public override void ApplyEffect(int? amount)
    {

    }

    public override void End()
    {
        modifierManager.MovementSpeed -= movementSpeed.speedModifier;
    }

    public override void Tick()
    {
        base.Tick();
    }
}
