using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImpacter
{
    Action<ImpactData> OnImpact { get; set; }
}

public struct ImpactData
{
    public float Amount { get; private set; }
    public ImpactSourceType SourceType { get; private set; }
    public ImpactData(float amount, ImpactSourceType sourceType)
    {
        this.Amount = amount;
        this.SourceType = sourceType;
    }
}

public enum ImpactSourceType
{
    Melee,
    Ranged
}