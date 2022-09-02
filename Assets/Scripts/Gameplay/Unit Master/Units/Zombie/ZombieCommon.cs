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
        melee.statusEffectsToApply.Add(damageStatusEffect);
    }

    public override void ApplySpeedTrait()
    {
        print(name + " Speed");
        animator.SetBool("Run", true);
        movementSpeed += 3; //this is overridden by unitbase because movement speed is set with = not +=
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}