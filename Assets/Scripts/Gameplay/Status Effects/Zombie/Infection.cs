using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infection : StatusEffect
{
    [Range(0, 3)] public int infectionLevel = 0;
    [Range(0, 100)] public int infectionRate = 0;
    private int maxInfectionLevel = 3;

    public Infection(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {

    }

    public override void Svr_ApplyEffect(int? amount)
    {
        if (amount == null) return;
        if (infectionLevel == maxInfectionLevel) return;

        int newAmount = (int)amount;

        infectionRate = Mathf.Clamp(infectionRate += newAmount, 0, 100);
        
        if (infectionRate == 100)
        {
            IncreaseInfectionLevel();
        }
    }

    public override void Svr_Tick()
    {
        if (infectionLevel == maxInfectionLevel) return;

        infectionRate = Mathf.Clamp(infectionRate -= 1, 0, 100);

        if (infectionRate <= 0)
        {
            if (infectionLevel == 0)
            {
                Svr_End();
            }
        }
    }

    //This is where the debuffs will be applied
    public void IncreaseInfectionLevel()
    {
        infectionLevel += 1;

        infectionRate = 0;

        ModifierManagerSurvivor mm = obj.GetComponent<ModifierManagerSurvivor>();

        switch (infectionLevel)
        {
            case 1:
                mm.MovementSpeed -= 0.3f;
                break;
            case 2:
                mm.Damage -= 0.3f;
                break;
            case 3:
                mm.RangedDamage -= 0.3f;
                break;
        }

        if (infectionLevel == maxInfectionLevel) return;
        currentImageIndex++;
    }

    public override void Svr_OnEffectApplied()
    {
        
    }

    public override float GetImageAlpha() => (infectionLevel / (float)(maxInfectionLevel + 1f)) + (infectionRate * 0.0025f);

    public override void Svr_End()
    {
        ModifierManagerSurvivor mm = obj.GetComponent<ModifierManagerSurvivor>();

        if (infectionLevel > 0)
        {
            mm.MovementSpeed += 0.3f;
        }
        if (infectionLevel > 1)
        {
            mm.Damage += 0.3f;
        }
        if (infectionLevel > 2)
        {
            mm.RangedDamage += 0.3f;
        }

        infectionLevel = 0;
        infectionRate = 0;
    }
}






