using System.Collections;
using UnityEngine;
using Pathfinding;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkAnimator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Dissolve))]
[RequireComponent(typeof(StatusEffectManager))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]

public abstract class UnitBase : NetworkBehaviour, IDamagable, IParticleEffect
{
    #region Fields

    [Header("Necessities")]
    [SerializeField] private UnitSO unitSO;

    [Header("Stats")]
    [SyncVar, SerializeField] private int health = 100; //Upgradeable
    [SyncVar] public bool isDead = false;
    private int maxHealth = 0;
    private int refundAmount = 0;
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
        //public bool standStill = true;
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
        public GameObject TEMPProjectilePrefab;
        public Tags projectileTag;
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
        public float specialTriggerRange = 5;
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

    private enum AttackAnimation{
        Melee,
        Ranged,
        Special
    }

    [HideInInspector] public float[] statModifiers;

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
    [SerializeField] private bool permanentTarget = true;
    public bool canPathfind = true;
    protected bool targetIsLiveEntity = false;
    private Seeker seeker;
    private AIPath ai;
    private CharacterController controller;
    Path path;

    [Header("References")]
    public Animator animator;
    [Space]
    [SerializeField] private SkinnedMeshRenderer bodyRenderer = null;
    [SerializeField] private SkinnedMeshRenderer headRenderer = null;
    [SerializeField] private SkinnedMeshRenderer leftArmRenderer = null;
    [SerializeField] private SkinnedMeshRenderer rightArmRenderer = null;

    [Header("Detatchable References")]
    [SerializeField] private UnitBodyPart leftArm = null;
    [SerializeField] private UnitBodyPart rightArm = null;
    [SerializeField] private UnitBodyPart head = null;

    [System.Serializable]
    public class Sounds
    {
        public float headHeight = 2f;
        [Space]
        public float basePitch = 1f;
        [Space]
        public AudioClip[] idleSounds;
        [Range(0, 1)] public float idleVolume = 0.3f;
        [Space]
        public AudioClip[] alertSounds;
        [Range(0, 1)] public float alertVolume = 0.3f;
        [Range(0, 1)] public float alertingSoundChance = 0.9f;
        [Range(0, 1)] public float alertedSoundChance = 0.15f;
        [Space]
        public AudioClip[] meleeSounds;
        [Range(0, 1)] public float meleeVolume = 0.4f;
        [Range(0, 1)] public float meleeSoundChance = 0.6f;
        [Space]
        public AudioClip[] rangedSounds;
        [Range(0, 1)] public float rangedVolume = 0.4f;
        [Range(0, 1)] public float rangedSoundChance = 1f;
        [Space]
        public AudioClip[] specialSounds;
        [Range(0, 1)] public float specialVolume = 0.4f;
        [Range(0, 1)] public float specialSoundChance = 1f;
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

    [Header("Ragdoll")]
    [SerializeField] private bool canRagdoll = true;

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
            projectileSpawns = value == true ? 0 : 1;
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

    [SyncVar] public bool walking;
    private bool attackMelee;
    private bool attackRange;
    private bool attackSpecial;

    public bool Walking
    {
        get { return walking; }
        set
        {
            if (value == walking) return;
            //Only update if the new value is actually different

            if (isServer) walking = value;

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
            Rpc_PlayAttackAnimation(AttackAnimation.Melee, attackMelee);
            if (Random.value < sounds.meleeSoundChance) PlaySound(sounds.meleeSounds, sounds.meleeVolume, true);
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
            Rpc_PlayAttackAnimation(AttackAnimation.Ranged, attackRange);
            if (Random.value < sounds.rangedSoundChance) PlaySound(sounds.rangedSounds, sounds.rangedVolume, true);
        }
    }
    public bool AttackSpecial
    {
        get { return attackSpecial; }
        set
        {
            if (value == attackSpecial) return;
            attackSpecial = value;
            Rpc_PlayAttackAnimation(AttackAnimation.Special, attackSpecial);
            if (Random.value < sounds.specialSoundChance) PlaySound(sounds.specialSounds, sounds.specialVolume, true);
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

    public int GetHealth => health;
    public bool IsDead => isDead;

    #endregion

    #region Start / Initial

    private void OnValidate()
    {
        if (unitSO) SetStats();
    }

    private void OnDisable()
    {
        if (seeker == null) return;
        seeker.pathCallback -= OnPathComplete;
    }

    public virtual void Start()
    {
        if (canRagdoll)
        {
            Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
        InitialUnitSetup();
        
        StartCoroutine(MovementAnimationCoroutine());

        if (!isServer)
        {
            ai.enabled = false;
            seeker.enabled = false;
            //controller.enabled = false;

            return;
        }

        CoSearch = SearchCoroutine();
        if (!searching) { StartCoroutine(CoSearch); searching = true; }
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
        int newMaxHealth = Mathf.RoundToInt(unitSO.health * statModifiers[0]);
        Health = newMaxHealth;
        maxHealth = newMaxHealth;

        //Attack bools
        isMelee = unitSO.hasMelee;
        isRanged = unitSO.hasRanged;
        hasSpecial = unitSO.hasSpecial;

        //Melee
        if (isMelee)
        {
            melee.meleeDamageMax = Mathf.RoundToInt(melee.meleeDamageMax + (unitSO.melee.meleeDamageMax * statModifiers[1]));
            melee.meleeDamageMin = Mathf.RoundToInt(melee.meleeDamageMin + (unitSO.melee.meleeDamageMin * statModifiers[1]));
            melee.meleeRange = unitSO.melee.meleeRange;
            melee.meleeCooldown = unitSO.melee.meleeCooldown;
            melee.statusEffectToApply = unitSO.melee.statusEffectToApply;
            melee.amount = unitSO.melee.amount;
        }

        //Ranged
        if (isRanged)
        {
            ranged.rangedDamage = Mathf.RoundToInt(ranged.rangedDamage + (unitSO.ranged.rangedDamage * statModifiers[1]));
            ranged.minRange = unitSO.ranged.minRange;
            ranged.maxRange = unitSO.ranged.maxRange;
            ranged.rangedCooldown = unitSO.ranged.rangedCooldown;
            ranged.projectileTag = unitSO.ranged.projectileTag;
            ranged.projectileSpawnLocation = unitSO.ranged.projectileSpawnLocation;
            ranged.projectileSpeed = unitSO.ranged.projectileSpeed;
            ranged.standStill = unitSO.ranged.standStill;
            ranged.directRangedAttack = unitSO.ranged.directRangedAttack;
            ranged.preferredRange = unitSO.ranged.preferredRange;
            ranged.statusEffectToApply = unitSO.ranged.statusEffectToApply;
            ranged.amount = unitSO.ranged.amount;
        }

        //Special
        if (hasSpecial)
        {
            if (special.specialDamage != 0) special.specialDamage = Mathf.RoundToInt(special.specialDamage + (unitSO.special.specialDamage * statModifiers[1]));
            special.specialCooldown = unitSO.special.specialCooldown;
            special.specialTriggerRange = unitSO.special.specialTriggerRange;
            special.specialRange = unitSO.special.specialRange;
            special.standStill = unitSO.special.standStill;
            special.lookAtTarget = unitSO.special.lookAtTarget;
            special.availableFromStart = unitSO.special.availableFromStart;
            special.statusEffectToApply = unitSO.special.statusEffectToApply;
            special.amount = unitSO.special.amount;
        }

        //Movement
        movementSpeed = unitSO.movementSpeed * statModifiers[2];

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
        sounds.basePitch = unitSO.sounds.basePitch;
        sounds.meleeSounds = unitSO.sounds.meleeSounds;
        sounds.meleeVolume = unitSO.sounds.meleeVolume;
        sounds.meleeSoundChance = unitSO.sounds.meleeSoundChance;
        sounds.alertSounds = unitSO.sounds.alertSounds;
        sounds.alertVolume = unitSO.sounds.alertVolume;
        sounds.alertingSoundChance = unitSO.sounds.alertingSoundChance;
        sounds.alertedSoundChance = unitSO.sounds.alertedSoundChance;
        sounds.rangedSounds = unitSO.sounds.rangedSounds;
        sounds.rangedVolume = unitSO.sounds.rangedVolume;
        sounds.rangedSoundChance = unitSO.sounds.rangedSoundChance;
        sounds.specialSounds = unitSO.sounds.specialSounds;
        sounds.specialVolume = unitSO.sounds.specialVolume;
        sounds.specialSoundChance = unitSO.sounds.specialSoundChance;
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
                $"This should not be possible, if the unit is spawned by the master. " +
                $"If this unit was not spawned by the Master, then ignore this.");
            return;
        }

        //Stats
        SetStats();

        //Pathfinding
        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
        controller = GetComponent<CharacterController>();
        ai = GetComponent<AIPath>();
        ai.maxSpeed = movementSpeed;        

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
        //Assign a random appearance to each of the body parts
        select.bodyPartsRenderers = new SkinnedMeshRenderer[4];

        //Body --- INDEX : 0
        if (unitSO.unitAppearanceVariations.bodyVariations.Length != 0 && bodyRenderer != null)
        {
            Mesh newAppearance = unitSO.unitAppearanceVariations.bodyVariations[
                Random.Range(0, unitSO.unitAppearanceVariations.bodyVariations.Length)];

            bodyRenderer.sharedMesh = newAppearance;
            select.bodyPartsRenderers[0] = bodyRenderer;
        }

        //Head --- INDEX : 1
        if (unitSO.unitAppearanceVariations.headVariations.Length != 0 && headRenderer != null)
        {
            Mesh newAppearance = unitSO.unitAppearanceVariations.headVariations[
                Random.Range(0, unitSO.unitAppearanceVariations.headVariations.Length)];

            headRenderer.sharedMesh = newAppearance;
            select.bodyPartsRenderers[1] = headRenderer;
        }

        //Left Arm --- INDEX : 2
        if (unitSO.unitAppearanceVariations.leftArmVariations.Length != 0 && leftArmRenderer != null)
        {
            Mesh newAppearance = unitSO.unitAppearanceVariations.leftArmVariations[
                Random.Range(0, unitSO.unitAppearanceVariations.leftArmVariations.Length)];

            leftArmRenderer.sharedMesh = newAppearance;
            select.bodyPartsRenderers[2] = leftArmRenderer;
        }

        //Right Arm --- INDEX : 3
        if (unitSO.unitAppearanceVariations.rightArmVariations.Length != 0 && rightArmRenderer != null)
        {
            Mesh newAppearance = unitSO.unitAppearanceVariations.rightArmVariations[
                Random.Range(0, unitSO.unitAppearanceVariations.rightArmVariations.Length)];

            rightArmRenderer.sharedMesh = newAppearance;
            select.bodyPartsRenderers[3] = rightArmRenderer;
        }

        bool randomMat = unitSO.unitMaterialVariations.Length != 0;
        select.unitMats = new Material[select.bodyPartsRenderers.Length];

        //Random Material to assign, if there are any
        Material newMat = randomMat ? unitSO.unitMaterialVariations[Random.Range(0, unitSO.unitMaterialVariations.Length)] : null;

        for (int i = 0; i < select.bodyPartsRenderers.Length; i++)
        {
            if (select.bodyPartsRenderers[i] == null) continue;
            SkinnedMeshRenderer unitRenderer = select.bodyPartsRenderers[i];

            unitRenderer.material = new Material(randomMat ? //If the unit has different materials to choose from
                newMat : //Assign a random material.
                unitRenderer.sharedMaterial //If not, use already assigned material.
                );

            select.unitMats[i] = unitRenderer.sharedMaterial;
        }
    }

    #endregion

    #region Movement

    #region Pathfinding

    bool repathDelay = false;
    private void Repath(bool lessRepaths = false, Vector3? commandLocation = null, bool masterOverride = false)
    {
        if (!canPathfind) return; //I can't pathfind, so...
        if (!masterOverride && !currentTarget) return; //If I have no target, then what am I pathing towards?...
        
        repathDelay = lessRepaths && !repathDelay;
        if (repathDelay) return; //If there's a repath delay, then don't repath.
        //Every second repath request passes through if there's a delay

        Vector3 destination = commandLocation != null ? (Vector3)commandLocation : currentTarget.position;

        /*
        //this bit of code ensures that AI try to avoid walls by 2 squares.
        //Currently bugged, they get stuck in walls.

        ABPath newpath = ABPath.Construct(transform.position, destination, null);

        newpath.traversalProvider = GridShapeTraversalProvider.SquareShape(5);

        //Calculate a new path for the unit.
        seeker.StartPath(newpath);
        */

        //Calculate a new path for the unit.
        seeker.StartPath(transform.position, destination);
    }

    private void OnPathComplete(Path p)
    {
        //Pool the path
        p.Claim(this);

        if (!p.error)
        {
            //Release the previous path from the pool
            if (path != null) path.Release(this);
            //Make the stored path the new path
            path = p;
        }
        else
        {
            if (path != null) path.Release(this);
        }
    }

    private void EnablePathfinding(bool enable = true, bool stopPath = false)
    {
        if (!enable && stopPath)
        {
            //Cancel the path
            seeker.StartPath(transform.position, transform.position);
        }

        canPathfind = enable;
    }

    #endregion

    protected void StopMovement()
    {
        if (stoppedMoving) return;

        EnablePathfinding(false, true);
        stoppedMoving = true;
    }
    protected void ResumeMovement()
    {
        if (stoppedMoving)
        {
            EnablePathfinding();
            stoppedMoving = false;
        }
    }

    private IEnumerator MovementAnimationCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.2f);

            Walking = controller.velocity.magnitude > 0.1f;
        }
    }

    #endregion

    #region Sight

    private IEnumerator SearchCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.5f);

            if (Random.value < 0.05f) PlaySound(sounds.idleSounds, sounds.idleVolume, true);

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
            if (currentTarget)
            {
                if (!CloserThanTarget(newTarget)) return;
            }
        }
        SetTarget(newTarget);

        NewTarget();

        bool makeAlertSound;

        if (!alerted) //If I got alerted, I won't alert others.
        {
            makeAlertSound = Random.value < sounds.alertingSoundChance;
            Alert();
        }
        else
        {
            makeAlertSound = Random.value < sounds.alertedSoundChance;
        }

        if (makeAlertSound) PlaySound(sounds.alertSounds, sounds.alertVolume, true, true);
    }

    private void LoseTarget()
    {
        //Lose the target
        SetTarget(null);

        //Stop the chase coroutine (stop chasing the survivor)
        if (chasing) { StopCoroutine(CoChase); chasing = false; }
        if (attacking) { StopCoroutine(CoAttack); attacking = false; }

        //Start the search coroutine (start searching for survivors)
        if (!searching) { StartCoroutine(CoSearch); searching = true; }

        //Stop moving
        EnablePathfinding(false);

        print($"{name} : Mission failed, we'll get em next time");
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
        if (stoppedMoving) ResumeMovement();
        if (!canPathfind) EnablePathfinding();

        if (currentTarget) 
        {
            //Start chasing the new target
            chaseTime = unitSO.chaseTime;

            //Calculate a path towards them
            Repath();

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

            if (!currentTarget) LoseTarget();

            //Can I see the player?
            if (CanSee(currentTarget) || permanentTarget) //Permanent targets don't need to be in view
            {
                chaseTime = unitSO.chaseTime;
                Repath();

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
            }
            else
            {
                //If I can't see the target, my chase time goes down.
                //If I don't find the target before it runs out, I lose them.
                chaseTime -= 0.5f;
                //I will still repath to the player for 1/4th of the chase time duration.
                if (chaseTime > unitSO.chaseTime * 0.75f)
                {
                    Repath();
                }
                if (chaseTime <= 0)
                {
                    //If chase time runs out, and I still can't see my target. Stop chasing and lose the target.
                    LoseTarget();
                }
            }
        }
    }

    private async void Alert()
    {
        if (!canAlert) return;

        //Get every gameobject within a radius with the layers in alertMask (Unit layer)
        Collider[] adjacentUnits = Physics.OverlapSphere(transform.position, alertRadius, alertMask);

        //Iterate through each and tell them who my target is
        foreach (Collider col in adjacentUnits)
        {
            if (!col) continue;
            if (col.gameObject == gameObject) continue;
            await JODSTime.WaitTime(Random.Range(0, 0.1f));
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
        if (!currentTarget) return false;

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
        if (!currentTarget) return true;
        if (compareTarget == null) return false;

        float newTargetDistance = Vector3.Distance(transform.position, currentTarget.position);
        float currentTargetDistance = Vector3.Distance(transform.position, compareTarget.position);

        return currentTargetDistance > newTargetDistance;
    }

    protected bool NextToTarget() => Vector3.Distance(transform.position, currentTarget.position) <= Mathf.Clamp(.5f + 0.4f,melee.meleeRange - 0.1f,100);

    #endregion

    #region Attacks

    //This is the function that applies damage to the current target.
    protected void Damage(int damage) => currentTarget.GetComponent<IDamagable>()?.Svr_Damage(damage);
    //This function is an override that is called by projectiles or other.
    //Called when not necessarily damaging the current target
    protected void Damage(int damage, GameObject objectHit) => objectHit.GetComponent<IDamagable>()?.Svr_Damage(damage);

    private IEnumerator AttackCoroutine()
    {
        Transform myTarget = null;
        IDamagable damagable = null;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (!currentTarget) // I don't have a target, so what am I attacking?
            {
                LoseTarget();
                break;
            }
            if (currentTarget != myTarget) //Is my current target a new target?
            {
                myTarget = currentTarget; //If so, get the IDamageable component and store it as a variable so we don't have to get it each iteration
                if (currentTarget.TryGetComponent(out IDamagable idmg))
                {
                    damagable = idmg;
                }
            }
            if (damagable.IsDead)
            {
                LoseTarget();
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
            AttackMelee = true;
            LookAtTarget();
        }
        else Debug.Log($"{name} can't see it's target. Is it's ignoreOnLayer mask set properly?" +
            $"Or maybe the unit is too close/inside the target?");
    }
    protected bool CanMeleeAttack => WithinMeleeRange() && melee.canMelee;
    protected bool WithinMeleeRange()
    {
        if (!currentTarget) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= melee.meleeRange;
    }
    // Called by animation event.
    public virtual void MeleeAttack() 
    {
        if (!isServer) return;

        AttackMelee = false;
        if (stoppedMoving) ResumeMovement();

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
        if (!currentTarget) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= ranged.maxRange && distance >= ranged.minRange;
    }
    // Called by animation event.
    public virtual void RangedShoot()
    {
        if (!isServer) return;
        AttackRange = false;

        if (ranged.standStill) ResumeMovement();

        if (ranged.statusEffectToApply) ApplyStatusEffect(ranged.statusEffectToApply, currentTarget, ranged.amount);

        //Apply the proper damage number
        Damage(ranged.rangedDamage);

        StartCoroutine(RangedCooldownCoroutine());
        Debug.LogError("Direct ranged damage not implemented");
    }
    int projectileSpawns = 1;
    
    public virtual void SpawnProjectile()
    {
        if (!isServer) return;
        if (!currentTarget) return;
        if (ranged.standStill) ResumeMovement();
        if (ranged.directRangedAttack)
        {
            Debug.LogError($"Something went wrong here... {name} wanted to do direct ranged damage, but tried to spawn a projectile instead." +
                $" Is the correct method set in the animation event?");
            return;
        }


        if (projectileSpawns == 0) return; //This bool ensures that only 1 projectile spawns during each attack
        //This wouldn't be necessary if the server wasn't delayed by like 0.001s....

        //Spawn the projectile
        GameObject projectile = Instantiate(ranged.TEMPProjectilePrefab, transform.TransformPoint(ranged.projectileSpawnLocation), Quaternion.identity);
        //GameObject projectile = ObjectPool.Instance.SpawnFromNetworkedPool(ranged.projectileTag, transform.TransformPoint(ranged.projectileSpawnLocation), Quaternion.identity);

        //Aim the projectile at the current target.
        projectile.transform.LookAt(new Vector3(currentTarget.position.x, projectile.transform.position.y, currentTarget.position.z));

        //Set the speed of the projectile
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        projectileRB.velocity = projectile.transform.forward * ranged.projectileSpeed;

        projectile.transform.SetParent(null);

        Projectile uProjectile = projectile.GetComponent<Projectile>();
        uProjectile.damage = ranged.rangedDamage;
        uProjectile.statusEffectsToApply.Add(ranged.statusEffectToApply);
        uProjectile.amount = ranged.amount;

        //TEMPORARY
        NetworkServer.Spawn(projectile);

        AttackRange = false; //Disables this bool, allowing the unit to do another ranged attack.
        StartCoroutine(RangedCooldownCoroutine()); //Start cooldown
    }
    
    protected bool AtPreferredDistance()
    {
        if (!currentTarget) return false;
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
    protected bool CanSpecialAttack => WithinSpecialTriggerDistance() && special.canSpecial;
    protected bool WithinSpecialTriggerDistance()
    {
        if (!currentTarget) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= special.specialTriggerRange;
    }
    protected bool WithinSpecialDistance()
    {
        if (!currentTarget) return false;
        float distance = Vector3.Distance(currentTarget.position, transform.position);
        return distance <= special.specialRange;
    }
    //This is the actual special, which is usually called from an animation event.
    public virtual void SpecialAttack()
    {
        if (!isServer) return;
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
        target.GetComponent<StatusEffectManager>()?.Svr_ApplyStatusEffect(effect.ApplyEffect(target.gameObject), amount);
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

            EnablePathfinding(false, true); //Stop the current path
            ai.enabled = false; //Disable the AIPath component

            Svr_DisableCollider();

            Svr_SetDeathAnimation();

            Svr_PostDeath();
        }
    }

    #region Dismemberment

    public void Dismember_BodyPart(int bodyPartIndex, int damageType)
    {
        Rpc_Dismember_BodyPart(bodyPartIndex, damageType);
    }
    [ClientRpc]
    public void Rpc_Dismember_BodyPart(int bodyPartIndex, int damageType)
    {
        switch (bodyPartIndex)
        {
            //Head
            case 1:
                head.OnDetach(damageType);
                break;

            //Left Arm
            case 2:
                if ((DamageTypes)damageType != DamageTypes.Pierce) { 
                    leftArm.OnDetach(damageType);
                }
                break;

            //Right Arm
            case 3:
                if ((DamageTypes)damageType != DamageTypes.Pierce)
                {
                    rightArm.OnDetach(damageType);
                }
                break;
        }
    }

    #endregion

    #region Post Death
    [Server]
    private void Svr_SetDeathAnimation()
    {
        if (canRagdoll)
        {
            animator.enabled = false;
            Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
            foreach(Rigidbody rb in rbs)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            Rpc_Ragdoll();
        }
        else
        {
            animator.SetTrigger("Die");
            //Activate Ragdoll effect, or death animation
            Rpc_SetDeathAnimation();
        }
    }
    [ClientRpc]
    private void Rpc_SetDeathAnimation()
    {
        //Activate Ragdoll effect, or death animation
        animator.SetTrigger("Die");
    }
    [ClientRpc]
    private void Rpc_Ragdoll()
    {
        animator.enabled = false;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
    [Server]
    private void Svr_DisableCollider()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider c in colliders)
        {
            c.gameObject.layer = 30;
            //c.enabled = false;
        }
        Rpc_DisableCollider();
    }
    [ClientRpc]
    private void Rpc_DisableCollider()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.gameObject.layer = 30;
            //c.enabled = false;
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

        //Dissolve Effect
        //Start the DissolveCoroutine which slowly dissolves over a set amount of time.

        GetComponent<Timer>()?.StartTimer(true, 2.5f, 3f, select.unitMats);

        yield return new WaitForSeconds(5.5f);
        //After 3 seconds, tell the server to destroy the object/unit
        if(isServer)
            Svr_Destroy();
    }
    [Server]
    private void Svr_Destroy()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    public Teams Team => Teams.Unit;

    #endregion

    #region Sounds

    public async void PlaySound(AudioClip[] clips, float volume, bool atHead, bool randomDelay = false, bool useBasePitch = true)
    {
        if (randomDelay)
        {
            await JODSTime.WaitTime(Random.value);
        }

        if (clips.Length == 0)
        {
            //Debug.LogWarning($"{name} could not play an audioclip. The clip array is empty.");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        audioSource.volume = volume;

        audioSource.pitch = (useBasePitch ? sounds.basePitch : 1) * Random.Range(0.9f, 1.1f);

        Vector3 sourcePos = atHead ? 
            new Vector3(transform.position.x, transform.position.y + sounds.headHeight, transform.position.z) :
            transform.position;


        AudioSource.PlayClipAtPoint(clip, sourcePos, volume);
    }

    //This function is usually called by animation events. Hence 0 references
    public void Footstep()
    {
        //          The sound clips         |The sound volume       | play at the head? | use base pitch?
        PlaySound(sounds.footstepSounds,    sounds.footstepVolume,  false,              false);
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

    [Server]
    public void Svr_MoveToLocation(Vector3 pos)
    {
        //If the unit meets the requirements to be commanded to move to a new location
        //Requirements : (Not Chasing)

        if (currentTarget) return;
        if (chasing) return;

        Repath(false, pos, true);
    }

    #endregion

    #region Refunding

    //Meet the requirements to refund (Is max health)
    public int Refund => IsMaxHealth ? refundAmount : 0;

    #endregion

    #endregion

    #region Animation

    [ClientRpc]
    private void Rpc_PlayAttackAnimation(AttackAnimation anim, bool value)
    {
        switch (anim)
        {
            case AttackAnimation.Melee:
                animator.SetBool("Melee", value);
                break;
                
            case AttackAnimation.Ranged:
                animator.SetBool("Ranged", value);
                break;
                
            case AttackAnimation.Special:
                animator.SetBool("Special", value);
                break;
        }
    }

    #endregion

    #region Other
    bool tryingToLookAtTarget = false;
    private void LookAtTarget()
    {
        if (!currentTarget) return;
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

    #region IDamagable

    [Server]
    public void Svr_Damage(int damage, Transform target = null)
    {
        if (isDead) return;
        if (target != null)
        {
            if (currentTarget)
            {
                if (CloserThanTarget(target))
                {
                    Debug.Log("I got shot by someone closer than my target, going after them instead.");
                    AcquireTarget(target, false, true);
                }
            }
            else
            {
                AcquireTarget(target, false, false);
            }
        }

        Health -= damage;

        if (IsDead)
        {
            if (!target)
            {
                return;
            }

            GamemodeBase.Instance.Svr_ModifyStat(target.GetComponent<NetworkIdentity>().netId, 10, PlayerDataStat.Points);
            GamemodeBase.Instance.Svr_ModifyStat(target.GetComponent<NetworkIdentity>().netId, 1, PlayerDataStat.Kills);
        }

        animator.SetTrigger("Hit");
    }

    public void Cmd_Damage(int damage)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        if (isMelee)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.forward * melee.meleeRange);
        }
        if (isRanged)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.75f, transform.position.z), transform.forward * ranged.maxRange);
        }
        if (hasSpecial)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), transform.forward * special.specialTriggerRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z), transform.forward * special.specialRange);
        }
    }

    private void OnDrawGizmos()
    {
        //Has a target  =   Green
        //Is searching  =   Yellow
        //Is chasing    =   Red
        //Is Attacking  =   Blue

        if (currentTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(currentTarget.position.x, currentTarget.position.y + 4, currentTarget.position.z), 0.6f);
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

    #region Debug

    [Header("Debug")]
    [SerializeField] private bool stopMovement = false;
    [SerializeField] private bool stopPathfinding = false;
    private void Update()
    {
        if (stopMovement)
        {
            StopMovement();
            stopMovement = false;
        }

        if (stopPathfinding)
        {
            EnablePathfinding(false);
            stopPathfinding = false;
        }
    }

    #endregion
}