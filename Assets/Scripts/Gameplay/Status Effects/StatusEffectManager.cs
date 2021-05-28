using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private Dictionary<StatusEffectSO, StatusEffect> currentEffects = new Dictionary<StatusEffectSO, StatusEffect>();
    public List<string> statusEffects = new List<string>();

    private Coroutine effectEnumerator;
    bool isActive;

    private void Start()
    {
        isActive = false;
    }

    public void RemoveStatusEffect(StatusEffectSO effect)
    {
        if (currentEffects.ContainsKey(effect))
        {
            Debug.Log(effect.name + " removed");
            currentEffects.Remove(effect);
        }
        if (statusEffects.Contains(effect.name))
        {
            statusEffects.Remove(effect.name);
        }
    }

    private IEnumerator StatusEffectEnumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            //Go through each of the current active effects, and call their Tick method
            foreach (var effect in currentEffects.Values.ToList())
            {
                //Call Tick, which does an effect over time and reduces the duration of the effect
                effect.Tick();

                //If the duration of this effect has reached 0, then stop the effect and remove it.
                if (effect.isFinished)
                {
                    //Remove the effect from the list of current active effects
                    currentEffects.Remove(effect.effect);
                    statusEffects.Remove(effect.effect.name);
                }
            }
            if (currentEffects.Count == 0)
            {
                isActive = false;
                StopCoroutine(effectEnumerator);
            }
        }
    }


    public void ApplyStatusEffect(StatusEffect newEffect, int? amount = null)
    {

        //If the status effect is already in the list, then activate the effect
        if (currentEffects.ContainsKey(newEffect.effect))
        {
            //If this effect can stack in any way, it will stack when activated again.
            currentEffects[newEffect.effect].Activate(amount);
        }
        else
        {
            Debug.Log("New Status Effect : " + newEffect.effect.name);
            statusEffects.Add(newEffect.effect.name);

            //Add the effect to the dictionary
            currentEffects.Add(newEffect.effect, newEffect);

            //Activate the effect
            newEffect.Activate(amount);

            //If the coroutine is not currently running, then activate it
            //The coroutine will stop when there are no more active effects
            if (!isActive)
            {
                effectEnumerator = StartCoroutine(StatusEffectEnumerator());
                isActive = true;
            }
        }
    }
}
