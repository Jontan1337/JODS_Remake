using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    protected float duration;
    protected int effectStacks;
    public StatusEffectSO effect { get; }
    protected readonly GameObject obj;
    public bool isFinished;

    public StatusEffect(StatusEffectSO effect, GameObject obj)
    {
        this.effect = effect;
        this.obj = obj;
    }

    public void Tick(float delta)
    {
        duration -= delta;
        if (duration <= 0)
        {
            isFinished = true;
        }
    }

    public void Activate()
    {
        if (effect.canEffectStack || duration <= 0)
        {
            ApplyEffect();
            effectStacks++;
        }

        if (effect.canDurationStack || duration <= 0)
        {
            duration += effect.duration;
        }
    }

    public abstract void ApplyEffect();
    public abstract void End();
}
