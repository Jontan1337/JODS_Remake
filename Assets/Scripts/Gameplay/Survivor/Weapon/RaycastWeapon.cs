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
    [SerializeField, SyncVar] private Transform playerHead;
    [SerializeField] private ParticleSystem bulletTrail;

    protected override void Shoot()
    {
        base.Shoot();
        Rpc_Shoot();
        
        Ray aimRay = new Ray(playerHead.position + new Vector3(0f, 0.1f), playerHead.forward);
        
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);

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
            }
        }
    }

    [Server]
    public override void Svr_Interact(GameObject interacter)
    {
        base.Svr_Interact(interacter);
        playerHead = interacter.GetComponent<LookController>().RotateVertical;
        Rpc_GetPlayerHead(playerHead);
    }
    [ClientRpc]
    private void Rpc_GetPlayerHead(Transform head)
    {
        playerHead = head;
    }

    [ClientRpc]
    protected override void Rpc_Shoot()
    {
        base.Rpc_Shoot();
        Ray aimRay = new Ray(playerHead.position + new Vector3(0f, 0.1f), playerHead.forward);
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);
            BulletTrail(targetPoint);
            if (Physics.Raycast(shootRay, out RaycastHit shootHit, range, ~ignoreLayer))
            {
                Debug.DrawRay(shootOrigin.position, targetPoint - shootOrigin.position, Color.green, 2f);
                PhysicMaterial phyMat = shootHit.collider.sharedMaterial;
                Bullethole(shootHit.point, shootHit.normal, phyMat ? phyMat.name : "");
            }
        }
        else
        {
            BulletTrail(playerHead.forward * range);
        }
    }

    private void Bullethole(Vector3 point, Vector3 normal, string phyMatName)
    {
        if (GlobalVariables.hallo.TryGetValue(phyMatName, out Tags fxTag))
        {
            GameObject bulletHole = ObjectPool.Instance.SpawnFromLocalPool(fxTag, point + normal * 0.01f, Quaternion.identity, 5);
            bulletHole.transform.LookAt(point + normal);
        }
    }

    private void BulletTrail(Vector3 direction)
    {
        bulletTrail.transform.forward = direction - shootOrigin.position;
        bulletTrail.Emit(1);
    }
}
