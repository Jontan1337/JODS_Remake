using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTentacle : UnitBase, IControllable
{
    [Header("Tentacle")]
    [SerializeField] private bool CaughtSurvivor = false;

    public override void Attack()
    {
        //Tentacle Zombie will always try and do the special attack if it is available.
        //If the special failed, it will go on cooldown and it will begin using melee attacks for a bit
        if (special.canSpecial)
        {
            if (CanSpecialAttack)
            {
                TrySpecialAttack();
            }
        }
        else if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    public override void SpecialAttack()
    {
        AttackSpecial = false;
        StartCoroutine(SpecialCooldownCoroutine());

        //Try to capture a player, making them unable to move or use weapons.
        if (!WithinSpecialDistance())
        {
            LoseGrappledTarget();
            return;
        }
        //If successful ------

        if (special.statusEffectsToApply == null)
        {
            Debug.LogError(name + " had no grapple debuff assigned and could not grapple the target");
            return;
        }
        if (special.statusEffectsToApply.Count > 0)
        {
            foreach (StatusEffectToApply statusEffectToApply in special.statusEffectsToApply)
            {
                ApplyStatusEffect(statusEffectToApply.statusEffect, currentTarget, statusEffectToApply.amount);
            }
        }

        CaughtSurvivor = true;
        animator.SetBool("Grapple", true);

        StartCoroutine(GrabEnumerator());
    }

    private IEnumerator GrabEnumerator()
    {
        while (currentTarget)
        {
            if (!WithinSpecialDistance()) break;

            yield return new WaitForSeconds(0.5f);
        }

        LoseGrappledTarget();
    }

    public override void Svr_Die()
    {
        LoseGrappledTarget();
        base.Svr_Die();
    }

    private void LoseGrappledTarget()
    {
        animator.SetBool("Grapple", false);
        ResumeMovement();

        if (CaughtSurvivor)
        {
            if (currentTarget)
            {
                if (special.statusEffectsToApply.Count > 0)
                {
                    foreach (StatusEffectToApply statusEffectToApply in special.statusEffectsToApply)
                    {
                        currentTarget.GetComponent<StatusEffectManager>().Svr_RemoveStatusEffect(statusEffectToApply.statusEffect);
                    }
                }
                
            }
        }
    }

    public override void ApplyHealthTrait()
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyDamageTrait()
    {
        throw new System.NotImplementedException();
    }

    public override void ApplySpeedTrait()
    {
        throw new System.NotImplementedException();
    }

    public override void OnSelect()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnDeselect()
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
