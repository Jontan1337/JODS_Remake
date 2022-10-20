using Mirror;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public sealed class Explosive : NetworkBehaviour, IExplodable
{
    [Title("Explosive entity settings")]
    [SerializeField] private Tags explosionTag = Tags.ExplosionMedium;
    [SerializeField] private float explosionRadius = 4.5f;
    [SerializeField] private int explosionDamage = 30;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float upwardExplosionForce = 1f;
    [SerializeField] private int damageLossOverDistance = 2;
    [SerializeField] private LayerMask layerMask;

    public Transform owner;

    public bool Exploded { get; private set; }

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Unit", "Survivor", "Dynamic Object");
    }

    // Used for checking explosion hits
    private Vector3 fromPosMid = Vector3.zero;
    private Vector3 fromPosTop = Vector3.zero;
    private Vector3 fromPosBottom = Vector3.zero;

    [ClientRpc]
    private void Rpc_ExplosionForce(GameObject target, float explosionForce)
    {
        explosionForce *= Random.Range(0.75f, 1.25f); //just some randomness for visual satisfaction

        target.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardExplosionForce, ForceMode.Impulse);
    }

    [ClientRpc]
    private void Rpc_StartExplosionEffect()
    {
        ObjectPool.Instance.SpawnFromLocalPool(explosionTag, transform.position, Quaternion.identity, 5);
    }

    [Server]
    public void Explode(Transform explosionSource = null)
    {
        if (Exploded) return;
        Exploded = true;
        // Get the middle, top and bottom of this gameobject
        // as to be more precise with other objects being hit by it.
        fromPosMid = new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z);
        fromPosTop = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
        fromPosBottom = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Play the explosion particle system effect.
        Rpc_StartExplosionEffect();

        // Filter through all objects with multiple colliders before doing a foreach.
        // Save all Colliders from the gameobjects the OverlapSphere hits.
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, explosionRadius, layerMask); // Run through each collider the OverlapSphere hit.
        List<GameObject> list = new List<GameObject>(); //Add all the gameobjects connected to each collider to a list
        foreach (Collider targetCollider in collidersInRange)
        {
            // One gameobject may contain more than one collider
            if (!list.Contains(targetCollider.gameObject))
            {
                list.Add(targetCollider.gameObject);
            }
        }
        // Go through the filtered list.
        foreach (GameObject target in list)
        {
            // OverlapSphere also detects itself,
            // so we first check if the current target is that.
            if (target.gameObject == gameObject) { continue; }

            #region TryGet_Target_Components

            // Does the object it hit have a Rigidbody component. Used for simple rigidbody objects near the explosion.
            //if (lesserExplosionForce == (lesserExplosionForce | (1 << target.gameObject.layer))) wtf is dis, seems dumb - John
            //{
            //    ExplosionForce(targetCollider, explosionForce / 10);
            //}
            //else
            //{
            //    ExplosionForce(targetCollider, explosionForce);
            //}

            Rpc_ExplosionForce(target, explosionForce);
            #endregion

            RaycastHit hit;
            // Temporary bool that checks if any of the raycasts hit a part of the target.
            bool tempCheckIsHit = false;

            #region Explosion_Raycast_Checking
            // -- MIDDLE --
            // Temporary position to look at the middle of the figure
            Vector3 tempPosMid = new Vector3(target.transform.position.x, target.transform.position.y + target.transform.localScale.y / 2, target.transform.position.z);
            // Did the raycast hit something and is it not Untagged
            tempCheckIsHit = Physics.Raycast(fromPosMid, tempPosMid - fromPosMid, out hit, explosionRadius);

            // -- TOP --
            // If the first raycast doesn't hit, check if the next does 
            if (!tempCheckIsHit)
            {
                // Temporary position to look at the top of the figure
                // if an object is in the way in the middle
                Vector3 tempPosTop = new Vector3(tempPosMid.x, target.transform.position.y + target.transform.localScale.y - 0.01f, tempPosMid.z);
                // Did the raycast hit something and is it not Untagged
                tempCheckIsHit = Physics.Raycast(fromPosTop, tempPosTop - fromPosTop, out hit, explosionRadius);
            }

            // -- BOTTOM --
            // If the second raycast doesn't hit, check if the last does
            if (!tempCheckIsHit)
            {
                // Temporary position to look at the top of the figure
                // if an object is in the way at the top
                Vector3 tempPosBottom = new Vector3(tempPosMid.x, target.transform.position.y, tempPosMid.z);
                // Did the raycast hit something and is it not Untagged
                tempCheckIsHit = Physics.Raycast(fromPosBottom, tempPosBottom - fromPosBottom, out hit, explosionRadius);
            }
            #endregion

            // Did any of the raycasts hit a part of the object
            if (tempCheckIsHit)
            {
                if (isServer)
                {
                    IDamagableTeam damagable = target.GetComponent<IDamagableTeam>();
                    int newExplosionDamage = explosionDamage;
                    // Check if explosion is from a rocket, then it is friendly fire.
                    //if (TryGetComponent(out WeaponType weaponType))
                    //    if (weaponType.name == "Rocket")
                    //        if (damagable?.Team == Teams.Player)
                    //            newExplosionDamage /= friendlyFireReduction;

                    int finalDamage = Mathf.Clamp(newExplosionDamage - (int)hit.distance * damageLossOverDistance, 0, int.MaxValue);
                    if (finalDamage > 0)
                        damagable?.Svr_Damage(finalDamage, owner);

                    //Is this a unit that we're damaging?
                    if (damagable?.Team == Teams.Unit)
                    {
                        float chance = 1 - (((hit.distance / explosionRadius) * 100) * 0.01f);

                        if (chance > 0)
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                bool chanceToDetach = Random.value < chance;
                                if (chanceToDetach)
                                {
                                    bool explode = chance > 0.75f ? Random.value > 0.30f : Random.value > 0.70f;
                                    target.GetComponent<UnitBase>()?.Dismember_BodyPart(i, explode ? 1 : 0);
                                }
                            }
                        }
                    }
                }
                target.GetComponent<IExplodable>()?.Explode(transform);
                WaitDestroy();
            }
        }
    }

    private async void WaitDestroy()
    {
        await JODSTime.WaitTime(0.1f);
        NetworkServer.Destroy(gameObject);
    }

}