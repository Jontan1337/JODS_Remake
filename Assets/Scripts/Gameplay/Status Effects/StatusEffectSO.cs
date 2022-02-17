using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    [Header("Timed Settings")]
    public bool activeUntilRemoved;
    public float duration = 1f;
    public float maxDuration = 5f;
    public bool canDurationStack;
    public bool canDurationReset;
    public bool canEffectStack;

    [Header("On Hit Settings")]
    public bool doEffectOnHit;

    public abstract StatusEffect ApplyEffect(GameObject target);

    [Header("Visual")]
    public bool useVisual = true;
    public Sprite[] uIImage;
    public Color uIImageColor = Color.white;
    [Space]
    public bool useImageAlpha = true;
    public bool useDurationAsAlpha = false;
}
