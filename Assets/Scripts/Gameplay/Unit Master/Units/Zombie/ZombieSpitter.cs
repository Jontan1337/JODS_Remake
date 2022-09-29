using UnityEngine;

public class ZombieSpitter : UnitBase
{
    public override void Attack()
    {
        if (CanRangedAttack)
        {
            TryRangedAttack();
        }
        else if (CanMeleeAttack) 
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

    #region Interface Functions

    #endregion
}
