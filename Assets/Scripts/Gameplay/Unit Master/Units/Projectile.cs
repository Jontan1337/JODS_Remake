using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

public abstract class Projectile : NetworkBehaviour
{
	[Header("State")]
	[SerializeField] private bool active = false;
	public virtual void Activate() { active = true; }

	[Header("Settings")]
	[SerializeField] private bool objectPooled = false; //TEMPORARY
	[SerializeField, ShowIf("objectPooled", true)] protected Tags objectPoolTag;
	[Space]
	[SerializeField] private bool enabledFromAwake = true;
	[Header("Projectile Stats")]
	public int damage = 0;
	[SerializeField] protected int lifetime = 5;
	[SerializeField] private bool destroyAfterLifetime = false;
	[SerializeField] private bool sticky = false;
	[SerializeField] public Transform owner;
	[Space]
	[SerializeField] protected bool hasDropoff = true;
	[SerializeField] protected int timeBeforeDropoff = 3;
	[Space]
	[SerializeField] protected bool piercing = false;


	[Header("Status Effect")]
	public List<StatusEffectToApply> statusEffectsToApply = new List<StatusEffectToApply>();

	protected bool hasHit = false;

	protected Rigidbody rb;

	public virtual void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

    public override void OnStartServer()
    {
		if (!isServer) return;
		if (!enabledFromAwake) return;
		else active = true;
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

			if (objectPooled)
            {
				ReturnObjectToPool(0);
            }
            else
            {
				Destroy();
            }
		}
	}

	public void OnTriggerEnter(Collider col)
	{
		if (!isServer || !active) return;

		OnHit(col);
	}
	private void OnCollisionEnter(Collision col)
	{
		if (!isServer || !active) return;

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
				print(objectHit.transform);
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
				Rpc_OnHit(objectHit.transform);
            }
			else
			{
				ReturnObjectToPool(0);
			}
		}
	}

	public void Rpc_OnHit(Transform objectHit)
    {
        transform.SetParent(objectHit);
		if (rb)
        {
			rb.isKinematic = true;
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
