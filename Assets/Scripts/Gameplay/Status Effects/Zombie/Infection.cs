using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infection : StatusEffect
{
    [Range(0, 3)] public int infectionLevel = 0;
    [Range(0, 100)] public int infectionRate = 0;

    public Infection(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {

    }

    public override void ApplyEffect(int? amount)
    {
        if (amount == null) return;
        if (infectionLevel == 3) return;

        int newAmount = (int)amount;

        infectionRate = Mathf.Clamp(infectionRate += newAmount, 0, 100);

        if (infectionRate >= 100)
        {
            IncreaseInfectionLevel();
        }
    }

    public override void End()
    {
        infectionLevel = 0;
        infectionRate = 0;
    }

    public override void Tick()
    {
        Debug.Log(duration);
        if (infectionLevel == 3) return;

        infectionRate = Mathf.Clamp(infectionRate -= 1, 0, 100);

        if (infectionRate <= 0)
        {
            if (infectionLevel == 0)
            {
                End();
            }
        }
    }

    //This is where the debuffs will be applied
    public void IncreaseInfectionLevel()
    {
        infectionLevel += 1;

        infectionRate = 0;
    }

    public override void OnEffectApplied()
    {
        
    }

    public override float GetImageAlpha() => (infectionLevel / 4f) + (infectionRate * 0.0025f);
}
