using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStronk : UnitBase
{
    [Header("Stronk")]
    [SerializeField] private float destructibleSearchRange = 20f;
    [SerializeField] private LayerMask destructibleLayerMask = 1 << 17;
    [Space]
    [SerializeField] private ParticleSystem slamFX;
    
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    public override void MeleeAttack()
    {
        //if (targetIsLiveEntity) SpecialAttack();

        base.MeleeAttack();
    }

    public override void SpecialAttack()
    {
        print("sepck");
        if (!WithinMeleeRange() || !CanSee(currentTarget)) return;

        currentTarget.GetComponent<LiveEntity>().Svr_DestroyEntity(transform);

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
        base.AcquireTarget(newTarget, alerted, closerThanCurrent, liveEntity);
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
        SearchForDestructibleCo = SearchForDestructible();
        StartCoroutine(SearchForDestructibleCo);
    }

    public override void OnDeselect()
    {
        StopCoroutine(SearchForDestructibleCo);
    }

    #region Interface Functions

    #endregion
}
