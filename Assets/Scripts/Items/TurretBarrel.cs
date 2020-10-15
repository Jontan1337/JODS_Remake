using UnityEngine;
using Mirror;
using System.Collections;

public class TurretBarrel : NetworkBehaviour
{
	private Animator animator;
	public EngineerTurret turret;
    public ParticleSystem ps;
    public SFXPlayer AS;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void Blood(Vector3 hit, Vector3 rot)
	{
		var blood = (GameObject)Resources.Load("Spawnables/FX/BloodSplatter1");
		blood = Instantiate(blood, hit, Quaternion.LookRotation(rot));
		NetworkServer.Spawn(blood);
        StartCoroutine(DestroyFX(1f, blood));
    }

    IEnumerator DestroyFX(float tiem, GameObject go)
    {
        yield return new WaitForSeconds(tiem);

        NetworkServer.Destroy(go);
    }

    [Server]
	public void DamageEnemy()
	{
		if (turret.target != null)
		{
            /*
			print(turret.target);
			if (turret.target.GetComponent<EnemyHealth>().health <= 0)
			{
				animator.SetBool("IsShooting", false);
				return;
			}
            ps.Play();
            AS.PlaySFX();
			Blood(turret.hit.point, turret.hit.normal);
			turret.target.GetComponent<EnemyHealth>().RpcDamage(turret.damage, false);
            */
		}
	}
}
