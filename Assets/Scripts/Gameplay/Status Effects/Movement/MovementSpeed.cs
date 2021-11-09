using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeed : StatusEffect
{
    private SurvivorController sClass;
    private MovementSpeedSO movementSpeed;
    private float defaultSpeed;
    public MovementSpeed(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        sClass = obj.GetComponent<SurvivorController>();
        movementSpeed = (MovementSpeedSO) effect;
        defaultSpeed = sClass.walkSpeed;
    }
    public override void OnEffectApplied()
    {
        sClass.walkSpeed += defaultSpeed * movementSpeed.speedModifier;
    }
    public override void ApplyEffect(int? amount)
    {

    }

    public override void End()
    {
        sClass.walkSpeed = defaultSpeed;
    }

    public override void Tick()
    {
        base.Tick();
        Debug.Log("I'm slow for another " + duration + " seconds");
    }
}
