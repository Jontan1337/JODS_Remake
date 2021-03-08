using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class RangedWeapon : NetworkBehaviour, IInteractable, IEquippable, IBindable
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
    private int damage = 0;
    [SerializeField]
    private float range = 0f;
    [SerializeField]
    private float fireRate = 0f;
    [SerializeField]
    private int bulletsPerShot = 1;
    [SerializeField, SyncVar]
    private int currentAmmunition = 10;
    [SerializeField]
    private int maxCurrentAmmunition = 10;
    [SerializeField, SyncVar]
    private int extraAmmunition = 20;
    [SerializeField]
    private int maxExtraAmmunition = 20;

    [Header("Game details")]
    [SerializeField, SyncVar]
    private string player = "Player name";

    [Header("References")]
    [SerializeField]
    private Animator weaponAnimator = null;
    [SerializeField]
    private Transform bulletRayOrigin = null;

    [SerializeField, SyncVar]
    private bool isInteractable = true;

    public bool IsInteractable {
        get => isInteractable;
        set => isInteractable = value;
    }

    public string ObjectName => gameObject.name;

    public string Name => weaponName;
    public GameObject Item => gameObject;
    public EquipmentType EquipmentType => equipmentType;

    private void OnValidate()
    {
        fireRate = Mathf.Clamp(fireRate, 0.1f, 100f);
    }

    public void Bind()
    {
        JODSInput.Controls.Survivor.LMB.performed += OnShoot;
        JODSInput.Controls.Survivor.Reload.performed += OnReload;
    }
    public void UnBind()
    {
        JODSInput.Controls.Survivor.LMB.performed -= OnShoot;
        JODSInput.Controls.Survivor.Reload.performed -= OnReload;
    }

    private void OnShoot(InputAction.CallbackContext context) => Cmd_Shoot();
    private void OnReload(InputAction.CallbackContext context) => Cmd_Reload();

    [Command]
    private void Cmd_Shoot()
    {
        if (currentAmmunition == 0) return;

        Ray shootRay = new Ray(bulletRayOrigin.position, transform.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
        {
            rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);
        }
        currentAmmunition -= bulletsPerShot;
    }

    [Command]
    private void Cmd_Reload()
    {
        Debug.Log("Reload!", this);

        int currentAmmunition = this.currentAmmunition;
        int neededAmmunition = maxCurrentAmmunition % currentAmmunition;

        this.currentAmmunition += extraAmmunition < maxCurrentAmmunition 
                                ? neededAmmunition
                                : maxCurrentAmmunition - currentAmmunition;

        extraAmmunition -= neededAmmunition;

        Debug.Log(neededAmmunition);
        Debug.Log(extraAmmunition);
    }

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        if (!IsInteractable) return;

        // Equipment should be on a child object of the player.
        Equipment equipment = interacter.GetComponentInChildren<Equipment>();

        if (equipment != null)
        {
            Svr_GiveAuthority(interacter.GetComponent<NetworkIdentity>().connectionToClient);
            equipment?.Svr_Equip(gameObject, equipmentType);
            IsInteractable = false;
        }
        else
        {
            // This should not be possible, but just to be absolutely sure.
            Debug.LogWarning($"{interacter} does not have an Equipment component", this);
        }
    }

    [Server]
    public void Svr_GiveAuthority(NetworkConnection conn)
    {
        netIdentity.AssignClientAuthority(conn);
    }
    [Server]
    public void Svr_RemoveAuthority()
    {
        netIdentity.RemoveClientAuthority();
    }
}
