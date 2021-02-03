using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieCommon : UnitBase, IZombie, IControllable
{
    public StatusEffectSO infection;
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    public override void MeleeAttack()
    {
        base.MeleeAttack();
        Infect(currentTarget);
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }

    public void Infect(Transform target)
    {
        target.GetComponent<StatusEffectManager>().ApplyStatusEffect(infection.ApplyEffect(target.gameObject));
    }
    #endregion
}
