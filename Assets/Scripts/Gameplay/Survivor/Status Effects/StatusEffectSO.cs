using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public bool isPermanent;

    public float duration;
    public bool canDurationStack;
    public bool canEffectStack;
    public abstract StatusEffect ApplyEffect(GameObject target);


}
