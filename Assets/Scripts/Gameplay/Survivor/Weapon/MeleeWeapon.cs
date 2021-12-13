using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Animator), typeof(NetworkAnimator), typeof(SFXPlayer))]
public class MeleeWeapon : EquipmentItem, IImpacter
{
    [Header("Settings")]
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Weapon stats")]
    [SerializeField] private int currentDamage = 10;
    [SerializeField] private int normalDamage = 10;
    [SerializeField] private int currentPunchthrough = 2;
    [SerializeField] private int normalPunchthrough = 2;
    [SerializeField] private DamageTypes damageType = DamageTypes.Slash;
    [SerializeField] private float attackInterval = 0.1f;
    [SerializeField] private bool chargeable = false;

    [Header("Game details")]
    [SerializeField] private float splatterAmount = 0f;
    [SerializeField, Range(0, 0.35f)] private float maxSplatterAmount = 0.35f;
    [SerializeField, Range(0, 0.35f)] private float splatterAmountOnHit = 0.04f;
    [SerializeField, Range(0f, 300f)] private float splatterRemoveAmount = 300f;
    [SerializeField, Range(-0.35f, 0f)] private float splatterRemoveAmountOnSwing = -0.01f;

    [Header("References")]
    [SerializeField] private Animator weaponAnimator = null;
    [SerializeField] private SFXPlayer sfxPlayer = null;
    [SerializeField] private Material material = null;
    [SerializeField] private Collider col = null;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip swingSound = null;
    [SerializeField] private AudioClip hitSound = null;

    [SyncVar] private bool isAttacking = false;
    [SyncVar] private bool canAttack = true;
    [SyncVar(hook = nameof(ToggleAnimator))] private bool animatorEnabled = false;

    private Coroutine COSplatterShader;
    private Coroutine COAttack;
    private Coroutine COAttackInterval;

    private int amountSlashed = 0;
    private bool hitOnSwing = false;

    private Transform previousHitColliderParent;

    private NetworkAnimator networkAnimator;

    public Action<float> OnImpact { get; set; }

    private const string AttackTrigger = "Attack";
    private const string AttackingBool = "Attacking";
    private const string BloodAmount = "_BloodAmount";

    public int Damage { get => currentDamage; }
    public DamageTypes DamageType { get => damageType; }
    public float SplatterAmount
    {
        get => splatterAmount;
        set
        {
            splatterAmount = Mathf.Clamp(value, 0, maxSplatterAmount);
            material.SetFloat(BloodAmount, splatterAmount);
        }
    }

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        networkAnimator = GetComponent<NetworkAnimator>();
        weaponAnimator.speed = attackInterval * 1f;
    }

    protected override void OnLMBPerformed(InputAction.CallbackContext context) => Cmd_StartAttack();
    protected override void OnLMBCanceled(InputAction.CallbackContext context) => Cmd_StopAttack();

    [Server]
    public override void Svr_Pickup(Transform newParent, NetworkConnection conn)
    {
        base.Svr_Pickup(newParent, conn);
        weaponAnimator.enabled = true;
        animatorEnabled = true;
    }

    [Server]
    public override void Svr_Drop()
    {
        animatorEnabled = false;
        weaponAnimator.enabled = animatorEnabled;
        base.Svr_Drop();
    }

    public override void Svr_Unequip()
    {
        base.Svr_Unequip();
        StopIEAttack();
        weaponAnimator.Play("Idle");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        // Wack camera shake animation played on local client.
        if (hasAuthority)
        {
            OnImpact?.Invoke(10);
        }

        if (isServer)
        {
            IDamagable damagable = null;
            // Damage the target root unless it's the same as previous root
            // (prevent multiple attacks on child colliders of same parent).
            if (other.transform.root != previousHitColliderParent)
            {
                if (other.TryGetComponent(out damagable))
                {
                    previousHitColliderParent = other.transform.root;
                    damagable?.Svr_Damage(currentDamage);
                    weaponAnimator.speed = 0.1f;
                }
                amountSlashed++;
            }
            if (other.TryGetComponent(out IParticleEffect particleEffect))
            {
                Vector3 weaponPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + col.bounds.size.z / 1.5f);
                Rpc_EmitParticle(other.ClosestPoint(weaponPos), particleEffect.ParticleColor);
                Rpc_ApplySplatter(splatterAmountOnHit);
            }
            switch (damageType)
            {
                case DamageTypes.Slash:
                    if (damagable != null)
                    {
                        if (other.TryGetComponent(out IDetachable detachable))
                        {
                            detachable.Detach((int)damageType);
                        }
                        if (amountSlashed == currentPunchthrough)
                        {
                            weaponAnimator.CrossFadeInFixedTime("Idle", 0.2f);
                            amountSlashed = 0;
                        }
                    }
                    break;
                case DamageTypes.Blunt:
                    weaponAnimator.CrossFadeInFixedTime("Idle", 0.2f);
                    break;
                case DamageTypes.Pierce:
                    break;
                default:
                    break;
            }
        }
    }

    #region Server

    // Called by animation even.
    // Using server should prevent clients
    // from calling this method and setting the damage.
    [ServerCallback]
    public void Svr_SetDamage(int damage)
    {
        currentDamage = damage;
    }
    // Called by animation even.
    // Using server should prevent clients
    // from calling this method and setting the damage.
    [ServerCallback]
    public void Svr_SetPunchthrough(int punchthrough)
    {
        currentPunchthrough = punchthrough;
    }

    [Command]
    private void Cmd_StartAttack()
    {
        // Play the melee attack animation.
        //networkAnimator.SetTrigger(AttackTrigger);
        StartIEAttack();

        //object arg1 = "NetworkConnection";
        //object arg2 = "Attack!";
        //NetworkTest.AddBuffer(this, nameof(Rpc_TestPrint), new object[] { arg1, arg2 });
    }

    private void StartIEAttack()
    {
        COAttack = StartCoroutine(IEAttack());
    }
    private void StopIEAttack()
    {
        if (COAttack != null)
        {
            StopCoroutine(COAttack);
            COAttack = null;
        }
        weaponAnimator.SetBool(AttackingBool, false);
    }

    [TargetRpc]
    private void Rpc_TestPrint(NetworkConnection target, string message)
    {
        print(message);
    }

    [Command]
    private void Cmd_StopAttack()
    {
        // Stop the melee attack animation.
        isAttacking = false;
        StopIEAttack();
    }

    // Called by animation event.
    public void StartOfAttack()
    {
        if (!hasAuthority) return;

        Cmd_StartOfAttack();
    }

    // Called by animation event.
    public void EndOfAttack()
    {
        if (!hasAuthority) return;

        Cmd_EndOfAttack();
    }

    [Command]
    private void Cmd_StartOfAttack()
    {
        Rpc_ApplySplatter(splatterRemoveAmountOnSwing);
        previousHitColliderParent = null;
        isAttacking = true;
    }

    [Command]
    private void Cmd_EndOfAttack()
    {
        amountSlashed = 0;
        currentDamage = normalDamage;
        currentPunchthrough = normalPunchthrough;
    }

    private IEnumerator IEAttack()
    {
        while (true)
        {
            if (canAttack)
            {
                //networkAnimator.SetTrigger(AttackTrigger);
                weaponAnimator.speed = attackInterval * 1f;
                weaponAnimator.SetBool(AttackingBool, true);
                StartAttackInterval();
            }
            else
            {
                weaponAnimator.SetBool(AttackingBool, false);
            }
            yield return null;
        }
    }

    private void StartAttackInterval()
    {
        COAttackInterval = StartCoroutine(IEAttackInterval());
    }

    private IEnumerator IEAttackInterval()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackInterval);
        canAttack = true;
    }

    #endregion

    #region Client

    [ClientRpc]
    private void Rpc_SwingSFX()
    {
        sfxPlayer.PlaySFX(swingSound);
    }
    [ClientRpc]
    private void Rpc_HitSFX()
    {
        sfxPlayer.PlaySFX(hitSound);
    }
    [ClientRpc]
    private void Rpc_EmitParticle(Vector3 objectPos, Color color)
    {
        GameObject pooledParticleObject = ObjectPool.Instance.SpawnFromLocalPool(Tags.MeleeBloodSplatter, objectPos, transform.rotation);
        ParticleSystem pooledParticle = pooledParticleObject.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainMod = pooledParticle.main;
        mainMod.startColor = color;
        ObjectPool.Instance.ReturnToLocalPool(Tags.MeleeBloodSplatter, pooledParticleObject, pooledParticle.main.duration);
    }

    private IEnumerator IESplatterShader()
    {
        while (splatterAmount > 0)
        {
            SplatterAmount -= Time.deltaTime / splatterRemoveAmount;
            yield return null;
        }
        COSplatterShader = null;
    }

    [ClientRpc]
    private void Rpc_ApplySplatter(float amount)
    {
        SplatterAmount += amount;
        if (COSplatterShader == null)
        {
            StartSplatterFading();
        }
    }
    private void StartSplatterFading()
    {
        COSplatterShader = StartCoroutine(IESplatterShader());
    }

    private void ToggleAnimator(bool oldValue, bool newValue)
    {
        weaponAnimator.enabled = newValue;
    }

    #endregion
}
