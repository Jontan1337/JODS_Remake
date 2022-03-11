using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorHit : StatusEffect
{
    private ModifierManager modifierManager;    
    private SurvivorHitSO survivorHitSO;
    int speedIncreases;
    int baseSpeedIncreases;

    public SurvivorHit(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        modifierManager = obj.GetComponent<ModifierManager>();
        survivorHitSO = (SurvivorHitSO) effect;
        baseSpeedIncreases = Mathf.RoundToInt(survivorHitSO.speedModifier * 10);
        speedIncreases = baseSpeedIncreases;

    }
    public override void OnEffectApplied()
    {
        modifierManager.MovementSpeed += survivorHitSO.speedModifier;
        SpeedIncrease();
    }
    public override void ApplyEffect(int? amount)
    {
        Debug.Log(baseSpeedIncreases);
        Debug.Log(speedIncreases);
        speedIncreases = baseSpeedIncreases;
    }

    public override void End()
    {

    }

    public override void Tick()
    {
        base.Tick();
    }

    private async void SpeedIncrease()
    {
        while (speedIncreases > 0)
        {
            await JODSTime.WaitTime(0.01f);
            modifierManager.MovementSpeed -= 0.01f;
            speedIncreases -= 1;
        }
    }

    public override float GetImageAlpha() => speedIncreases;
}
