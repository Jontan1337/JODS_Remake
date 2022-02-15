using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class LiveEntity : NetworkBehaviour, IDamagable, IExplodable
{
    [Header("Basic entity settings")]
    [SyncVar(hook = nameof(CheckHealth)), SerializeField] private int health = 50;
    [SyncVar, SerializeField] private int maxHealth = 100;
    [SyncVar] private bool isDead = false;
    public EntityType entityType = EntityType.explosive;

    [Header("Explosive entity settings")]
    [SerializeField] private Tags explosionTag = Tags.ExplosionMedium;
    [Space]
    [SerializeField] private ParticleSystem explosionEffect = null;
    [SerializeField] private ParticleSystem criticalEffect = null;
    [SerializeField] private int criticalHealth = 20;
    [SerializeField] private float explosionRadius = 4.5f;
    [SerializeField] private int explosionDamage = 30;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float upwardExplosionForce = 1f;
    [SerializeField] private int criticalEffectDamage = 3;
    [SerializeField] private float criticalDamageInterval = 0.8f;
    //[SerializeField] private int friendlyFireReduction = 16;

    [Header("NonExplosive entity settings")]
    [SerializeField] private GameObject[] pieces = null;
    [SerializeField] private bool singleDestructable = false;
    [SerializeField] private bool removeDebris = false;
    [SerializeField] private GameObject singleBrokenObject = null;
    [SerializeField] private SFXPlayer wallDestruction = null;
    [SerializeField] private bool destroySelf = false;
    private LayerMask damageLayer = 0;
    private LayerMask lesserExplosionForce = 0;
    public Transform owner;

    [Header("Other entity settings")]
    [SerializeField] private Tags objectPoolTag = Tags.ExplosionMedium;
    [SerializeField] private bool objectPooled = false;
    [SyncVar] public bool isBurning = false;
    public bool destroyed = false;

    // Used for checking explosion hits
    private Vector3 fromPosMid = Vector3.zero;
    private Vector3 fromPosTop = Vector3.zero;
    private Vector3 fromPosBottom = Vector3.zero;
    private int damageLossOverDistance = 2;

    private Rigidbody[] explodableRBs = null;

    private void Awake()
    {
        lesserExplosionForce = LayerMask.GetMask("PickUp", "Weapon", "Dynamic Object");
        damageLayer = LayerMask.GetMask("Unit", "Survivor", "Dynamic Object");
    }

    public enum EntityType
    {
        explosive,
        nonexplosive
    }



    public int GetHealth => health;
    public bool IsDead => isDead;

    public int Health { get => health; private set => health = value; }
    public int MaxHealth { get => maxHealth; private set => maxHealth = value; }

    public Teams Team { get; private set; }

    private void OnValidate()
    {
        health = maxHealth;
    }

    private void CheckHealth(int oldInt, int newInt)
    {
        if (entityType == EntityType.explosive)
        {
            if (health <= criticalHealth)
            {
                CriticalEffects();
            }
        }
        if (health <= 0f)
        {
            Svr_DestroyEntity();
        }
    }

    private void CriticalEffects()
    {
        if (criticalEffect)
        {
            criticalEffect.Play();
            if (!isBurning)
            {
                if (isServer)
                    Burning();
            }
        }
    }

    private void Burning()
    {
        isBurning = true;
        health -= criticalEffectDamage;
        Invoke(nameof(Burning), criticalDamageInterval);
    }


    /// <summary>
    /// Transform is the source of the "explosion".
    /// </summary>
    /// <param name="sourceOfExplosion"></param>
    public void DestroyWall(Transform sourceOfExplosion = null)
    {
        // Get the BoxCollider of the parent that is used
        // to damage the object and then disable the collider
        // before adding explosionforce to the pieces.
        if (TryGetComponent(out BoxCollider tempCollider))
        {
            tempCollider.enabled = false;
        }
        explodableRBs = new Rigidbody[pieces.Length];
        int i = 0;
        // Run through all the pieces and set Rigidbody isKinematic to false.
        foreach (GameObject item in pieces)
        {
            explodableRBs[i] = item.GetComponent<Rigidbody>();
            i++;
            item.GetComponent<Rigidbody>().isKinematic = false;
        }
        // If a source of explosion is given/not null, add explosionforce to the pieces.
        if (sourceOfExplosion)
        {
            // Now that all the pieces are no longer kinematic,
            // apply an explosionforce to them.
            for (int x = 0; x < explodableRBs.Length; x++)
            {
                explodableRBs[x].AddExplosionForce(explosionForce, sourceOfExplosion.position, explosionRadius, upwardExplosionForce, ForceMode.Impulse);
            }
        }
        // Get the Nav Mesh Obstacle component and disable it so AI can pass through.
        // This is primarily meant for walls that has an obstacle component for navmeshes.
        TryGetComponent(out NavMeshObstacle navMeshObstacle);
        navMeshObstacle.enabled = false;

        if (removeDebris)
        {
            RemoveDebris();
            if (wallDestruction)
            {
                wallDestruction.PlaySFX();
            }
        }
        //gameObject.layer = 11;
    }
    private void RemoveDebris()
    {
        foreach (GameObject piece in pieces)
        {
            DissolvePiece(piece);
        }
        if (destroySelf) Destroy(gameObject, 10f);
    }
    private void DissolvePiece(GameObject piece)
    {
        piece.transform.SetParent(null);
        float interval = Random.Range(3f, 5f);

        piece.GetComponent<Timer>()?.StartTimer(true, 5f, interval);
    }

    [Server]
    public void Svr_DestroyEntity(Transform sourceOfExplosion = null)
    {
        isDead = true;
        // Prevent this entity from running code with bool
        // since Destroy(gameObject) takes some time
        // and will cause a repeating chain reaction with other explosives.
        if (destroyed) { return; }
        destroyed = true;

        // If this entity is an explosive
        if (entityType == EntityType.explosive)
        {
            // Get the middle, top and bottom of this gameobject
            // as to be more precise with other objects being hit by it.
            fromPosMid = new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z);
            fromPosTop = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
            fromPosBottom = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            // Play the explosion particle system effect.
            Rpc_StartExplosionEffect();
            // Save all Colliders from the gameobjects the OverlapSphere hits.
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, explosionRadius);	  // Run through each collider the OverlapSphere hit.
            List<GameObject> list = new List<GameObject>(); //Add all the gameobjects connected to each collider to a list
            foreach (Collider targetCollider in collidersInRange)
            {
                //One gameobject may container more than one collider
                if (!list.Contains(targetCollider.gameObject))
                {
                    list.Add(targetCollider.gameObject);
                }
            }
            foreach (GameObject target in list)
            {
                //store the collider component for later use
                Collider targetCollider = target.GetComponent<Collider>();

                // OverlapSphere also detects itself,
                // so we first check if the current target is that.
                if (target.gameObject == gameObject) { continue; }

                #region TryGet_Target_Components

                LiveEntity targetLE = null;
                Rigidbody targetRB = null;
                // Does the object it hit have a LiveEntity component.
                if (target.TryGetComponent(out LiveEntity tempLE))
                {
                    targetLE = tempLE;
                }
                // Does the object it hit have a Rigidbody component. Used for simple rigidbody objects near the explosion.
                if (lesserExplosionForce == (lesserExplosionForce | (1 << target.gameObject.layer)))
                {
                    ExplosionForce(targetCollider, explosionForce / 10);
                }
                else
                {
                    ExplosionForce(targetCollider, explosionForce);
                }
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
                if (damageLayer == (damageLayer | (1 << target.gameObject.layer)))
                {
                    if (tempCheckIsHit)
                    {
                        if (isServer)
                        {
                            IDamagable damagable = target.GetComponent<IDamagable>();
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
                            if (damagable.Team == Teams.Unit)
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
                    }
                }

            }
        }
        else if (entityType == EntityType.nonexplosive)
        {
            if (singleDestructable)
            {
                // If the breakable object contains one broken object
                Instantiate(singleBrokenObject, transform.position, transform.rotation);
            }
            else
            {
                DestroyWall(sourceOfExplosion);
            }
        }

        StartCoroutine(DestroyWait()); //This is necessary for some reason

    }



    IEnumerator DestroyWait()
    {
        yield return new WaitForSeconds(0.1f);
        // Is the entity a single destructable object or an explosive
        if (singleDestructable || entityType == EntityType.explosive)
        {
            if (objectPooled)
            {
                ObjectPool.Instance.ReturnToNetworkedPool(objectPoolTag, gameObject, 0);
            }
            else
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    private void ExplosionForce(Collider targetCollider, float explosionForce)
    {
        targetCollider.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardExplosionForce, ForceMode.Impulse);
    }

    [ClientRpc]
    private void Rpc_StartExplosionEffect()
    {
        ObjectPool.Instance.SpawnFromLocalPool(explosionTag, transform.position, Quaternion.identity, 1);
    }

    public void Explode(Transform explosionSource)
    {
        if (entityType != EntityType.nonexplosive) return;
        DestroyWall();
    }

    [Server]
    public void Svr_Damage(int damage, Transform target = null)
    {
        health -= damage;
    }

    public void Cmd_Damage(int damage)
    {
        throw new System.NotImplementedException();
    }

    #region Editor debugging tools
    // Check the explosion radius by selecting the object when using unity editor.
    private void OnDrawGizmosSelected()
    {
        if (entityType == EntityType.explosive)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
    #endregion
}
