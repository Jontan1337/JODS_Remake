using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Mirror;
using DG.Tweening;

public class RaycastWeapon : RangedWeapon
{
    [Title("RAYCAST WEAPON", "", TitleAlignments.Centered), Space(30f)]
    [Header("Raycast Settings")]
    [SerializeField] private float range = 1000f;
    [Header("Settings")]
    [SerializeField] private LayerMask ignoreLayer = 13;
    [Header("References")]
    [SerializeField, Required] private ParticleSystem bulletTrail = null;
    [SerializeField, Required] private Light muzzleFlashLight = null;

    private Transform target;

    [Server]
    protected override void Svr_Shoot(Vector2 aimPoint)
    {
        Vector2 recoil = Random.insideUnitCircle * currentCurveAccuracy + aimPoint;
        Ray aimRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2 + recoil.x, Screen.height / 2 + recoil.y));
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, range, ~ignoreLayer))
        {
            Vector3 targetPoint = aimHit.point;
            Ray shootRay = new Ray(shootOrigin.position, targetPoint - shootOrigin.position);
            RaycastHit shootHit = new RaycastHit();
            float currentDamage = damage;
            for (int i = 0; i <= penetrationAmount; i++)
            {
                if (Physics.Raycast(shootRay, out shootHit, range, ~ignoreLayer))
                {
                    if (shootHit.collider.TryGetComponent(out IDamagable damagable))
                    {
                        BaseStatManager statManagerBase = shootHit.collider.GetComponentInParent<BaseStatManager>();
                        statManagerBase.onDied.AddListener(delegate { Svr_OnTargetDied(); });
                        bool detach = highPower ? highPower : Random.value > 0.6f;
                        if (detach)
                        {
                            if (shootHit.collider.TryGetComponent(out IDetachable detachable))
                            {
                                detachable.Detach((int)DamageTypes.Pierce);
                            }
                        }
                        if (shootHit.collider.TryGetComponent(out IDamagableTeam damagableTeam))
                        {
                            if (damagableTeam.Team == Teams.Player)
                            {
                                currentDamage /= 2;
                            }
                        }
                        damagable.Svr_Damage((int)currentDamage, owner);
                        statManagerBase.onDied.RemoveListener(delegate { Svr_OnTargetDied(); });
                    }
                    // This ray shoots it's own collider on the other side to get the "penetration point"
                    // on the opposite side of the collider where the bullet should leave.
                    Ray penRay = new Ray(shootHit.point + shootRay.direction * range, -shootRay.direction);
                    RaycastHit penHit;
                    if (shootHit.collider.Raycast(penRay, out penHit, range))
                    {
                        shootRay = new Ray(penHit.point, -penRay.direction);
                        float damageReduction = currentDamage * damageFallOff;
                        currentDamage = Mathf.Clamp(Mathf.RoundToInt(currentDamage - damageReduction), 1f, int.MaxValue);
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
        muzzleFlashLight.DOComplete();
        muzzleFlashLight.DOIntensity(0.4f, 0.01f).OnComplete(() => { muzzleFlashLight.DOIntensity(0f, 0.01f); });
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
                    if (shootHit.collider.TryGetComponent(out IDamagable damagable))
                    {
                        target = shootHit.collider.transform;
                        //if (highPower)
                        //{
                        //    if (damagable.IsDead)
                        //    {
                        //        if (shootHit.collider.TryGetComponent(out Rigidbody rb))
                        //        {
                        //            rb.AddForce(transform.forward * rigidbodyPunchback * 10f, ForceMode.Impulse);
                        //        }
                        //    }
                        //}
                    }
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

    [Server]
    private void Svr_OnTargetDied()
    {
        Rpc_OnTargetDied();
    }
    [ClientRpc]
    private void Rpc_OnTargetDied()
    {
        if (target.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(transform.forward * rigidbodyPunchback * 10f, ForceMode.Impulse);
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
