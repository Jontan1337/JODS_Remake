﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    public bool activeUntilRemoved;
    public float duration;
    protected int effectStacks;
    public StatusEffectSO effect { get; }
    protected readonly GameObject obj;
    public bool isFinished;
    private bool isApplied = false;

    public StatusEffect(StatusEffectSO effect, GameObject obj)
    {
        this.activeUntilRemoved = effect.activeUntilRemoved;
        this.effect = effect;
        this.obj = obj;
    }

    //This function is called every second
    public virtual void Tick()
    {
        if (!activeUntilRemoved)
        {
            //Decrease the duration by 1 (every second)
            duration -= 1;

            //If duration has reached 0
            if (duration <= 0)
            {
                isFinished = true;

                End(); //Do something when the effect ends, like reset movement speed
            }
        }
    }

    //This function is called when the status effect is applied 
    public virtual void Activate(int? amount)
    {
        if (!isApplied)
        {
            OnEffectApplied();
            isApplied = true;
        }
        if (effect.doEffectOnHit)
        {
            ApplyEffect(amount);
        }
        else if (effect.canEffectStack || duration <= 0)
        {
            //If the effect can stack or if duration is 0, stack the effect.
            ApplyEffect(amount);
            effectStacks++;
        }

        if (effect.canDurationStack || duration <= 0)
        {
            //If the effect duration can stack or if duration is 0, extend the duration.
            duration += effect.duration;
        }
    }

    public abstract void OnEffectApplied();
    public abstract void ApplyEffect(int? amount);//This will apply the actual effect.
    public abstract void End(); //This function is called when the effect ends, either by duration or stopped by some other means.
}
