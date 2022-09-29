using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieCommon : UnitBase
{
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    [Title("Zombie Common", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private StatusEffectToApply damageStatusEffect = new StatusEffectToApply();
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
        while (true)
        {
            yield return new WaitForSeconds(1);

            Health++;
        }
    }

    #region Interface Functions

    #endregion
}