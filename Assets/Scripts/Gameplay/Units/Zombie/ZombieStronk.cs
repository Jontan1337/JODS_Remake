using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStronk : UnitBase, IZombie, IControllable
{
    [SerializeField]
    private InfectionSO infection;
    public InfectionSO Infection { get => infection; set => infection = value; }
    [SerializeField]
    private int infectionAmount = 15;
    public int InfectionAmount { get => infectionAmount; set => infectionAmount = value; }

    [Header("Stronk")]
    [SerializeField] private float destructibleSearchRange = 20f;
    [SerializeField] private LayerMask destructibleLayerMask = 1 << 17;
    [Space]
    [SerializeField] private ParticleSystem slamFX;
    


    public override void Start()
    {
        base.Start();
    }

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

    private IEnumerator SearchForDestructibleCo;
    Collider[] destructiblesInRange;
    private IEnumerator SearchForDestructible()
    {
        while (select.isSelected)
        {
            destructiblesInRange = Physics.OverlapSphere(transform.position, destructibleSearchRange, destructibleLayerMask);
            foreach (Collider destructible in destructiblesInRange)
            {
                destructible.GetComponent<Outline>().ShowOutline(0.5f, 2f);
            }

            yield return new WaitForSeconds(0.5f);
        }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, destructibleSearchRange);
    }
    private void OnDrawGizmos()
    {
        if (select.isSelected)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, destructibleSearchRange);
        }
    }
}
