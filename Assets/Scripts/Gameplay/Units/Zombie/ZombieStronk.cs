using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStronk : UnitBase, IZombie, IControllable
{
    [Header("Stronk")]
    [SerializeField] private float destructibleSearchRange = 20f;
    [SerializeField] private LayerMask destructibleLayerMask = 1 << 17;
    [Space]
    [SerializeField] private ParticleSystem slamFX;
    
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

    public override void MeleeAttack()
    {
        if (targetIsLiveEntity) SpecialAttack();

        base.MeleeAttack();
    }

    public override void SpecialAttack()
    {
        print("sepck");
        if (!WithinMeleeRange() || !CanSee(currentTarget)) return;

        currentTarget.GetComponent<LiveEntity>().DestroyEntity(transform);

        base.SpecialAttack();
    }

    private IEnumerator SearchForDestructibleCo;
    Collider[] destructiblesInRange;
    private IEnumerator SearchForDestructible()
    {
        while (select.isSelected)
        {
            destructiblesInRange = Physics.OverlapSphere(transform.position, destructibleSearchRange, destructibleLayerMask);
            foreach (Collider destructible in destructiblesInRange)
            {
                destructible.GetComponent<Outline>().ShowOutline(0.5f,
                    destructible.transform == currentTarget ? 8 : 2f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public override void AcquireTarget(Transform newTarget, bool alerted, bool closerThanCurrent = false, bool liveEntity = false)
    {
        print("o shit boi a wall");

        base.AcquireTarget(newTarget, alerted, closerThanCurrent, liveEntity);
    }

    public override void OnSelect()
    {
        SearchForDestructibleCo = SearchForDestructible();
        StartCoroutine(SearchForDestructibleCo);
    }

    public override void OnDeselect()
    {
        StopCoroutine(SearchForDestructibleCo);
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
