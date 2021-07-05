using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Projectile : NetworkBehaviour
{
    public int damage = 0;

    [Header("Projectile Stats")]
    [SerializeField] protected int lifetime = 5;
    [SerializeField] private bool destroyAfterLiftime = false;
    [Space]
    [SerializeField] protected bool hasDropoff = true;
    [SerializeField] protected int timeBeforeDropoff = 3;
    [Space]
    [SerializeField] protected bool piercing = false;


    [Header("Status Effect")]
    public StatusEffectSO statusEffectToApply;
    public int amount;

    protected bool hasHit = false;

    protected Rigidbody rb;

    public virtual void Start()
    {
        if (hasDropoff)
        {
            rb = GetComponent<Rigidbody>();
            StartCoroutine(DropoffEnumerator());
        }

        StartCoroutine(LifetimeEnumerator());
    }

    public virtual IEnumerator DropoffEnumerator()
    {
        yield return new WaitForSeconds(timeBeforeDropoff);

        rb.useGravity = true;
    }

    public virtual IEnumerator LifetimeEnumerator()
    {
        yield return new WaitForSeconds(lifetime);
        if (destroyAfterLiftime)
        {
            Destroy();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        OnHit(other);        
    }

    public virtual void OnHit(Collider objectHit)
    {
        if (!piercing && !hasHit)
        {
            hasHit = true; //Prevents the projectile from hitting multiple times
            Destroy();
        }
    }

    public virtual void Destroy() => NetworkServer.Destroy(gameObject);

    protected void Damage(GameObject objectHit) => objectHit.GetComponent<IDamagable>()?.Svr_Damage(damage);
}
