using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System.Collections;

public class LiveEntity : NetworkBehaviour, IDamagable, IExplodable
{
    [Header("Basic entity settings")]
    [SyncVar(hook = nameof(CheckHealth)), SerializeField] private int health = 50;
    [SyncVar, SerializeField] private int maxHealth = 100;
    [SyncVar] private bool isDead = false;
    public EntityType entityType = EntityType.explosive;

    [Header("Explosive entity settings")]
    [SerializeField] private ParticleSystem explosionEffect = null;
    [SerializeField] private ParticleSystem criticalEffect = null;
    [SerializeField] private int criticalHealth = 20;
    [SerializeField] private float explosionRadius = 4.5f;
    [SerializeField] private int explosionDamage = 30;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float upwardExplosionForce = 1f;
    [SerializeField] private int criticalEffectDamage = 3;
    [SerializeField] private float criticalDamageInterval = 0.8f;
    [SerializeField] private int friendlyFireReduction = 16;

    [Header("NonExplosive entity settings")]
    [SerializeField] private GameObject[] pieces = null;
    [SerializeField] private bool singleDestructable = false;
    [SerializeField] private bool removeDebris = false;
    [SerializeField] private GameObject singleBrokenObject = null;
    [SerializeField] private SFXPlayer wallDestruction = null;
    [SerializeField] private bool destroySelf = false;

    [Header("Other entity settings")]
    [SyncVar] public bool isBurning = false;
    public bool destroyed = false;

    // Used for checking explosion hits
    private Vector3 fromPosMid = Vector3.zero;
    private Vector3 fromPosTop = Vector3.zero;
    private Vector3 fromPosBottom = Vector3.zero;
    private int damageLossOverDistance = 2;

    private Rigidbody[] explodableRBs = null;

    public enum EntityType
    {
        explosive,
        nonexplosive
    }

    public LayerMask zombiePartLayer = 0;


    // -- LOCAL PLAYER METHODS --

    public int Health { get => health; private set => health = value; }
    public int MaxHealth { get => maxHealth; private set => maxHealth = value; }
    public bool IsDead { get => isDead; private set => isDead = value; }
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
            DestroyEntity();
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
            StartCoroutine(DissolvePiece(piece));
        }
        if (destroySelf) Destroy(gameObject,10f);
    }
    private IEnumerator DissolvePiece(GameObject piece)
    {
        piece.transform.SetParent(null);
        float interval = Random.Range(3f, 5f);
        yield return new WaitForSeconds(interval);

        piece.GetComponent<Timer>()?.StartTimer(true, 5f);
    }

    public void DestroyEntity(Transform sourceOfExplosion = null)
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
            StartExplosionEffect();
            // Save all Colliders from the gameobjects the OverlapSphere hits.
            Collider[] tempHitObjects = Physics.OverlapSphere(transform.position, explosionRadius, ~zombiePartLayer); // Ignore individual zombie parts.
            // Run through each collider the OverlapSphere hit.
            foreach (Collider targetCollider in tempHitObjects)
            {
                // OverlapSphere also detects itself,
                // so we first check if the current target is that.
                if (targetCollider.gameObject == gameObject) { continue; }

                #region TryGet_Target_Components

                LiveEntity targetLE = null;
                Rigidbody targetRB = null;
                // Does the object it hit have a LiveEntity component.
                if (targetCollider.TryGetComponent(out LiveEntity tempLE))
                {
                    targetLE = tempLE;
                }
                // Does the object it hit have a Rigidbody component. Used for simple rigidbody objects near the explosion.
                if (targetCollider.TryGetComponent(out Rigidbody tempRB))
                {
                    targetRB = tempRB;
                    // Apply an ExplosionForce to the object Rigidbody
                    targetRB.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardExplosionForce, ForceMode.Impulse);
                }
                #endregion

                RaycastHit hit;
                // Temporary bool that checks if any of the raycasts hit a part of the target.
                bool tempCheckIsHit = false;

                #region Explosion_Raycast_Checking
                // -- MIDDLE --
                // Temporary position to look at the middle of the figure
                Vector3 tempPosMid = new Vector3(targetCollider.transform.position.x, targetCollider.transform.position.y + targetCollider.transform.localScale.y / 2, targetCollider.transform.position.z);
                // Did the raycast hit something and is it not Untagged
                tempCheckIsHit = Physics.Raycast(fromPosMid, tempPosMid - fromPosMid, out hit, explosionRadius) && hit.collider.tag != "Untagged";

                // -- TOP --
                // If the first raycast doesn't hit, check if the next does 
                if (!tempCheckIsHit)
                {
                    // Temporary position to look at the top of the figure
                    // if an object is in the way in the middle
                    Vector3 tempPosTop = new Vector3(tempPosMid.x, targetCollider.transform.position.y + targetCollider.transform.localScale.y - 0.01f, tempPosMid.z);
                    // Did the raycast hit something and is it not Untagged
                    tempCheckIsHit = Physics.Raycast(fromPosTop, tempPosTop - fromPosTop, out hit, explosionRadius) && hit.collider.tag != "Untagged";
                }

                // -- BOTTOM --
                // If the second raycast doesn't hit, check if the last does
                if (!tempCheckIsHit)
                {
                    // Temporary position to look at the top of the figure
                    // if an object is in the way at the top
                    Vector3 tempPosBottom = new Vector3(tempPosMid.x, targetCollider.transform.position.y, tempPosMid.z);
                    // Did the raycast hit something and is it not Untagged
                    tempCheckIsHit = Physics.Raycast(fromPosBottom, tempPosBottom - fromPosBottom, out hit, explosionRadius) && hit.collider.tag != "Untagged";
                }
                #endregion

                // Did any of the raycasts hit a part of the object
                if (tempCheckIsHit)
                {
                    GameObject tempGO = targetCollider.gameObject;
                    if (isServer)
                    {
                        IDamagable damagable = tempGO.GetComponent<IDamagable>();
                        int newExplosionDamage = explosionDamage;
                        // Check if explosion is from a rocket, then it is friendly fire.
                        //if (TryGetComponent(out WeaponType weaponType))
                        //    if (weaponType.name == "Rocket")
                        //        if (damagable?.Team == Teams.Player)
                        //            newExplosionDamage /= friendlyFireReduction;

                        int finalDamage = Mathf.Clamp(newExplosionDamage - (int)hit.distance * damageLossOverDistance, 0, int.MaxValue);
                        if (finalDamage > 0)
                            damagable?.Svr_Damage(finalDamage);
                    }
                    tempGO.GetComponent<IExplodable>()?.Explode(transform);
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
        // Is the entity a single destructable object or an explosive
        if (singleDestructable || entityType == EntityType.explosive)
        {
            Destroy(gameObject);
        }
    }

    private void StartExplosionEffect()
    {
        explosionEffect.Play();
        explosionEffect.gameObject.transform.parent = null;
        explosionEffect.GetComponent<SFXPlayer>()?.PlaySFX();
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

    int IDamagable.GetHealth() => health;

    bool IDamagable.IsDead() => isDead;
	#endregion
}
