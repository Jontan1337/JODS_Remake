using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EngineerTurret : NetworkBehaviour
{
	public GameObject turretForward;
	Animator turretAnim;
	public Animator barrelAnim;
	float range = 10000;
	public int damage;
	int defaultDamage = 100;
	public RaycastHit hit;
	bool zombieFound;
	public LayerMask ignoreLayer;
	public GameObject target;
	public float duration = 10;
    public GameObject explosionFx;

	void Start()
	{
		turretAnim = GetComponent<Animator>();

        Invoke(nameof(EndTurret), duration);
	}

	void Update()
	{
        /*
		Debug.DrawRay(turretForward.transform.position, turretForward.transform.forward * range);

		if (Physics.Raycast(turretForward.transform.position, turretForward.transform.forward, out hit, range, ~ignoreLayer))
		{
            EnemyHealth e = hit.collider.transform.root.GetComponent<EnemyHealth>();

            if (e)
			{
                if (e.dead) {
                    zombieFound = false;
                    turretAnim.enabled = true;
                    turretAnim.SetBool("IsSearching", true);
                }
                else
                {
                    target = hit.collider.transform.root.gameObject;
                    zombieFound = true;
                }
            }

			switch (hit.collider.tag)
			{
				// ----- ZOMBIE PARTS START-----
				case "Head":
					damage = defaultDamage * 2;
					break;

				case "Torso":
					damage = defaultDamage;
					break;

				case "Arm":
					damage = defaultDamage / 2;
					break;

				case "Leg":
					damage = defaultDamage / 3;
					break;
				default:
					break;
			}
		}
		else
		{
			damage = defaultDamage;
			if (zombieFound)
			{
				turretAnim.enabled = true;
				turretAnim.SetBool("IsSearching", true);
				zombieFound = false;
			}

		}
		if (zombieFound)
		{
			turretForward.transform.LookAt(hit.transform);
			barrelAnim.SetBool("IsShooting", true);
			turretAnim.SetBool("IsSearching", false);
			turretAnim.enabled = false;
		}
        */
	}

    private void EndTurret()
    {
        var fx = Instantiate(explosionFx, transform.position, Quaternion.identity);
        Destroy(fx, 4f);
        Destroy(gameObject);
    }
	
    public void StopSearching()
    {
        turretAnim.SetBool("IsSearching", false);
    }

	//public Transform GetClosestEnemy(GameObject[] objects)
	//{

	//	Transform tMin = null;
	//	float minDist = Mathf.Infinity;
	//	Vector3 currentPos = transform.position;
	//	foreach (GameObject t in objects)
	//	{
	//		if (t.Equals(this.gameObject)) { continue; }
	//		if (t.Equals(gameObject)) { continue; }
	//		float dist = Vector3.Distance(t.transform.position, currentPos);
	//		if (dist < minDist)
	//		{
	//			tMin = t.transform;
	//			minDist = dist;
	//		}
	//	}
	//	return tMin;
	//}
}
