using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grenade : NetworkBehaviour
{
    public GameObject explosionEffect;

    [SyncVar] public bool thrown;
    public float delay = 3f;
    public float destroyDelay = 10f;
    public float blastRadius = 5f;
    public int damage = 200;
    protected float countdown;
    protected bool boom;

    public void Explode()
    {
        explosionEffect = Instantiate(explosionEffect, transform.position, transform.rotation);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (var collider in colliders)
        {
            GameObject tempGO = collider.transform.root.gameObject;
            tempGO.GetComponent<IDamagable>()?.Svr_Damage(damage);
            tempGO.GetComponent<IExplodable>()?.Explode();
        }
        Remove();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Zombie")
		{
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
		}
	}
}
