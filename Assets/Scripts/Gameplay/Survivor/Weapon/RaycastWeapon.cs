using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RaycastWeapon : RangedWeapon
{
    [Header("RAYCAST WEAPON")]
    [SerializeField, TextArea(2, 2)] private string header3 = "";

    [Header("Raycast Settings")]
    [SerializeField] private float range = 1000f;
    [Header("Settings")]
    [SerializeField] private LayerMask ignoreLayer = 13;
    [Header("References")]
    [SerializeField] private ParticleSystem bulletTrail;

    protected override void Svr_Shoot(Vector2 aimPoint)
    {
        Vector2 recoil = Random.insideUnitCircle * currentCurveAccuracy + aimPoint;
        Ray aimRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2 + recoil.x, Screen.height / 2 + recoil.y));
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;

            //RaycastHit[] shootRayHits = Physics.RaycastAll(shootRay, range, ~ignoreLayer);
            //for (int i = 0; i <= penetrationAmount; i++)
            //{
            //    RaycastHit hit = shootRayHits[shootRayHits.Length];
            //    print(hit.collider.name);
            //    if (hit.collider.TryGetComponent(out IDamagable damagable))
            //    {
            //    }
            //}
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);
            RaycastHit shootHit = new RaycastHit();
            float currentDamage = damage;
            for (int i = 0; i <= penetrationAmount; i++)
            {
                if (Physics.Raycast(shootRay, out shootHit, range, ~ignoreLayer))
                {
                    if (shootHit.collider.TryGetComponent(out IDamagable damagable))
                    {
                        damagable.Svr_Damage((int)currentDamage, owner);
                        if (highPower)
                        {
                            if (damagable.IsDead)
                            {
                                if (shootHit.collider.TryGetComponent(out Rigidbody rb))
                                {
                                    rb.AddForce(transform.forward * visualPunchback * 10f, ForceMode.Impulse);
                                }
                            }
                            if (shootHit.collider.TryGetComponent(out IDetachable detachable))
                            {
                                detachable.Detach((int)DamageTypes.Pierce);
                            }
                        }
                    }
                    // This ray shoots it's own collider on the other side to get the "penetration point"
                    // on the opposite side of the collider where the bullet should leave.
                    Ray penRay = new Ray(shootHit.point + shootRay.direction * range, -shootRay.direction);
                    RaycastHit penHit;
                    if (shootHit.collider.Raycast(penRay, out penHit, range))
                    {
                        shootRay = new Ray(penHit.point, -penRay.direction);
                        currentDamage = Mathf.Clamp(Mathf.RoundToInt(currentDamage *= damageFallOff), 1f, int.MaxValue);
                    }
                }
                else
                {
                    break;
                }
            }
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
            for (int i = 0; i <= penetrationAmount; i++)
            {
                if (Physics.Raycast(shootRay, out RaycastHit shootHit, range, ~ignoreLayer))
                {
                    PhysicMaterial phyMat = shootHit.collider.sharedMaterial;
                    Bullethole(shootHit.point, shootHit.normal, phyMat ? phyMat.name : "");
                    Ray penRay = new Ray(shootHit.point + shootRay.direction * range, -shootRay.direction);
                    RaycastHit penHit;
                    if (shootHit.collider.Raycast(penRay, out penHit, range))
                    {
                        shootRay = new Ray(penHit.point, -penRay.direction);
                    }
                }
            }
        }
        else
        {
            Vector2 randomCircle = Random.insideUnitCircle;
            BulletTrail(
                (playerHead.forward + 
                new Vector3(
                    randomCircle.x,
                    randomCircle.y
                ) / 300f * (currentCurveAccuracy)) * range
            );
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
