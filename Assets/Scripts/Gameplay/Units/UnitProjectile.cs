using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class UnitProjectile : NetworkBehaviour
{
    public int damage = 0;
    [Header("Projectile Stats")]
    [SerializeField] private int lifetime = 5;
    [Space]
    [SerializeField] private bool hasDropoff = true;
    [SerializeField] private int timeBeforeDropoff = 3;
    [Space]
    [SerializeField] private bool piercing = false;

    private Rigidbody rb;

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
        Destroy();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Damage(other.gameObject);

        if (!piercing)
        {
            Destroy();
        }
    }

    public virtual void Destroy() => NetworkServer.Destroy(gameObject);

    protected void Damage(GameObject objectHit) => objectHit.GetComponent<IDamagable>()?.Svr_Damage(damage);
}
