using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTentacle : UnitBase, IZombie, IControllable
{
    [Header("Tentacle")]
    [SerializeField] private bool CaughtSurvivor = false;

    public override void Attack()
    {
        if (!Infect())
        {
            return;
        }

        if (CanSpecialAttack)
        {
            TrySpecialAttack();
        }
        else if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    public override void SpecialAttack()
    {
        Debug.Log("Gotcha bitch!");

        //Try to capture a player, making them unable to move or use weapons.

        if (special.statusEffectToApply == null)
        {
            Debug.LogError(name + " had no grapple debuff assigned and could not grapple the target");
            return;
        }
        currentTarget.GetComponent<StatusEffectManager>()?.ApplyStatusEffect(special.statusEffectToApply.ApplyEffect(currentTarget.gameObject));

        //If successful ------
        CaughtSurvivor = true;
        animator.SetBool("Grapple", true);
    }

    public override void OnSelect()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDeselect()
    {
        throw new System.NotImplementedException();
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }

    public bool Infect()
    {
        if (melee.statusEffectToApply == null)
        {
            Debug.LogError(name + " had no infection debuff assigned and could not infect the target");
            return false;
        }
        if (melee.amount == 0)
        {
            Debug.LogError(name + " had no infection amount and could not infect the target");
            return false;
        }
        return true;
    }
    #endregion
}
