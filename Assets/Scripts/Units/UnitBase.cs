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
    private int maxHealth = 0;
    private float upgradeMultiplier = 0f;
    [Space]
    [SerializeField] private bool isMelee;
    [SerializeField] private bool isRanged;
    [SerializeField] private bool hasSpecial;
    [Space]
    [SerializeField] private int meleeDamage = 10;
    [SerializeField] private float meleeRange = 2.5f;
    private float meleeCooldown = 2;
    [Space]
    [SerializeField] private int rangedDamage = 0;
    [SerializeField] private int minRange = 2;
    [SerializeField] private int maxRange = 20;
    private float rangedCooldown = 2;

    private float specialCooldown = 2;

    [Space]
    [SerializeField] private bool canMelee = true;
    [SerializeField] private bool canRanged = true;
    [SerializeField] private bool canSpecial = false;

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
    [SyncVar, SerializeField] private bool isDead = false;

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

    [SerializeField] private bool walking;
    [SerializeField] private bool attackMelee;
    [SerializeField] private bool attackRange;
    [SerializeField] private bool attackSpecial;

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

        meleeDamage = unitSO.meleeDamage;
        meleeRange = unitSO.meleeRange;
        meleeCooldown = unitSO.meleeCooldown;

        rangedDamage = unitSO.rangedDamage;
        minRange = unitSO.minRange;
        maxRange = unitSO.maxRange;
        rangedCooldown = unitSO.rangedCooldown;

        specialCooldown = unitSO.specialCooldown;

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
        navAgent.stoppingDistance = Mathf.Clamp(meleeRange - 1, 1f, 20f);

        //Animations -----------------------
        animator = GetComponent<Animator>();
        AnimatorOverrideController animationsOverride = new AnimatorOverrideController(unitSO.unitAnimator);

        //Override the default animations with the unit's animations
        animator.runtimeAnimatorController = animationsOverride;

        canMelee = isMelee;
        canRanged = isRanged;

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
    //or when a projectile hits a survivor.
    //Depending on the unit, special attacks also trigger this function.
    public virtual void GiveDamage()
    {
        int damage = 0;
        if (AttackMelee)
        {
            AttackMelee = false;
            //This will check one last time if the survivor is within melee range. 
            //Because the survivor might have moved while a melee animation was happening
            if (!WithinMeleeRange()) return;

            //Apply the proper damage number
            damage = meleeDamage;  
           
        }
        if (AttackRange) 
        {
            //Apply the proper damage number
            damage = rangedDamage; 
            AttackRange = false; 
        }

        //Give damage ----- THIS NEEDS TO CHANGE WHEN SURVIVORS ARE REMADE
        currentTarget.GetComponent<IDamagable>()?.Svr_Damage(damage);

        //Start cooldowns
        if (isMelee) StartCoroutine(MeleeCooldownCoroutine());
        if (isRanged) StartCoroutine(RangedCooldownCoroutine());
        if (hasSpecial) StartCoroutine(SpecialCooldownCoroutine());
    }

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
        yield return new WaitForSeconds(meleeCooldown);

        canMelee = true;
    }
    protected IEnumerator RangedCooldownCoroutine()
    {
        yield return new WaitForSeconds(rangedCooldown);

        canRanged = true;
    }
    protected IEnumerator SpecialCooldownCoroutine()
    {
        yield return new WaitForSeconds(specialCooldown);

        canSpecial = true;
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
    protected bool CanMeleeAttack => WithinMeleeRange() && canMelee;
    private bool WithinMeleeRange()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return  distance <= meleeRange;
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
        else Debug.LogWarning($"{name} can't see it's target. Is it's ignoreOnLayer mask set properly?");
    }
    protected bool CanRangedAttack => WithinRangedDistance() && canRanged;
    private bool WithinRangedDistance()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= maxRange && distance >= minRange;
    }
    #endregion
    #region Special
    public virtual void SpecialAttack() { }
    protected bool CanSpecialAttack => canSpecial;

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
