using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    [Header("Timed Settings")]
    public float duration;
    public bool canDurationStack;
    public bool canEffectStack;

    [Header("On Hit Settings")]
    public bool doEffectOnHit;

    public abstract StatusEffect ApplyEffect(GameObject target);
}
