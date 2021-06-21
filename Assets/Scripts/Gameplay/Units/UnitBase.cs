using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkAnimator))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(AudioSource))]

public abstract class UnitBase : NetworkBehaviour, IDamagable, IParticleEffect
{
    #region Fields

    [Header("Necessities")]
    [SerializeField] private UnitSO unitSO;

    [Header("Stats")]
    [SyncVar, SerializeField] private int health = 100; //Upgradeable
    [SyncVar] public bool isDead = false;
    private int maxHealth = 0;
    private float upgradeMultiplier = 0f;
    private int refundAmount = 0;
    [SyncVar, SerializeField] private int unitLevel = 1;
    [Space]
    [SerializeField] private bool isMelee;
    [SerializeField] private bool isRanged;
    [SerializeField] private bool hasSpecial;
    [System.Serializable]
    public class Melee
    {
        public int meleeDamageMin = 0; //Upgradeable
        public int meleeDamageMax = 0; //Upgradeable
        public float meleeRange = 0;
        public float meleeCooldown = 0;
        [Space]
        public bool canMelee = true;
        [Space]
        public StatusEffectSO statusEffectToApply = null;
        public int amount = 0;
    }
    [Space]
    public Melee melee;

    [System.Serializable]
    public class Ranged
    {
        public int rangedDamage = 0; //Upgradeable
        public int minRange = 0;
        public int maxRange = 0;
        public int rangedCooldown = 0;
        public int preferredRange = 0;
        [Space]
        public GameObject projectile;
        public int projectileSpeed;
        public Vector3 projectileSpawnLocation;
        public bool standStill = true;
        [Space]
        public bool directRangedAttack = false;
        [Space]
        public bool canRanged = false;
        [Space]
        public StatusEffectSO statusEffectToApply = null;
        public int amount = 0;
    }
    public Ranged ranged;

    [System.Serializable]
    public class Special
    {
        public int specialCooldown = 0;
        public int specialDamage = 0; //Upgradeable
        public float specialRange = 5;
        [Space]
        public bool standStill = true;
        public bool lookAtTarget = true;
        public bool availableFromStart = true;
        [Space]
        public bool canSpecial = false;
        [Space]
        public StatusEffectSO statusEffectToApply = null;
        public int amount = 0;
    }
    public Special special;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1.5f;
    [Space]
    [SerializeField] private float chaseTime = 10; //How long the unit will chase, if the unit can't see it's target
    [SerializeField] private bool stoppedMoving = false;

    [Header("Other")]
    [SerializeField] private int sightDistance = 20;
    [SerializeField] private float eyeHeight = 2f;
    [SerializeField] private int viewAngle = 50;
    [SerializeField] private LayerMask ignoreOnRaycast = 1 << 9;
    [SerializeField] private LayerMask survivorLayer = 1 << 13;
    [Space]
    [SerializeField] private int alertRadius = 0;
    [SerializeField] private bool canAlert = true;
    [SerializeField] private LayerMask alertMask = 1 << 9; //Unit is layer 9. We only want to alert Units
    [Space]
    [SerializeField] private Color particleColor = Color.red;

    [Header("AI")]
    [SerializeField] protected Transform currentTarget = null;
    public NavMeshAgent navAgent;
    [SerializeField] private bool hasTarget = false;
    [SerializeField] private bool permanentTarget = true;
    protected bool targetIsLiveEntity = false;

    [Header("References")]
    public Animator animator;

    [System.Serializable]
    public class Sounds
    {
        public float headHeight = 2f;
        [Space]
        public AudioClip[] idleSounds;
        [Range(0, 1)] public float idleVolume = 0.3f;
        [Space]
        public AudioClip[] meleeSounds;
        [Range(0, 1)] public float meleeVolume = 0.4f;
        [Space]
        public AudioClip rangedSound;
        [Range(0, 1)] public float rangedVolume = 0.4f;
        [Space]
        public AudioClip specialSound;
        [Range(0, 1)] public float specialVolume = 0.4f;
        [Space]
        public AudioClip[] footstepSounds;
        [Range(0, 1)] public float footstepVolume = 0.1f;
    }
    [Space]
    public Sounds sounds;
    private AudioSource audioSource;

    [System.Serializable]
    public class Selectable
    {
        public bool canSelect = true;
        public SkinnedMeshRenderer[] bodyPartsRenderers;
        public Material[] unitMats;
        public bool isSelected = false;
    }
    [Space]
    public Selectable select;

    #region Coroutine references

    private IEnumerator CoSearch;
    private Coroutine CoChase;
    private Coroutine CoAttack;
    bool searching = false;
    bool chasing = false; // These bools make sure that only one instance of each coroutine is running at a time.
    bool attacking = false;

    protected bool meleeCooldown = false;
    protected bool rangedCooldown = false; // Same witht these
    protected bool specialCooldown = false;

    public bool MeleeCooldown
    {
        get { return meleeCooldown; }

        set
        {
            meleeCooldown = value;
            melee.canMelee = !value;
        }
    }
    public bool RangedCooldown 
    {
        get { return rangedCooldown; }
        
        set 
        { 
            rangedCooldown = value;
            ranged.canRanged = !value;
        }
    }
    public bool SpecialCooldown
    {
        get { return specialCooldown; }

        set
        {
            specialCooldown = value;
            special.canSpecial = !value;
        }
    }

    #endregion

    #region Attack Bools

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
            //melee.canMelee = false;
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
            if (health <= 0)
            {
                Svr_Die();
            }
        }
    }
    public Color ParticleColor { get => particleColor; }

    #endregion

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
        if (!searching) { StartCoroutine(CoSearch); searching = true; }

        StartCoroutine(MovementAnimationCoroutine());
    }

    private void SetStats()
    {
        if (!unitSO)
        {
            Debug.LogError($"{name} had no Unit Scriptable Object assigned when it tried to set it's stats!" +
                $" - Did it get initialized by the Master?");
            return;
        }

        //Health
        health = unitSO.health;
        maxHealth = unitSO.health;

        //Upgrade
        upgradeMultiplier = unitSO.upgradeMultiplier;

        //Attack bools
        isMelee = unitSO.isMelee;
        isRanged = unitSO.isRanged;
        hasSpecial = unitSO.hasSpecial;

        //Melee
        melee.meleeDamageMin = unitSO.melee.meleeDamageMin;
        melee.meleeDamageMax = unitSO.melee.meleeDamageMax;
        melee.meleeRange = unitSO.melee.meleeRange;
        melee.meleeCooldown = unitSO.melee.meleeCooldown;
        melee.statusEffectToApply = unitSO.melee.statusEffectToApply;
        melee.amount = unitSO.melee.amount;

        //Ranged
        ranged.rangedDamage = unitSO.ranged.rangedDamage;
        ranged.minRange = unitSO.ranged.minRange;
        ranged.maxRange = unitSO.ranged.maxRange;
        ranged.rangedCooldown = unitSO.ranged.rangedCooldown;
        ranged.projectile = unitSO.ranged.projectile;
        ranged.projectileSpawnLocation = unitSO.ranged.projectileSpawnLocation;
        ranged.projectileSpeed = unitSO.ranged.projectileSpeed;
        ranged.standStill = unitSO.ranged.standStill;
        ranged.directRangedAttack = unitSO.ranged.directRangedAttack;
        ranged.preferredRange = unitSO.ranged.preferredRange;
        ranged.statusEffectToApply = unitSO.ranged.statusEffectToApply;
        ranged.amount = unitSO.ranged.amount;

        //Special
        special.specialCooldown = unitSO.special.specialCooldown;
        special.specialDamage = unitSO.special.specialDamage;
        special.specialRange = unitSO.special.specialRange;
        special.standStill = unitSO.special.standStill;
        special.lookAtTarget = unitSO.special.lookAtTarget;
        special.availableFromStart = unitSO.special.availableFromStart;
        special.statusEffectToApply = unitSO.special.statusEffectToApply;
        special.amount = unitSO.special.amount;

        //Movement
        movementSpeed = unitSO.movementSpeed;

        //Refunding
        refundAmount = unitSO.refundAmount;

        //Sight
        sightDistance = unitSO.sightDistance;
        eyeHeight = unitSO.eyeHeight;
        viewAngle = unitSO.viewAngle;

        //Chase
        chaseTime = unitSO.chaseTime;

        //Alert
        alertRadius = unitSO.alertRadius;
        canAlert = unitSO.canAlert;

        //Sounds
        sounds.headHeight = unitSO.sounds.headHeight;
        sounds.meleeSounds = unitSO.sounds.meleeSounds;
        sounds.meleeVolume = unitSO.sounds.meleeVolume;
        sounds.rangedSound = unitSO.sounds.rangedSound;
        sounds.rangedVolume = unitSO.sounds.rangedVolume;
        sounds.specialSound = unitSO.sounds.specialSound;
        sounds.specialVolume = unitSO.sounds.specialVolume;
        sounds.footstepSounds = unitSO.sounds.footstepSounds;
        sounds.footstepVolume = unitSO.sounds.footstepVolume;
        sounds.idleSounds = unitSO.sounds.idleSounds;
        sounds.idleVolume = unitSO.sounds.idleVolume;

        //Selecting
        select.canSelect = unitSO.select.canSelect;

        //Other
        particleColor = unitSO.bloodColor;
    }

    public void SetUnitSO(UnitSO myNewUnit)
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
        if (unitSO == null)
        {
            Debug.LogError($"{name} had no Unit Scriptable Object and could not be setup properly. " +
                $"This should not be possible, if the unit is spawned by the master." +
                $"If this unit was not spawned by the-- Master, then ignore this.");
            return;
        }

        //Stats
        SetStats();
        //If the unit's level is higher than 1 (base level), then increase stats.
        if (unitLevel > 1) IncreaseStats(); //Increase the stats based on what level the unit is.

        //Nav Mesh 
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = movementSpeed;
        navAgent.acceleration = 60;
        navAgent.stoppingDistance = Mathf.Clamp(melee.meleeRange - 0.5f, 1f, 20f);
        navAgent.avoidancePriority = Random.Range(1, 100);

        //Animations -----------------------
        animator = GetComponent<Animator>();
        AnimatorOverrideController animationsOverride = new AnimatorOverrideController(unitSO.unitAnimator);

        //Override the default animations with the unit's animations
        animator.runtimeAnimatorController = animationsOverride;

        melee.canMelee = isMelee;
        ranged.canRanged = isRanged;

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1; //Set the audiosource to be 3D
        audioSource.minDistance = 5;
        audioSource.maxDistance = 20;

        //This is because some units may not have their special available from the moment they spawn.
        if (hasSpecial) {
            if (special.availableFromStart) { special.canSpecial = true; }
            else { StartCoroutine(SpecialCooldownCoroutine()); }
        }

        SetMaterialsAndMeshes();
       
        //random unit size, just to make units look less alike
        transform.localScale = transform.localScale * Random.Range(0.9f, 1.1f);
    }
    
    private void SetMaterialsAndMeshes()
    {
        bool randomMat = unitSO.unitMaterials.Length != 0;
        select.unitMats = new Material[select.bodyPartsRenderers.Length];

        for (int i = 0; i < select.bodyPartsRenderers.Length; i++)
        {
            //Random Material to assign, if there are any
            Material newMat = randomMat ? unitSO.unitMaterials[Random.Range(0, unitSO.unitMaterials.Length)] : null;

            SkinnedMeshRenderer unitRenderer = select.bodyPartsRenderers[i];

            unitRenderer.material = new Material(randomMat ? //If the unit has different materials to choose from
                unitRenderer.sharedMaterial : //If not, use already assigned material.
                newMat //Assign a random material.
                );

            select.unitMats[i] = unitRenderer.sharedMaterial;
        }


        if (unitSO.unitMeshes.Length != 0)
        {
            Debug.LogWarning("TODO: fix body parts dynamic meshes");
            select.bodyPartsRenderers[0].sharedMesh = unitSO.unitMeshes[Random.Range(0, unitSO.unitMeshes.Length)];
        }
    }

    //This is called by the Master, who sets the unit's level, which increases it's stats.
    public void SetLevel(int level) => unitLevel = Mathf.Clamp(level,1,100);
    private void IncreaseStats()
    {
        float multiplier = 1 + (upgradeMultiplier * (unitLevel - 1));
        print("Stats multiplier not implemented (" + multiplier + ")");
    }

    #endregion

    #region Movement

    protected void StopMovement()
    {
        navAgent.speed = 0;
        stoppedMoving = true;
        navAgent.isStopped = true;
        navAgent.ResetPath();
    }
    protected void ResumeMovement()
    {
        if (stoppedMoving)
        {
            navAgent.speed = movementSpeed;
            stoppedMoving = false;
        }
    }

    private IEnumerator MovementAnimationCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.25f);

            //Set Walk Animation, if walking
            Walking = navAgent.velocity.magnitude > 0.1f;
        }
    }

    #endregion

    #region Sight

    private IEnumerator SearchCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.5f);

            //Search ----------

            //Get a list of all survivor colliders within Sight Distance
            Collider[] collidersHit = Physics.OverlapSphere(transform.position, sightDistance, survivorLayer);

            //Iterate through each of them, to see if they meet the requirements to be chased
            foreach (Collider col in collidersHit)
            {
                //Can I see the player?
                bool canSee = CanSee(col.transform);

                //Is the player within my field of view?
                bool inViewAngle = InViewAngle(col.transform);

                //If I can see the player, and it is within my field of view
                if (canSee && inViewAngle || Vector3.Distance(transform.position, col.transform.position) <= 10)
                {
                    //I can see the player, go go go!
                    AcquireTarget(col.transform, false, default);
                }
            }
        }
    }

    public virtual void AcquireTarget(Transform newTarget, bool alerted = false, bool closerThanCurrent = false, bool liveEntity = false)
    {
        targetIsLiveEntity = liveEntity;

        if (!closerThanCurrent)
        {
            if (HasTarget())
            {
                if (!CloserThanTarget(newTarget)) return;
            }
        }

        SetTarget(newTarget);

        NewTarget();

        if (!alerted) //If I got alerted, I won't alert others.
        {
            Alert();
        }
    }

    private void LoseTarget()
    {
        //Lose the target
        SetTarget(null);
        HasTarget();

        //Stop the chase coroutine (stop chasing the survivor)
        if (chasing) { StopCoroutine(CoChase); chasing = false; }
        if (attacking) { StopCoroutine(CoAttack); attacking = false; }

        //Start the search coroutine (start searching for survivors)
        if (!searching) { StartCoroutine(CoSearch); searching = true; }

        //Stop moving
        navAgent.isStopped = true;
        navAgent.ResetPath();

        //Set Walk Animation to not walk
        Walking = false;

        print($"{name} : Mission failed, we'll get em next time");
    }

    protected bool HasTarget()
    {
        hasTarget = (currentTarget != null);
        return hasTarget;
    }

    private void SetTarget(Transform newTarget, bool? permanent = false)
    {
        currentTarget = newTarget;
        permanentTarget = permanent ?? false; //If the target is permanent, the unit will never stop chasing the target.
    }

    private void NewTarget()
    {
        //Stop searching for more survivors
        if (searching) { StopCoroutine(CoSearch); searching = false; }

        if (HasTarget()) 
        {
            chaseTime = unitSO.chaseTime;
            navAgent.SetDestination(currentTarget.position);

            if (!chasing) { CoChase = StartCoroutine(ChaseCoroutine()); chasing = true; }
            if (!attacking) { CoAttack = StartCoroutine(AttackCoroutine()); attacking = true; }
        }
    }

    #endregion

    #region Chase 

    private IEnumerator ChaseCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.5f);

            if (!hasTarget) LoseTarget();

            //Can I see the player?
            if (CanSee(currentTarget) || permanentTarget) //Permanent targets don't need to be in view
            {
                chaseTime = unitSO.chaseTime;
                navAgent.SetDestination(currentTarget.position);

                //This whole thing could be optimized?
                //Currently it does a distance check every 0.5 seconds if ranged.
                if (isRanged)
                {
                    bool atDistance = AtPreferredDistance();
                    if (atDistance && !stoppedMoving) StopMovement();
                    else if (!atDistance && stoppedMoving && !attackRange)
                    {
                        ResumeMovement();
                    }
                    else if (stoppedMoving)
                    {
                        if (!LookingAtTarget())
                        {
                            LookAtTarget(); // only rotate towards the target if the target is not within a 10 degree angle
                        }
                    }
                }

                else
                {
                    if (NextToTarget()) StopMovement();
                    else if (!NextToTarget()) ResumeMovement();
                }
            }
            else
            {
                chaseTime -= 0.5f;
                if (chaseTime > unitSO.chaseTime * 0.75f)
                {
                    navAgent.SetDestination(currentTarget.position);
                }
                if (chaseTime <= 0)
                {
                    //If chase time runs out, and it still can't see it's target. Stop chasing and lose the target.
                    LoseTarget();
                }
            }

            if (!HasTarget()) yield break; //What the heck
            //This is to stop the coroutine from setting walk animation again... even though the code stops the coroutine? dun work for some reason
        }
    }

    private void Alert()
    {
        if (!canAlert) return;
        Collider[] adjacentUnits = Physics.OverlapSphere(transform.position, alertRadius, alertMask);
        foreach (Collider col in adjacentUnits)
        {
            if (col.gameObject == gameObject) continue;
            col.gameObject.GetComponent<UnitBase>().AcquireTarget(currentTarget, true, default);
        }
    }

    //This is currently only used in Offline
    public void SetPermanentTarget(Transform newTarget)
    {
        SetTarget(newTarget, true);

        NewTarget();

        //print($"{name} : Target acquired (Permanently)");
    }

    #endregion

    #region Raycast Checks

    protected bool CanSee(Transform target)
    {
        if (!target) return false;

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + eyeHeight, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + 1f, target.position.z);
        Ray ray = new Ray(pos, targetPos - pos);
        if (Physics.Raycast(ray, out RaycastHit hit, sightDistance, ~ignoreOnRaycast))
        {
            return hit.transform == target;
        }
        return false;
    }

    protected bool InViewAngle(Transform target)
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + eyeHeight, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y + 1.5f, target.position.z);

        Vector3 targetDir = targetPos - pos;
        float angle = Vector3.Angle(targetDir, transform.forward);

        Debug.DrawRay(pos, targetDir, angle <= viewAngle ? Color.green : Color.red, 0.5f);

        Debug.DrawRay(pos, transform.forward * sightDistance, Color.blue, 0.5f);

        return angle <= viewAngle;
    }

    protected bool LookingAtTarget()
    {
        if (!HasTarget()) return false;

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + eyeHeight, transform.position.z);
        Vector3 targetPos = new Vector3(currentTarget.position.x, transform.position.y + 1.5f, currentTarget.position.z);

        Vector3 targetDir = targetPos - pos;
        float angle = Vector3.Angle(targetDir, transform.forward);

        Debug.DrawRay(pos, targetDir, angle <= viewAngle ? Color.green : Color.red, 0.5f);

        Debug.DrawRay(pos, transform.forward * sightDistance, Color.blue, 0.5f);

        return angle <= 10;
    }

    protected bool CloserThanTarget(Transform compareTarget)
    {
        if (!HasTarget()) return true;
        if (compareTarget == null) return false;

        float newTargetDistance = Vector3.Distance(transform.position, currentTarget.position);
        float currentTargetDistance = Vector3.Distance(transform.position, compareTarget.position);

        return currentTargetDistance > newTargetDistance;
    }

    protected bool NextToTarget() => Vector3.Distance(transform.position, currentTarget.position) <= Mathf.Clamp(navAgent.radius + 0.4f,melee.meleeRange - 0.1f,100);

    #endregion

    #region Attacks

    //This is the function that applies damage to the current target.
    protected void Damage(int damage) => currentTarget.GetComponent<IDamagable>()?.Svr_Damage(damage);
    //This function is an override that is called by projectiles or other.
    //Called when not necessarily damaging the current target
    protected void Damage(int damage, GameObject objectHit) => objectHit.GetComponent<IDamagable>()?.Svr_Damage(damage);

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (!HasTarget())
            {
                LoseTarget();
                yield return null;
            }

            if (currentTarget.TryGetComponent(out IDamagable idmg))
            {
                if (idmg.IsDead())
                {
                    LoseTarget();
                }
                yield return null;
            }

            Attack();
        }
    }

    public abstract void Attack();

    #region Cooldowns
    protected IEnumerator MeleeCooldownCoroutine()
    {
        if (meleeCooldown) yield break; //if an instance is already running, exit this one.

        MeleeCooldown = true;

        yield return new WaitForSeconds(melee.meleeCooldown);

        MeleeCooldown = false;
    }
    protected IEnumerator RangedCooldownCoroutine()
    {
        if (rangedCooldown) yield break; //if an instance is already running, exit this one.

        RangedCooldown = true;

        yield return new WaitForSeconds(ranged.rangedCooldown);

        RangedCooldown = false;
    }
    protected IEnumerator SpecialCooldownCoroutine()
    {
        if (specialCooldown) yield break; //if an instance is already running, exit this one.

        SpecialCooldown = true;

        yield return new WaitForSeconds(special.specialCooldown);

        SpecialCooldown = false;
    }
    #endregion

    #region Melee
    public virtual void TryMeleeAttack()
    {
        if (CanSee(currentTarget) && CanMeleeAttack)
        {
            bool inRange = WithinMeleeRange(); //This could be optimized later
            if (inRange && !stoppedMoving) StopMovement();

            AttackMelee = true;
            LookAtTarget();
        }
        else Debug.LogWarning($"{name} can't see it's target. Is it's ignoreOnLayer mask set properly?" +
            $"Or maybe the unit is too close/inside the target?");
    }
    protected bool CanMeleeAttack => WithinMeleeRange() && melee.canMelee;
    protected bool WithinMeleeRange()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= melee.meleeRange;
    }
    public virtual void MeleeAttack() 
    {
        AttackMelee = false;
        ResumeMovement();

        //This will check one last time if the survivor is within melee range. 
        //Because the survivor might have moved while a melee animation was happening.
        //Also checks if the survivor is still in view, not obstructed by an object.
        if (!WithinMeleeRange() || !CanSee(currentTarget)) return;

        //Apply the proper damage number
        int damage = Random.Range(melee.meleeDamageMin, melee.meleeDamageMax + 1); //why the fok is max exclusive??? stoopid unity 

        if (melee.statusEffectToApply) ApplyStatusEffect(melee.statusEffectToApply, currentTarget, melee.amount);
        StartCoroutine(MeleeCooldownCoroutine());
        Damage(damage);
    }
    #endregion
    #region Ranged
    public virtual void TryRangedAttack()
    {
        if (CanSee(currentTarget) && CanRangedAttack)
        {
            AttackRange = true;
            LookAtTarget();
            if (ranged.standStill) StopMovement();
        }
        else Debug.LogWarning($"{name} can't see it's target. Is it's ignoreOnLayer mask set properly?" +
            $" Does it ignore UnitProjectile?");
    }
    protected bool CanRangedAttack => WithinRangedDistance() && ranged.canRanged;
    protected bool WithinRangedDistance()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= ranged.maxRange && distance >= ranged.minRange;
    }
    public virtual void RangedShoot()
    {
        AttackRange = false;

        if (ranged.standStill) ResumeMovement();

        if (ranged.statusEffectToApply) ApplyStatusEffect(ranged.statusEffectToApply, currentTarget, ranged.amount);

        //Apply the proper damage number
        Damage(ranged.rangedDamage);

        StartCoroutine(RangedCooldownCoroutine());
        Debug.LogError("Direct ranged damage not implemented");
    }
    public virtual void SpawnProjectile()
    {
        if (ranged.standStill) ResumeMovement();
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

        UnitProjectile uProjectile = projectile.GetComponent<UnitProjectile>();
        uProjectile.damage = ranged.rangedDamage;
        uProjectile.statusEffectToApply = ranged.statusEffectToApply;
        uProjectile.amount = ranged.amount;

        AttackRange = false; //Disables this bool, allowing the unit to do another ranged attack.
        StartCoroutine(RangedCooldownCoroutine()); //Start cooldown
    }
    protected bool AtPreferredDistance()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= ranged.preferredRange;
    }

    #endregion
    #region Special
    public virtual void TrySpecialAttack()
    {
        if (CanSee(currentTarget) && CanSpecialAttack)
        {
            AttackSpecial = true;
            if (special.lookAtTarget) LookAtTarget();
            if (special.standStill) StopMovement();
        }
        else Debug.LogWarning($"{name} can't see it's target. Is it's ignoreOnLayer mask set properly?" +
            $" Does it ignore UnitProjectile?");
    }
    protected bool CanSpecialAttack => WithinSpecialDistance() && special.canSpecial;
    protected bool WithinSpecialDistance()
    {
        if (!HasTarget()) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= special.specialRange;
    }
    //This is the actual special, which is usually called from an animation event.
    public virtual void SpecialAttack()
    {
        ResumeMovement();

        if (special.statusEffectToApply) ApplyStatusEffect(special.statusEffectToApply, currentTarget, special.amount);

        AttackSpecial = false;
        StartCoroutine(SpecialCooldownCoroutine()); 
    }

    #endregion

    protected void ApplyStatusEffect(StatusEffectSO effect, Transform target, int? amount = null)
    {
        if (effect == null || target == null)
        {
            return;
        }
        target.GetComponent<StatusEffectManager>()?.ApplyStatusEffect(effect.ApplyEffect(target.gameObject), amount);
    }

    #endregion

    #region Health

    public bool IsMaxHealth => health == maxHealth;


    [Server]
    public virtual void Svr_Die()
    {
        if (!isDead)
        {
            isDead = true; //Bool used to ensure this only happens once

            Svr_StopNavAgent();

            Svr_SetDeathAnimation();

            Svr_DisableCollider();

            Svr_PostDeath();
        }
    }

    #region Dismemberment

    [Server]
    public void Svr_Dismember(DamageTypes damageType, GameObject oldPart, GameObject newPart, GameObject bloodFX)
    {
        newPart.GetComponent<MeshRenderer>().material = new Material(oldPart.GetComponent<SkinnedMeshRenderer>().sharedMaterial);

        if (IsDead()) { 
            switch (damageType)
            {
                case DamageTypes.Blunt:

                    Svr_Dismember_BodyPart(oldPart, newPart, bloodFX);

                    break;
                case DamageTypes.Slash:

                    Svr_Dismember_BodyPart(oldPart, newPart, bloodFX);

                    break;
                case DamageTypes.Pierce:

                    Svr_Dismember_BodyPart(oldPart, newPart, bloodFX);

                    break;
            }
        }
    }

    [Server]
    private void Svr_Dismember_BodyPart(GameObject oldPart, GameObject newPart, GameObject bloodFX)
    {
        oldPart.SetActive(false);

        newPart.gameObject.SetActive(true);
        newPart.transform.SetParent(null);

        bloodFX.SetActive(true);

        Rigidbody newPartRB = newPart.GetComponent<Rigidbody>();
        newPartRB.isKinematic = false;

        Vector3 randomForce = new Vector3(Random.Range(-50, 50), Random.Range(-20, 20), Random.Range(-50, 50));

        newPartRB.AddForce(randomForce / 2);
        newPartRB.AddTorque(randomForce);

        Rpc_Dismember_BodyPart(oldPart, newPart, bloodFX, randomForce);
    }
    [ClientRpc]
    private void Rpc_Dismember_BodyPart(GameObject oldPart, GameObject newPart, GameObject bloodFX, Vector3 randomForce)
    {
        oldPart.SetActive(false);

        bloodFX.SetActive(true);

        newPart.gameObject.SetActive(true);
        newPart.transform.SetParent(null);

        Rigidbody newPartRB = newPart.GetComponent<Rigidbody>();
        newPartRB.isKinematic = false;

        newPartRB.AddForce(randomForce / 2);
        newPartRB.AddTorque(randomForce);

    }

    #endregion

    #region Post Death
    [Server]
    private void Svr_StopNavAgent()
    {
        //Stop NavMesh Movement
        navAgent.isStopped = true;
        navAgent.enabled = false;
        Rpc_StopNavAgent();
    }
    [ClientRpc]
    private void Rpc_StopNavAgent()
    {
        //Stop NavMesh Movement
        //navAgent.isStopped = true;
        navAgent.enabled = false;
    }
    [Server]
    private void Svr_SetDeathAnimation()
    {
        animator.SetTrigger("Die");
        //Activate Ragdoll effect, or death animation
        Rpc_SetDeathAnimation();
    }
    [ClientRpc]
    private void Rpc_SetDeathAnimation()
    {
        //Activate Ragdoll effect, or death animation
        animator.SetTrigger("Die");
    }
    [Server]
    private void Svr_DisableCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider c in colliders)
        {
            c.enabled = false;
        }
        Rpc_DisableCollider();
    }
    [ClientRpc]
    private void Rpc_DisableCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
    }
    [Server]
    private void Svr_PostDeath()
    {            
        //Stop all coroutines
        StopAllCoroutines();

        StartCoroutine(PostDeath());
        Rpc_PostDeath();
    }
    [ClientRpc]
    private void Rpc_PostDeath()
    {
        //Stop all coroutines
        StopAllCoroutines();

        StartCoroutine(PostDeath());
    }

    private IEnumerator PostDeath() 
    {
        yield return new WaitForSeconds(3);

        //Dissolve Effect
        //Start the DissolveCoroutine which slowly dissolves over a set amount of time.

        GetComponent<Timer>()?.StartTimer(true, 2.5f, select.unitMats);

        yield return new WaitForSeconds(3);
        //After 3 seconds, tell the server to destroy the object/unit
        Svr_Destroy();
    }
    [Server]
    private void Svr_Destroy()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    public Teams Team => throw new System.NotImplementedException();

    #endregion

    #region Sounds

    public void PlaySound(AudioClip[] clips, float volume, bool atHead)
    {
        if (clips.Length == 0)
        {
            Debug.LogWarning($"{name} could not play an audioclip. The clip array is empty.");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        audioSource.volume = volume;

        audioSource.pitch = Random.Range(0.9f, 1.1f);

        if (atHead)
        {
            Debug.Log("Playing sounds at head position is not implemented yet, playing at feet instead.");
            Vector3 headPos = new Vector3(transform.position.x, transform.position.y + sounds.headHeight, transform.position.z);
            audioSource.PlayOneShot(clip);
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    //This function is usually called by animation events.
    public void Footstep()
    {
        //          The sound clips     |   The sound volume  | play at the head?
        PlaySound(sounds.footstepSounds, sounds.footstepVolume, false);
    }

    #endregion

    #region Commanding Units & Refunding
    #region Selecting
    public void Select(Color highlightColor)
    {
        select.isSelected = true;

        OnSelect();

        if (select.unitMats.Length == 0)
        {
            Debug.LogWarning($"{name} had no material assigned and could not be highlighted. -" +
                $"Does it have a unitRenderer assigned?");
            return;
        }

        foreach (Material unitMat in select.unitMats)
        {
            unitMat.SetInt("_Highlight", 1);
            unitMat.SetColor("_HighlightColor", highlightColor);
        }
    }
    public void Deselect()
    {
        select.isSelected = false;

        OnDeselect();

        if (select.unitMats.Length == 0)
        {
            Debug.LogWarning($"{name} had no material assigned and could not be highlighted. -" +
                $"Does it have a unitRenderer assigned?");
            return;
        }

        foreach (Material unitMat in select.unitMats)
        {
            unitMat.SetInt("_Highlight", 0);
        }
    }
    #endregion

    public abstract void OnSelect();
    public abstract void OnDeselect();

    #region Commands

    public void MoveToLocation(Vector3 pos)
    {
        //If the unit meets the requirements to be commanded to move to a new location
        //Requirements : (Not Chasing)

        if (HasTarget()) return;
        if (chasing) return;


        navAgent.SetDestination(pos);
    }

    #endregion

    #region Refunding

    public int Refund()
    {
        //Meet the requirements to refund (Is max health)
        return IsMaxHealth ? refundAmount : 0;
    }

    #endregion

    #endregion

    #region Other
    bool tryingToLookAtTarget = false;
    private void LookAtTarget()
    {
        if (!HasTarget()) return;
        //transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
        if (!tryingToLookAtTarget)
        {
            //make sure only one instance of this coroutine is active at any time
            tryingToLookAtTarget = true;
            StartCoroutine(SmoothLookAt()); //This might be slow...
        }
    }
    private IEnumerator SmoothLookAt()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z)
            - transform.position);
        for (float t = 0f; t < 1; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, t / 1);
            yield return null;
        }
        tryingToLookAtTarget = false;
        transform.rotation = endRot;
    }

    public void Trap() { /*trapped = !trapped;*/ }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        //Has a target  =   Green
        //Is searching  =   Yellow
        //Is chasing    =   Red
        //Is Attacking  =   Blue

        if (currentTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(currentTarget.position.x, currentTarget.position.y + 4, currentTarget.position.z),0.6f);
        }
        if (searching)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), new Vector3(0.25f, 1, 0.25f));
        }
        if (chasing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), new Vector3(0.25f, 1, 0.25f));
        }
        if (attacking)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), new Vector3(0.25f, 1, 0.25f));
        }
        if (attackRange || attackSpecial)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), new Vector3(0.25f, 1, 0.25f));
        }
        if (ranged.canRanged || special.canSpecial)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 6, transform.position.z), new Vector3(0.25f, 1, 0.25f));
        }
        if (rangedCooldown || specialCooldown)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 7, transform.position.z), new Vector3(0.25f, 1, 0.25f));
        }
    }

    #endregion

    #region IDamagable

    [Server]
    public void Svr_Damage(int damage, Transform target = null)
    {
        if (isDead) return;
        if (target != null)
        {
            if (CloserThanTarget(target))
            {
                Debug.Log("I got shot by someone closer than my target");
                AcquireTarget(target, false, true);
            }
        }

        Health -= damage;

        animator.SetTrigger("Hit");
    }

    public int GetHealth()
    {
        return Health;
    }

    public bool IsDead()
    {
        return isDead;
    }

    #endregion
}