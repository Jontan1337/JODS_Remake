using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Mirror;

public abstract class UnitBase : NetworkBehaviour
{
    #region Fields

    [Header("Necessities")]
    [SerializeField] private SOUnit unitSO;

    [Header("Stats")]
    [SerializeField] private int health = 100;
    [SyncVar, SerializeField] private bool isDead = false;
    private int maxHealth = 0;
    private float upgradeMultiplier = 0f;
    [Space]
    [SerializeField] private bool isMelee;
    [SerializeField] private bool isRanged;
    [SerializeField] private bool hasSpecial;
    [System.Serializable]
    public class Melee
    {
        public int meleeDamage = 0;
        public float meleeRange = 0;
        public float meleeCooldown = 0;
        [Space]
        public bool canMelee = true;
    }
    [Space]
    public Melee melee;

    [System.Serializable]
    public class Ranged
    {
        public int rangedDamage = 0;
        public int minRange = 0;
        public int maxRange = 0;
        public float rangedCooldown = 0;
        [Space]
        public GameObject projectile;
        public int projectileSpeed;
        public Vector3 projectileSpawnLocation;
        public bool standStill = true;
        [Space]
        public bool directRangedAttack = false;
        [Space]
        public bool canRanged = false;
    }
    public Ranged ranged;

    [System.Serializable]
    public class Special
    {
        public float specialCooldown = 0;
        [Space]
        public bool canSpecial = false;
    }
    public Special special;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1.5f;
    [Space]
    [SerializeField] private float chaseTime = 10; //How long the unit will chase, if the unit can't see it's target

    [Header("Other")]
    [SerializeField] private int sightDistance = 20;
    [SerializeField] private float eyeHeight = 2f;
    [SerializeField] private int viewAngle = 50;
    [SerializeField] private LayerMask ignoreOnRaycast;
    [Space]
    [SerializeField] private int alertRadius = 0;
    [SerializeField] private bool canAlert = true;

    [Header("AI")]
    [SerializeField] protected Transform currentTarget = null;
    public NavMeshAgent navAgent;
    [SerializeField] private bool hasTarget = false;

    [Header("References")]
    [SerializeField] private SFXPlayer footsteps = null;
    [SerializeField] private SFXPlayer sounds = null;
    public Animator animator;

    #region Coroutine references

    private IEnumerator CoSearch;
    private Coroutine CoChase;
    private Coroutine CoAttack;

    #endregion

    private bool walking;
    private bool attackMelee;
    private bool attackRange;
    private bool attackSpecial;

    public bool Walking
    {
        get { return walking; }
        set
        {
            if (value == walking) return;
            walking = value;
            animator.SetBool("Walk", walking);
        }
    }
    public bool AttackMelee
    {
        get { return attackMelee; }
        set
        {
            if (value == attackMelee) return;
            attackMelee = value;
            animator.SetBool("Melee", attackMelee);
        }
    } 
    public bool AttackRange
    {
        get { return attackRange; }
        set
        {
            if (value == attackRange) return;
            attackRange = value;
            animator.SetBool("Ranged", attackRange);
        }
    }   
    public bool AttackSpecial
    {
        get { return attackSpecial; }
        set
        {
            if (value == attackSpecial) return;
            attackSpecial = value;
            animator.SetBool("Special", attackSpecial);
        }
    }
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            print($"{name}: lost {value} health. Current health: {health}");
            if (health > 0)
            {
                Die();
            }
        }
    }

    #endregion

    #region Start / Initial

    private void OnValidate()
    {
        if (unitSO) SetStats();
    }

    public virtual void Start()
    {
        CoSearch = SearchCoroutine();
        InitialUnitSetup();
        StartCoroutine(CoSearch);

        Invoke("PlaySound", 5f);
    }

    private void SetStats()
    {
        if (!unitSO)
        {
            Debug.LogError($"{name} had no Unit Scriptable Object assigned when it tried to set it's stats!");
            return;
        }

        health = unitSO.health;
        maxHealth = unitSO.health;

        upgradeMultiplier = unitSO.upgradeMultiplier;

        isMelee = unitSO.isMelee;
        isRanged = unitSO.isRanged;
        hasSpecial = unitSO.hasSpecial;

        melee.meleeDamage = unitSO.melee.meleeDamage;
        melee.meleeRange = unitSO.melee.meleeRange;
        melee.meleeCooldown = unitSO.melee.meleeCooldown;

        ranged.rangedDamage = unitSO.ranged.rangedDamage;
        ranged.minRange = unitSO.ranged.minRange;
        ranged.maxRange = unitSO.ranged.maxRange;
        ranged.rangedCooldown = unitSO.ranged.rangedCooldown;
        ranged.projectile = unitSO.ranged.projectile;
        ranged.projectileSpawnLocation = unitSO.ranged.projectileSpawnLocation;
        ranged.projectileSpeed = unitSO.ranged.projectileSpeed;
        ranged.standStill = unitSO.ranged.standStill;
        ranged.directRangedAttack = unitSO.ranged.directRangedAttack;

        special.specialCooldown = unitSO.special.specialCooldown;

        movementSpeed = unitSO.movementSpeed;

        sightDistance = unitSO.sightDistance;
        eyeHeight = unitSO.eyeHeight;
        viewAngle = unitSO.viewAngle;

        chaseTime = unitSO.chaseTime;

        alertRadius = unitSO.alertRadius;
        canAlert = unitSO.canAlert;
    }

    public void SetUnitSO(SOUnit myNewUnit)
    {
        if (!myNewUnit)
        {
            Debug.LogError($"{name} had no Unit Scriptable Object carried over when it tried to do it's initial setup!");
            return;
        }

        //Apply new Unit Scriptable Object, which stores all the data for the unit
        if (myNewUnit != unitSO) unitSO = myNewUnit;
    }

    private void InitialUnitSetup()
    {
        //Stats
        SetStats();

        //Nav Mesh 
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = movementSpeed;
        navAgent.stoppingDistance = Mathf.Clamp(melee.meleeRange - 1, 1f, 20f);

        //Animations -----------------------
        animator = GetComponent<Animator>();
        AnimatorOverrideController animationsOverride = new AnimatorOverrideController(unitSO.unitAnimator);

        //Override the default animations with the unit's animations
        animator.runtimeAnimatorController = animationsOverride;

        melee.canMelee = isMelee;
        ranged.canRanged = isRanged;

        if (hasSpecial) StartCoroutine(SpecialCooldownCoroutine());
    }

    public void SetLevel(int level)
    {
        for (int i = 1; i < level; i++)
        {
            // Use upgradeMultiplier somehow, which is (default) 0.2, so round it up to int?

            //this.maxHealth *= 1;
            //this.damage *= 1;
            //this.speed *= 1.25F;
            //this.distance *= 1.25F;
        }
    }

    #endregion

    #region Movement



    #endregion

    #region Sight

    private IEnumerator SearchCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            //Search ----------

            //Get a list of all colliders within Sight Distance
            Collider[] collidersHit = Physics.OverlapSphere(transform.position, sightDistance, ~ignoreOnRaycast);

            //Iterate through each of them, to see if they're players
            foreach(Collider col in collidersHit)
            {
                //If they are players
                if (col.CompareTag("Player"))
                {
                    //Can I see the player?
                    bool canSee = CanSee(col.transform);

                    //Is the player within my field of view?
                    bool inViewAngle = InViewAngle(col.transform);

                    //If I can see the player, and it is within my field of view
                    if (canSee && inViewAngle)
                    {
                        //I can see the player
                        AcquireTarget(col.transform);
                    }
                }
            }
        }
    }

    private void AcquireTarget(Transform newTarget)
    {
        //Stop searching for more survivors
        StopCoroutine(CoSearch);

        SetTarget(newTarget);
        HasTarget();

        CoChase = StartCoroutine(ChaseCoroutine());
        CoAttack = StartCoroutine(AttackCoroutine());

        print($"{name} : Target acquired");
    }

    private void LoseTarget()
    {
        //Lose the target
        SetTarget(null);
        HasTarget();

        //Stop the chase coroutine (stop chasing the survivor)
        StopCoroutine(CoChase);
        StopCoroutine(CoAttack);

        //Start the search coroutine (start searching for survivors)
        StartCoroutine(CoSearch);

        print($"{name} : Mission failed, we'll get em next time");
    }

    private bool HasTarget()
    {
        hasTarget = (currentTarget != null);
        return hasTarget;
    }
    
    private void SetTarget(Transform newTarget) => currentTarget = newTarget;

    #endregion

    #region Chase 

    private IEnumerator ChaseCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            //Can I see the player?
            if (CanSee(currentTarget))
            {
                chaseTime = unitSO.chaseTime;
                navAgent.SetDestination(currentTarget.position);

                //actionRanged
                //actionMelee
            }
            else
            {
                chaseTime -= 0.5f;
                if (chaseTime <= 0)
                {
                    //If chase time runs out, and it still can't see it's target. Stop chasing and lose the target.
                    LoseTarget();
                }
            }

            //Set Walk Animation
            Walking = navAgent.velocity.magnitude > 0.5f;
        }
    }

    #endregion

    #region Raycast Checks

    private bool CanSee(Transform target)
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + eyeHeight, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + 1f, target.position.z);
        Ray ray = new Ray(pos, targetPos - pos);
        if (Physics.Raycast(ray, out RaycastHit hit, sightDistance, ~ignoreOnRaycast))
        {
            return hit.transform == target;
        }
        return false;
    }

    private bool InViewAngle(Transform target)
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + eyeHeight, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y + 1.5f, target.position.z);

        Vector3 targetDir = targetPos - pos;
        float angle = Vector3.Angle(targetDir, transform.forward);

        Debug.DrawRay(pos, targetDir, angle <= viewAngle ? Color.green : Color.red, 0.5f);

        Debug.DrawRay(pos, transform.forward * sightDistance, Color.blue, 0.5f);

        return angle <= viewAngle;
    }

    #endregion

    #region Attacks

    //This function will apply the damage of the attack, to the target.
    //This happens either when an attack animation triggers this function
    //or when the unit shoots the player (not projectile, but direct hit, like a raycast).
    //Depending on the unit, special attacks can also trigger this function.
    public virtual void GiveDamage()
    {
        int damage = 0;
        if (AttackMelee)
        {
            AttackMelee = false;

            //This will check one last time if the survivor is within melee range. 
            //Because the survivor might have moved while a melee animation was happening.
            //Also checks if the survivor is still in view, not obstructed by an object.
            if (!WithinMeleeRange() || !CanSee(currentTarget)) return;

            //Apply the proper damage number
            damage = melee.meleeDamage;  
           
        }
        if (AttackRange) 
        {
            AttackRange = false;

            //Apply the proper damage number
            damage = ranged.rangedDamage; 
        }


        Damage(damage);

        //Start cooldowns
        if (isMelee) StartCoroutine(MeleeCooldownCoroutine());
        if (isRanged) StartCoroutine(RangedCooldownCoroutine());
        if (hasSpecial) StartCoroutine(SpecialCooldownCoroutine());
    }

    //This function is specifically for projectile damage
    public virtual void GiveProjectileDamage()
    {
        //Apply the proper damage number
        Damage(ranged.rangedDamage);
    }

    //This is the function that applies damage to the current target.
    //Give damage ----- THIS NEEDS TO CHANGE WHEN SURVIVORS ARE REMADE
    private void Damage(int damage) => currentTarget.GetComponent<IDamagable>()?.Svr_Damage(damage);

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Attack();
        }
    }

    public abstract void Attack();

    #region Cooldowns
    protected IEnumerator MeleeCooldownCoroutine()
    {
        yield return new WaitForSeconds(melee.meleeCooldown);

        melee.canMelee = true;
    }
    protected IEnumerator RangedCooldownCoroutine()
    {
        yield return new WaitForSeconds(ranged.rangedCooldown);

        ranged.canRanged = true;
    }
    protected IEnumerator SpecialCooldownCoroutine()
    {
        yield return new WaitForSeconds(special.specialCooldown);

        special.canSpecial = true;
    }
    #endregion

    #region Melee
    public virtual void MeleeAttack()
    {
        if (CanSee(currentTarget))
        {
            AttackMelee = true;
            transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
        }
        else Debug.LogWarning($"{name} can't see it's target. Is it's ignoreOnLayer mask set properly?");
    }
    protected bool CanMeleeAttack => WithinMeleeRange() && melee.canMelee;
    private bool WithinMeleeRange()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return  distance <= melee.meleeRange;
    }
    #endregion
    #region Ranged
    public virtual void RangedAttack()
    {
        if (CanSee(currentTarget))
        {
            AttackRange = true;
            transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
        }
        else Debug.LogWarning($"{name} can't see it's target. Is it's ignoreOnLayer mask set properly?" +
            $" Does it ignore UnitProjectile?");
    }
    protected bool CanRangedAttack => WithinRangedDistance() && ranged.canRanged;
    private bool WithinRangedDistance()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= ranged.maxRange && distance >= ranged.minRange;
    }
    public virtual void RangedShoot()
    {
        Debug.LogError("Direct ranged damage not implemented");
    }
    public virtual void SpawnProjectile()
    {
        if (ranged.directRangedAttack)
        {
            Debug.LogError($"Something went wrong here... {name} wanted to do direct ranged damage, but tried to spawn a projectile instead." +
                $" Is the correct method set in the animation event?");
            return;
        }

        //Spawn the projectile
        GameObject projectile = Instantiate(ranged.projectile, transform);
        projectile.transform.localPosition = ranged.projectileSpawnLocation;

        //Aim the projectile at the current target.
        projectile.transform.LookAt(new Vector3(currentTarget.position.x, projectile.transform.position.y, currentTarget.position.z));

        //Set the speed of the projectile
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        projectileRB.velocity = projectile.transform.forward * ranged.projectileSpeed;

        projectile.transform.SetParent(null);

        projectile.GetComponent<UnitProjectile>().unit = this;

        AttackRange = false; //Disables this bool, allowing the unit to do another ranged attack.
        StartCoroutine(RangedCooldownCoroutine()); //Start cooldown

        Debug.LogWarning("'standStill?' not implemented, unit still moves when shooting a projectile");
    }
    #endregion
    #region Special
    public virtual void SpecialAttack() { }
    protected bool CanSpecialAttack => special.canSpecial;

    #endregion

    #endregion

    #region Health

    public bool IsMaxHealth => health == maxHealth;

    public void Die()
    {
        print(name + ": Am dead");
    }

    #endregion

    #region Sounds

    public void PlaySound()
    {
        //Play random sound
        sounds.PlaySFX();

        //Call the method again after some time. If the Unit is chasing someone, the sound plays more frequently.
        //Invoke("PlaySound", Random.Range(seen ? 2f : 10f, seen ? 10f : 50f));
    }

    public void Footstep()
    {
        if (footsteps)
        {
            footsteps.PlaySFX();
        }
    }

    #endregion

    #region Other
    public void Trap() { /*trapped = !trapped;*/ }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (hasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(currentTarget.position.x, currentTarget.position.y + 2, currentTarget.position.z),0.6f);
        }
    }

    #endregion
}
