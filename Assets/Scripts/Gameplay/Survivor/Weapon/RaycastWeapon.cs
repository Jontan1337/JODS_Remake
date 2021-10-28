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
        
        Ray aimRay = new Ray(playerHead.position + new Vector3(0f, 0.1f), playerHead.forward);

        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);

            //Bullet Trail FX           This is only happening on server! make rpc for it or something
            GameObject fx = ObjectPool.Instance.SpawnFromLocalPool(Tags.BulletTrail, shootOrigin.position, Quaternion.identity, 1);
            fx.transform.forward = targetPoint - shootOrigin.position;
            //-----------------------------------------------------------------------------------------------------------------

            if (Physics.Raycast(shootRay, out RaycastHit shootHit, range, ~ignoreLayer))
            {
                Debug.DrawRay(shootOrigin.position, targetPoint - shootOrigin.position, Color.green, 2f);

                if (shootHit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable?.Svr_Damage(damage, owner);
                    if (highPower)
                    {
                        if (shootHit.collider.TryGetComponent(out IDetachable detachable))
                        {
                            detachable.Detach((int)DamageTypes.Pierce);
                        }
                    }
                }
                else
                {
                    // Only bulletholes on things that can't take damage for now.
                    Rpc_Bullethole(shootHit.point, shootHit.normal);
                }
                
            }
        }
    }

    [Server]
    public override void Svr_Interact(GameObject interacter)
    {
        base.Svr_Interact(interacter);
        playerHead = interacter.GetComponent<LookController>().RotateVertical;
    }

    [ClientRpc]
    private void Rpc_Bullethole(Vector3 point, Vector3 normal)
    {
        GameObject bulletHole = ObjectPool.Instance.SpawnFromLocalPool(Tags.BulletHole, point + normal * 0.01f, Quaternion.identity, 5);
        bulletHole.transform.LookAt(point + normal);
    }
}
