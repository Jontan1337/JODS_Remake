using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieCommon : UnitBase, IZombie, IControllable
{
    [SerializeField]
    private InfectionSO infection;
    public InfectionSO Infection { get => infection; set => infection = value; }
    [SerializeField]
    private int infectionAmount = 15;
    public int InfectionAmount { get => infectionAmount; set => infectionAmount = value; }

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

        if (!WithinMeleeRange() || !CanSee(currentTarget)) return;

        Infect(currentTarget);
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }

    public void Infect(Transform target)
    {
        if (infection == null)
        {
            Debug.LogError(name + " had no infection debuff assigned and could not infect the target");
            return;
        }
        target.GetComponent<StatusEffectManager>()?.ApplyStatusEffect(infection.ApplyEffect(target.gameObject), infectionAmount);
    }
    #endregion
}
