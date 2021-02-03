using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effects
{
    public float durationLeft;
    public string effectName;

    public Effects(float duration, string v)
    {
        durationLeft = duration;
        effectName = v;
    }
}

public class StatusEffectManager : MonoBehaviour
{
    private Dictionary<StatusEffectSO, StatusEffect> currentEffects = new Dictionary<StatusEffectSO, StatusEffect>();
    public List<Effects> listlol = new List<Effects>();
    public void ApplyStatusEffect(StatusEffect newEffect)
    {
        Debug.Log("New Status Effect : " + newEffect.ToString());
        if (currentEffects.ContainsKey(newEffect.effect))
        {
            currentEffects[newEffect.effect].Activate();
        }
        else
        {
            currentEffects.Add(newEffect.effect, newEffect);
            newEffect.Activate();
            listlol.Add(new Effects(newEffect.effect.duration, newEffect.effect.ToString()));
        }
    }
    public void RemoveStatusEffect()
    {

    }
}
