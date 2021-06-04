using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PhysicsToggler), typeof(Rigidbody), typeof(BoxCollider)),
 RequireComponent(typeof(AuthorityController), typeof(SFXPlayer))]
public class MeleeWeapon : NetworkBehaviour, IInteractable, IEquippable, IBindable
{
    [Header("Settings")]
    [SerializeField]
    private string weaponName = "Weapon name";
    [SerializeField]
    private EquipmentType equipmentType = EquipmentType.Weapon;
    [SerializeField]
    private LayerMask ignoreLayer;

    [Header("Weapon stats")]
    [SerializeField]
    private int damage = 10;
    [SerializeField]
    private float range = 1000f;

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
    [SerializeField]
    private AuthorityController authController = null;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip swingSound = null;
    [SerializeField]
    private AudioClip hitSound = null;
    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    [Space]
    [SerializeField, SyncVar]
    private bool isInteractable = true;

    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public string ObjectName => gameObject.name;
    public string Name => weaponName;
    public GameObject Item => gameObject;
    public EquipmentType EquipmentType => equipmentType;

    private void OnValidate()
    {

    }

    public void Bind()
    {
        JODSInput.Controls.Survivor.LMB.performed += OnAttack;
        //JODSInput.Controls.Survivor.LMB.canceled += OnStopShoot;
        //JODSInput.Controls.Survivor.Reload.performed += OnReload;
        //JODSInput.Controls.Survivor.Changefiremode.performed += OnChangeFireMode;
    }
    public void Unbind()
    {
        JODSInput.Controls.Survivor.LMB.performed -= OnAttack;
        //JODSInput.Controls.Survivor.LMB.canceled -= OnStopShoot;
        //JODSInput.Controls.Survivor.Reload.performed -= OnReload;
        //JODSInput.Controls.Survivor.Changefiremode.performed -= OnChangeFireMode;
    }

    private void OnAttack(InputAction.CallbackContext context) => Cmd_Attack();
    //private void OnStopShoot(InputAction.CallbackContext context) => Cmd_StopShoot();
    //private void OnReload(InputAction.CallbackContext context) => Cmd_Reload();
    //private void OnChangeFireMode(InputAction.CallbackContext context) => Cmd_ChangeFireMode();

    #region Server

    [Command]
    private void Cmd_Attack()
    {
        // Play the melee attack animation.
    }

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        if (!IsInteractable) return;

        // Equipment should be on a child object of the player.
        PlayerEquipment equipment = interacter.GetComponentInChildren<PlayerEquipment>();

        if (equipment != null)
        {
            authController.Svr_GiveAuthority(interacter.GetComponent<NetworkIdentity>().connectionToClient);
            equipment?.Svr_Equip(gameObject, equipmentType);
            IsInteractable = false;
        }
        else
        {
            // This should not be possible, but just to be absolutely sure.
            Debug.LogWarning($"{interacter} does not have an Equipment component", this);
        }
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
