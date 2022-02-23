using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Projectile : NetworkBehaviour
{
	[Header("Settings")]
	[SerializeField] private bool objectPooled = false; //TEMPORARY
	[Header("Projectile Stats")]
	public int damage = 0;
	[SerializeField] protected int lifetime = 5;
	[SerializeField] private bool destroyAfterLifetime = false;
	[SerializeField] private bool sticky = false;
	[SerializeField] protected Tags objectPoolTag;
	[SerializeField] public Transform owner;
	[Space]
	[SerializeField] protected bool hasDropoff = true;
	[SerializeField] protected int timeBeforeDropoff = 3;
	[Space]
	[SerializeField] protected bool piercing = false;


	[Header("Status Effect")]
	public List<StatusEffectSO> statusEffectsToApply = new List<StatusEffectSO>();
	public int amount;

	protected bool hasHit = false;

	protected Rigidbody rb;

	public virtual void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

    private void OnEnable()
    {
		if (hasDropoff)
		{
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
		if (destroyAfterLifetime)
		{
			yield return new WaitForSeconds(lifetime);
			ReturnObjectToPool(0);
		}
	}

	public void OnTriggerEnter(Collider col)
	{
		if (!isServer) return;

		OnHit(col);
	}
	private void OnCollisionEnter(Collision col)
	{
		if (!isServer) return;

		OnHit(col);
	}


	public virtual void OnHit(Collider objectHit)
	{
		if (!isServer) return;

		if (!piercing && !hasHit)
		{
			hasHit = true; //Prevents the projectile from hitting multiple times
			if (sticky)
			{
				transform.SetParent(objectHit.transform);
				rb.velocity = Vector3.zero;
			}
			else
			{
				ReturnObjectToPool(0);
			}
		}
	}

	public virtual void OnHit(Collision objectHit)
	{
		ContactPoint point = objectHit.GetContact(0);

		if (!piercing && !hasHit)
		{
			hasHit = true; //Prevents the projectile from hitting multiple times
			if (sticky)
			{
				transform.SetParent(objectHit.transform);
				rb.isKinematic = true;
				transform.localScale = transform.localScale;
				//rb.transform.position = point.point - rb.velocity.normalized * 30;
			}
			else
			{
				ReturnObjectToPool(0);
			}
		}
	}

	public virtual void ReturnObjectToPool(float time)
	{
		if (!objectPooled) return;
		ObjectPool.Instance.ReturnToNetworkedPool(objectPoolTag, gameObject, time);
	}



	public virtual void Destroy() => NetworkServer.Destroy(gameObject);

	protected void Damage(GameObject objectHit) => objectHit.GetComponent<IDamagable>()?.Svr_Damage(damage);
}
