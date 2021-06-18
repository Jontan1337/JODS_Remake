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

    [Header("Game details")]
    [SerializeField] private float splatterAmount = 0f;
    [SerializeField, Range(0, 0.35f)] private float maxSplatterAmount = 0.35f;
    [SerializeField, Range(0, 0.35f)] private float splatterAmountOnHit = 0.04f;
    [SerializeField, Range(0f, 300f)] private float splatterRemoveAmount = 300f;
    [SerializeField, Range(-0.35f, 0f)] private float splatterRemoveAmountOnSwing = -0.01f;

    [Header("References")]
    [SerializeField] private Animator weaponAnimator = null;
    [SerializeField] private SFXPlayer sfxPlayer = null;
    [SerializeField] private ParticleSystem hitParticle = null;
    [SerializeField] private Material material = null;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip swingSound = null;
    [SerializeField] private AudioClip hitSound = null;

    [SyncVar] private bool isAttacking;

    private Coroutine COSplatterShader;

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
        if (other.TryGetComponent(out IDamagable damagable))
        {
            damagable?.Svr_Damage(damage);

            if (other.TryGetComponent(out IParticleEffect particleEffect))
            {
                Rpc_ParticleColor(particleEffect.ParticleColor);
                Rpc_EmitParticles();
                Rpc_ApplySplatter(splatterAmountOnHit);
            }
        }
    }

    #region Server

    [Command]
    private void Cmd_Attack()
    {
        // Play the melee attack animation.
        weaponAnimator.SetTrigger(AttackTrigger);
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
    }
    [Command]
    private void Cmd_StopAttacking()
    {
        isAttacking = false;
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
    private void Rpc_EmitParticles()
    {
        hitParticle.Emit(20);
    }
    [ClientRpc]
    private void Rpc_ParticleColor(Color color)
    {
        ParticleSystem.MainModule mainMod = hitParticle.main;
        mainMod.startColor = color;
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
}
