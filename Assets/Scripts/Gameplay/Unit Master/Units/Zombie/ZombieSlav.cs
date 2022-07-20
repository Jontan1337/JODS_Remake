using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSlav : UnitBase, IControllable
{
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            TryMeleeAttack();
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
