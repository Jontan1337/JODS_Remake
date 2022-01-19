using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    [Header("Timed Settings")]
    public bool activeUntilRemoved;
    public float duration;
    public bool canDurationStack;
    public bool canDurationReset;
    public bool canEffectStack;

    [Header("On Hit Settings")]
    public bool doEffectOnHit;

    public abstract StatusEffect ApplyEffect(GameObject target);

    [Header("Visual")]
    public Sprite uIImage;
    public Color uIImageColor = Color.white;
}
