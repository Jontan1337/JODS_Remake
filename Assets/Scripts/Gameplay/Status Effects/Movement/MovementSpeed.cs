using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeed : StatusEffect
{
    private SurvivorController sClass;
    private MovementSpeedSO movementSpeed;
    private float defaultWalkSpeed;
    private float defaultSprintSpeed;
    public MovementSpeed(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        sClass = obj.GetComponent<SurvivorController>();
        movementSpeed = (MovementSpeedSO) effect;
        defaultWalkSpeed = sClass.walkSpeedMultiplier;
        defaultSprintSpeed = sClass.sprintSpeedMultiplier;
    }
    public override void OnEffectApplied()
    {
        Debug.Log("Movement Speed Status effects do not work on clients because the server is handling all of the status effects.");
        Debug.Log("The actual effects are therefor also run on the server. Fix.");
        sClass.walkSpeedMultiplier += defaultWalkSpeed * movementSpeed.speedModifier;
        sClass.sprintSpeedMultiplier += defaultSprintSpeed * movementSpeed.speedModifier;
    }
    public override void ApplyEffect(int? amount)
    {

    }

    public override void End()
    {
        sClass.walkSpeedMultiplier = defaultWalkSpeed;
        sClass.sprintSpeedMultiplier = defaultSprintSpeed;
    }

    public override void Tick()
    {
        base.Tick();
    }
}
