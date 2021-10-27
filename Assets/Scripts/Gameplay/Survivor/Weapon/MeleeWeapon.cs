using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class MeleeWeapon : EquipmentItem, IImpacter
{
    [Header("Settings")]
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Weapon stats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private DamageTypes damageType = DamageTypes.Slash;
    [SerializeField] private float attackInterval = 0.1f;
    [SerializeField] private int slashPower = 2;

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
    private Coroutine COAttackInterval;

    private int amountSlashed = 0;

    private Transform previousHitColliderParent;

    private NetworkAnimator networkAnimator;

    public Action<float> OnImpact { get; set; }

    private const string AttackTrigger = "Attack";
    private const string BloodAmount = "_BloodAmount";

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
    }

    protected override void OnLMBPerformed(InputAction.CallbackContext context) => Cmd_Attack();

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

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (!isAttacking) return;

        // Wack animation played on local client.
        if (isLocalPlayer)
        {
            OnImpact?.Invoke(10);
        }

        if (isServer)
        {
            IDamagable damagable = null;
            if (other.transform.root != previousHitColliderParent)
            {
                if (other.TryGetComponent(out damagable))
                {
                    previousHitColliderParent = other.transform.root;
                    damagable?.Svr_Damage(damage);
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
                        if (amountSlashed == slashPower)
                        {
                            weaponAnimator.CrossFadeInFixedTime("Idle", 0.1f);
                            amountSlashed = 0;
                        }
                    }
                    break;
                case DamageTypes.Blunt:
                    weaponAnimator.CrossFadeInFixedTime("Idle", 0.1f);
                    break;
                case DamageTypes.Pierce:
                    break;
                default:
                    break;
            }
        }

        // All clients
        //if (isClient)
        //{
        //    if (other.TryGetComponent(out IDamagable damagable))
        //    {
        //        if (other.TryGetComponent(out IDetachable detachable))
        //        {
        //            detachable.Detach(damageType);
        //        }
        //    }
        //}
    }

    #region Server

    [Command]
    private void Cmd_Attack()
    {
        if (!canAttack) return;
        amountSlashed = 0;
        // Play the melee attack animation.
        networkAnimator.SetTrigger(AttackTrigger);
        //weaponAnimator.SetTrigger(AttackTrigger);
        COAttackInterval = StartCoroutine(IEAttackInterval());

        //object arg1 = "NetworkConnection";
        //object arg2 = "Attack!";
        //NetworkTest.AddBuffer(this, nameof(Rpc_TestPrint), new object[] { arg1, arg2 });
    }

    [TargetRpc]
    private void Rpc_TestPrint(NetworkConnection target, string message)
    {
        print(message);
    }

    // Called by animation event.
    public void StartAttacking()
    {
        if (!hasAuthority) return;

        Cmd_StartAttacking();
    }
    // Called by animation event.
    public void StopAttacking()
    {
        if (!hasAuthority) return;

        Cmd_StopAttacking();
    }
    [Command]
    private void Cmd_StartAttacking()
    {
        isAttacking = true;
        Rpc_ApplySplatter(splatterRemoveAmountOnSwing);
        previousHitColliderParent = null;
    }
    [Command]
    private void Cmd_StopAttacking()
    {
        isAttacking = false;
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
