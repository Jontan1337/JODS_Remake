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
    [Header("References")]
    [SerializeField] private Transform playerHead;

    protected override void Shoot()
    {
        base.Shoot();
        Rpc_ShootFX();
        
        Ray aimRay = new Ray(playerHead.position, playerHead.forward);

        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);

            if (Physics.Raycast(shootRay, out RaycastHit shootHit, range, ~ignoreLayer))
            {
                shootHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);

                Rpc_Bullethole(shootHit.point, shootHit.normal);

                if (shootHit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable?.Svr_Damage(damage);
                    if (highPower)
                    {
                        if (shootHit.collider.TryGetComponent(out IDetachable detachable))
                        {
                            detachable.Detach((int)DamageTypes.Pierce);
                        }
                    }
                }
            }
        }
    }

    public override void Svr_Interact(GameObject interacter)
    {
        base.Svr_Interact(interacter);
        playerHead = interacter.GetComponent<LookController>().playerItemCamera.transform;
    }

    [ClientRpc]
    private void Rpc_Bullethole(Vector3 point, Vector3 normal)
    {
        GameObject bulletHole = ObjectPool.Instance.SpawnFromLocalPool(Tags.BulletHole, point + normal * 0.01f, Quaternion.identity, 5);
        bulletHole.transform.LookAt(point + normal);
    }
}
