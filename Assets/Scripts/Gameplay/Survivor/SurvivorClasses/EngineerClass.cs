using UnityEngine;
using Mirror;
using System.Collections;

public class EngineerClass : SurvivorClass
{

    private PlayerEquipment playerEquipment;
    private GameObject turret;
    private SurvivorController sController;
    private ActiveSClass sClass;
    private ModifierManager modifierManager;

    private bool recharging = false;


    #region Serialization

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            return true;
        }
        else
        {
            return true;
        }
    }
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {

        }
        else
        {

        }
    }
    #endregion

    private void OnTransformParentChanged()
    {
        if (hasAuthority || isServer)
        {
            sController = GetComponentInParent<SurvivorController>();
            sClass = GetComponentInParent<ActiveSClass>();
            modifierManager = GetComponentInParent<ModifierManager>();
            RechargeCo = Recharge();
            StartCoroutine(RechargeCo);
        }
    }
    public override void ActiveAbility()
    {
        if (!turret)
        {
            Cmd_EquipTurret();
        }
    }

    IEnumerator RechargeCo;

    [SerializeField] private float range = 30;
    [SerializeField] private float idling = 0;
    [SerializeField] private LayerMask survivorLayer = 0;
    [SerializeField] private bool rechargeActive = false;
    [SerializeField] private Collider[] survivorsInRange;

    private IEnumerator Recharge()
    {
        while (true)
        {
            if (!sController.IsMoving())
            {
                StartRecharge();
            }
            else
            {
                StopRecharge();
            }
            yield return null;
        }
    }

    private void StartRecharge()
    {
        idling += Time.deltaTime;
        if (idling >= 2 && !rechargeActive)
        {
            survivorsInRange = Physics.OverlapSphere(transform.position, range, survivorLayer);
            foreach (var item in survivorsInRange)
            {
                item.GetComponentInParent<ModifierManager>().Cooldown += 1;
            }
            rechargeActive = true;
        }
    }
    private void StopRecharge()
    {
        idling = 0;
        if (rechargeActive)
        {
            foreach (var item in survivorsInRange)
            {
                item.GetComponentInParent<ModifierManager>().Cooldown -= 1;
            }
            survivorsInRange = null;
            rechargeActive = false;
        }
    }


    [Command]
    private void Cmd_EquipTurret()
    {
        if (!turret)
        {
            EquipTurret();
        }
    }

    [Server]
    private void EquipTurret()
    {
        turret = Instantiate(abilityObject, transform.position, transform.rotation);
        NetworkServer.Spawn(turret);
        playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

        turret.GetComponent<IInteractable>().Svr_Interact(transform.root.gameObject);
    }

}
