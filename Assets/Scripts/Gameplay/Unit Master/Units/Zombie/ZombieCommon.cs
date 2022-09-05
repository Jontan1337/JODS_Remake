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
        StartCoroutine(Regeneration());
    }

    public override void ApplyDamageTrait()
    {
        melee.statusEffectsToApply.Add(damageStatusEffect);
    }

    public override void ApplySpeedTrait()
    {
        animator.SetBool("Run", true);
        movementSpeed += 3;
    }

    private IEnumerator Regeneration()
    {
        while (!IsDead)
        {
            yield return new WaitForSeconds(1);

            Health++;
        }
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}