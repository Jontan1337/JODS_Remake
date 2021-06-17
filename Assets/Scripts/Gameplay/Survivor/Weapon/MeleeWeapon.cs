using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class MeleeWeapon : EquipmentItem
{
    [Header("Settings")]
    [SerializeField]
    private LayerMask ignoreLayer;

    [Header("Weapon stats")]
    [SerializeField]
    private int damage = 10;

    [Header("Game details")]
    [SerializeField, SyncVar]
    private string player = "Player name";

    [Header("References")]
    [SerializeField]
    private Animator weaponAnimator = null;
    [SerializeField]
    private AudioSource audioSource = null;
    [SerializeField]
    private SFXPlayer sfxPlayer = null;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip swingSound = null;
    [SerializeField]
    private AudioClip hitSound = null;
    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    [SyncVar] private bool isAttacking;

    private const string AttackTrigger = "Attack";

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
        other.TryGetComponent(out IDamagable damagable);
        damagable?.Svr_Damage(damage);
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
    }
    [Command]
    private void Cmd_StopAttacking()
    {
        isAttacking = false;
    }

    #endregion

    #region Client

    public void AttackAnimation()
    {
        //Rpc_SwingSFX();
        //Ray shootRay = new Ray(bulletRayOrigin.position, transform.forward);
        //RaycastHit rayHit;
        //if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
        //{
        //    rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);
        //}
    }

    [ClientRpc]
    private void Rpc_SwingSFX()
    {
        sfxPlayer.PlaySFX(swingSound);
        // Implement partyicle efect.
    }
    [ClientRpc]
    private void Rpc_HitSFX()
    {
        sfxPlayer.PlaySFX(hitSound);
        // Implement partyicle efect.
    }

    #endregion
}
