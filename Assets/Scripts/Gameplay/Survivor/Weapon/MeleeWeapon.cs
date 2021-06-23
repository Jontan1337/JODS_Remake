using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class MeleeWeapon : EquipmentItem
{
    [Header("Settings")]
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Weapon stats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private DamageTypes damageType = DamageTypes.Slash;
    [SerializeField] private float attackInterval;

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

    [SyncVar] private bool isAttacking;
    [SyncVar] private bool canAttack = true;

    private Coroutine COSplatterShader;
    private Coroutine COAttackInterval;

    private Transform previousHitColliderParent;

    private NetworkAnimator networkAnimator;

    private const string AttackTrigger = "Attack";
    private const string BloodAmount = "_BloodAmount";

    public Collider testCol;

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
    }

    protected override void OnDropPerformed(InputAction.CallbackContext obj)
    {
        weaponAnimator.enabled = false;
        base.OnDropPerformed(obj);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (!isAttacking) return;
        if (other.transform.root != previousHitColliderParent)
        {
            if (other.TryGetComponent(out IDamagable damagable))
            {
                previousHitColliderParent = other.transform.root;
                damagable?.Svr_Damage(damage);
            }
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
                if (other.TryGetComponent(out IDetachable detachable))
                {
                    detachable?.Detach(damageType);
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

    #region Server

    [Command]
    private void Cmd_Attack()
    {
        if (!canAttack) return;
        // Play the melee attack animation.
        networkAnimator.SetTrigger(AttackTrigger);
        //weaponAnimator.SetTrigger(AttackTrigger);
        COAttackInterval = StartCoroutine(IEAttackInterval());
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
        GameObject pooledParticleObject = ObjectPool.Instance.SpawnFromPool("Blood Splatter", objectPos, transform.rotation);
        ParticleSystem pooledParticle = pooledParticleObject.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainMod = pooledParticle.main;
        mainMod.startColor = color;
        ObjectPool.Instance.ReturnToPool("Blood Splatter", pooledParticleObject, pooledParticle.main.duration);
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

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (testCol != null)
        {
            Vector3 vec3 = new Vector3(transform.position.x, transform.position.y, transform.position.z + col.bounds.size.z / 1.5f);
            Debug.DrawLine(vec3, testCol.ClosestPoint(vec3));
        }
    }
}
