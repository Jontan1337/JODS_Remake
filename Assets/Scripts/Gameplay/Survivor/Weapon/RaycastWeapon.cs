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
            for (int i = 0; i <= penetrationAmount; i++)
            {
                if (Physics.Raycast(shootRay, out shootHit, range))
                {
                    print(shootHit.collider.transform.root.name);
                    Debug.DrawRay(shootRay.origin, shootRay.direction*shootHit.distance, Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)), 5f);

                    shootRay = new Ray(shootHit.point + shootRay.direction*range, shootOrigin.position - shootHit.point);
                    //Physics.Raycast(shootRay, out shootHit, range);
                    //shootRay = new Ray(shootRay.origin, shootRay.direction * (shootHit.distance + 5f));
                    if (shootHit.collider.TryGetComponent(out IDamagable damagable))
                    {
                        damagable.Svr_Damage(damage, owner);
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
            if (Physics.Raycast(shootRay, out RaycastHit shootHit, range, ~ignoreLayer))
            {
                PhysicMaterial phyMat = shootHit.collider.sharedMaterial;
                Bullethole(shootHit.point, shootHit.normal, phyMat ? phyMat.name : "");
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
