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
        print("htlh");

    }

    public override void ApplyDamageTrait()
    {
        print("dmg");
    }

    public override void ApplySpeedTrait()
    {
        print("speed");

    }


    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
