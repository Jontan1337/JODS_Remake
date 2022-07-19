﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSlav : UnitBase, IZombie, IControllable
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
