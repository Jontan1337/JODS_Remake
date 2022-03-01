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
    [SerializeField] private ParticleSystem bulletTrail;

    protected override void Shoot(Vector2 aimPoint)
    {
        Vector2 recoil = (Random.insideUnitCircle * currentCurveAccuracy) + aimPoint;
        Ray aimRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2 + recoil.x, Screen.height / 2 + recoil.y));
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);
            if (Physics.Raycast(shootRay, out RaycastHit shootHit, range, ~ignoreLayer))
            {
                PhysicMaterial phyMat = shootHit.collider.sharedMaterial;

                //Rpc_BulletEffect(aimHit.point, shootHit.point, shootHit.normal, phyMat ? phyMat.name : "");
                if (shootHit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.Svr_Damage(damage, owner);
                    if (highPower)
                    {
                        if (damagable.IsDead)
                        {
                            if (shootHit.collider.TryGetComponent(out Rigidbody rb))
                            {
                                rb.AddForce(transform.forward * visualPunchback, ForceMode.Impulse);
                            }
                        }
                        if (shootHit.collider.TryGetComponent(out IDetachable detachable))
                        {
                            detachable.Detach((int)DamageTypes.Pierce);
                        }
                    }
                }
            }
        }
        else
        {
            //Vector2 randomCircle = Random.insideUnitCircle;
            //Rpc_BulletEffect((playerHead.forward + new Vector3(
            //    randomCircle.x,
            //    randomCircle.y) / 200f * (currentCurveAccuracy)) * range,
            //    Vector3.zero, Vector3.zero, "");
        }
        Rpc_Shoot(recoil);
    }

    [ClientRpc]
    protected override void Rpc_Shoot(Vector2 recoil)
    {
        base.Rpc_Shoot(recoil);
        Ray aimRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2 + recoil.x, Screen.height / 2 + recoil.y));
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);
            BulletTrail(targetPoint);
            if (Physics.Raycast(shootRay, out RaycastHit shootHit, range, ~ignoreLayer))
            {
                PhysicMaterial phyMat = shootHit.collider.sharedMaterial;
                Bullethole(shootHit.point, shootHit.normal, phyMat ? phyMat.name : "");
            }
        }
        else
        {
            Vector2 randomCircle = Random.insideUnitCircle;
            //Rpc_BulletEffect((playerHead.forward + new Vector3(
            //    randomCircle.x,
            //    randomCircle.y) / 200f * (currentCurveAccuracy)) * range,
            //    Vector3.zero, Vector3.zero, "");
            BulletTrail(
                (playerHead.forward + 
                new Vector3(
                    randomCircle.x,
                    randomCircle.y
                ) / 300f * (currentCurveAccuracy)) * range
            );
            //BulletTrail(playerHead.forward * range);
        }
    }

    private void Bullethole(Vector3 point, Vector3 normal, string phyMatName)
    {
        if (GlobalVariables.SurfaceTypes.TryGetValue(phyMatName, out Tags fxTag))
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
