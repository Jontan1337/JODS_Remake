using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            GameObject bulletHole = ObjectPool.Instance.SpawnFromNetworkedPool(Tags.BulletHole, rayHit.point + rayHit.normal * 0.01f, Quaternion.identity, 5);
            bulletHole.transform.LookAt(rayHit.point + rayHit.normal);
        }
    }
}
