using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RaycastWeapon : RangedWeapon
{
    [Header("Raycast Settings")]
    [SerializeField] private float range = 1000f;
    [Header("Settings")]
    [SerializeField] private LayerMask ignoreLayer = 13;

    protected override void Shoot()
    {
        base.Shoot();
        Rpc_ShootFX();
        Ray shootRay = new Ray(shootOrigin.position, transform.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
        {
            rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);

            //bullet hole

            Rpc_Bullethole(rayHit.point, rayHit.normal);

            IDamagable damagable = null;

            if (rayHit.collider.TryGetComponent(out damagable))
            {
                damagable?.Svr_Damage(damage);
                if (highPower)
                {
                    if (rayHit.collider.TryGetComponent(out IDetachable detachable))
                    {
                        detachable.Detach((int)DamageTypes.Pierce);
                    }
                }
            }
        }
    }

    [ClientRpc]
    private void Rpc_Bullethole(Vector3 point, Vector3 normal)
    {
        GameObject bulletHole = ObjectPool.Instance.SpawnFromLocalPool(Tags.BulletHole, point + normal * 0.01f, Quaternion.identity, 5);
        bulletHole.transform.LookAt(point + normal);
    }
}
