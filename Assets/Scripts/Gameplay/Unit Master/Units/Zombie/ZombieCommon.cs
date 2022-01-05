using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieCommon : UnitBase, IZombie, IControllable
{
    public override void Attack()
    {
        if (!Infect())
        {
            return;
        }
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

    protected override void IncreaseStats()
    {
        base.IncreaseStats();
        if (unitLevel == 10)
        {
            //RUN! dududududududududu
        }
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
            Debug.LogError(name + " has no infection debuff! Assign the 'Infection' as the 'Status Effect To Apply' on the UnitSO");
            return false;
        }
        if (melee.amount == 0)
        {
            Debug.LogError(name + " has no infection amount!");
            return false;
        }
        return true;
    }
    #endregion
}
