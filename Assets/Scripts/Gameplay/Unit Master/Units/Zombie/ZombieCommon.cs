using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieCommon : UnitBase, IControllable
{
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    public override void OnSelect()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnDeselect()
    {
        //throw new System.NotImplementedException();
    }
    [Header("Traits")]
    [SerializeField] private StatusEffectToApply damageStatusEffect;
    public override void ApplyHealthTrait()
    {
        //throw new System.NotImplementedException();
    }

    public override void ApplyDamageTrait()
    {
        Debug.Log("Hallo" + melee.statusEffectsToApply.Count);
        melee.statusEffectsToApply.Add(damageStatusEffect);
    }

    public override void ApplySpeedTrait()
    {
        //throw new System.NotImplementedException();
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}