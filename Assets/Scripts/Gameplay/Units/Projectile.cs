using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Projectile : NetworkBehaviour
{

	[Header("Projectile Stats")]
	public int damage = 0;
	[SerializeField] protected int lifetime = 5;
	[SerializeField] private bool destroyAfterLiftime = false;
	[SerializeField] private bool sticky = false;
	[SerializeField] protected string objectPoolTag = "";
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
		rb = GetComponent<Rigidbody>();
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
		yield return new WaitForSeconds(lifetime);
		if (destroyAfterLiftime)
		{
			ReturnObjectToPool(0);
		}
	}

	public void OnTriggerEnter(Collider col)
	{
		OnHit(col);
	}
	private void OnCollisionEnter(Collision col)
	{
		OnHit(col);
	}


	public virtual void OnHit(Collider objectHit)
	{
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
		ObjectPool.Instance.ReturnToNetworkedPool(objectPoolTag, gameObject, time);
	}



	public virtual void Destroy() => NetworkServer.Destroy(gameObject);

	protected void Damage(GameObject objectHit) => objectHit.GetComponent<IDamagable>()?.Svr_Damage(damage);
}
