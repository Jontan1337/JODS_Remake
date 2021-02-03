using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infection : StatusEffect
{
    [Range(1, 3)] public int infectionLevel = 0;
    [Range(0, 100)] public float infectionRate = 0;

    public Infection(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {

    }
    public override void ApplyEffect(int? amount)
    {
        if (amount == null) return;

        int newAmount = (int)amount;

        infectionRate = Mathf.Clamp(infectionRate += newAmount, 0, 100);

        Debug.Log("My infection rate is: " + infectionRate);
        Debug.Log("My infection level is: " + infectionLevel);

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
        infectionRate = Mathf.Clamp(infectionRate -= 1, 0, 100);
        Debug.Log("My infection rate is: " + infectionRate);
        Debug.Log("My infection level is: " + infectionLevel);

        if (infectionRate <= 0)
        {
            if (infectionLevel == 0)
            {
                End();
            }
        }
    }

    public void IncreaseInfectionLevel()
    {
        infectionLevel += 1;

        infectionRate = 0;
    }
}
