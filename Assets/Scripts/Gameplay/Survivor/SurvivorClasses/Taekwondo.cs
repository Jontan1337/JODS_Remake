using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RootMotion.FinalIK;

public class Taekwondo : Survivor, IHitter
{
    private CharacterController cController;
    private SurvivorController sController;
    private SurvivorClassStatManager sClass;
    private PlayerEquipment playerEquipment;
    ModifierManagerSurvivor modifiers;

    [SerializeField, SyncVar] private float flyingKickStart = 0;
    [SerializeField, SyncVar] private float flyingKickEnd = 100;
    [SerializeField, SyncVar] private float flyingKickSpeed = 1.5f;
    [SerializeField, SyncVar] private int flyingKickDamage = 100;
    [SerializeField, SyncVar] private int kickDamage = 50;

    [SerializeField] private Collider lowerLeg;
    [SyncVar] private bool flyingKick = false;
    [SyncVar] private bool kicking = false;


    public List<Collider> unitsHit = new List<Collider>();

    #region Serialization

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteBoolean(flyingKick);
            return true;
        }
        else
        {
            writer.WriteSingle(flyingKickStart);
            writer.WriteSingle(flyingKickEnd);
            writer.WriteSingle(flyingKickSpeed);


            writer.WriteBoolean(flyingKick);
            return true;
        }
    }
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            flyingKick = reader.ReadBoolean();
        }
        else
        {
            flyingKickStart = reader.ReadSingle();
            flyingKickEnd = reader.ReadSingle();
            flyingKickSpeed = reader.ReadSingle();


            flyingKick = reader.ReadBoolean();
        }
    }
    #endregion

    private void OnTransformParentChanged()
    {
        if (hasAuthority || isServer)
        {
            cController = GetComponentInParent<CharacterController>();
            sController = GetComponentInParent<SurvivorController>();
            modifiers = transform.root.GetComponent<ModifierManagerSurvivor>();
            playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();
            sClass = GetComponentInParent<SurvivorClassStatManager>();
            lowerLeg = transform.parent.Find("Armature").GetComponentInChildren<Collider>();

        }
    }

    public override void ActiveAbility()
    {
        if (CanFlyKick())
        {
            StartCoroutine(FlyingKick());
            sClass.Cmd_StartAbilityCooldown(transform.root);
        }
        else if (!kicking && sController.isGrounded && !playerEquipment?.ItemInHands)
        {
            StartCoroutine(Kick());
        }
    }
    public override void ActiveAbilitySecondary()
    {
        if (!kicking && sController.isGrounded && !playerEquipment?.ItemInHands)
        {
            StartCoroutine(Kick());
        }
    }

    #region FlyingKick

    private IEnumerator FlyingKick()
    {
        FlyingKickStart();
        while (flyingKickStart < flyingKickEnd)
        {
            flyingKickStart += Time.deltaTime;

            MoveForward();
            yield return null;
        };
        FlyingKickEnd();

    }

    private void MoveForward()
    {
        cController.Move(transform.forward * flyingKickSpeed * Time.deltaTime);
    }

    private void FlyingKickStart()
    {
        unitsHit.Clear();
        sController.gravity = 0;
        flyingKick = true;

        JODSInput.DisableMovement();
        JODSInput.DisableCamera();
    }
    private void FlyingKickEnd()
    {
        JODSInput.EnableCamera();
        JODSInput.EnableMovement();
        sController.gravity = 20;

        flyingKickStart = 0;
        flyingKick = false;

        foreach (Collider item in unitsHit)
        {
            if (item)
            {
                Physics.IgnoreCollision(item, cController, false);
            }
        }
    }

    private bool CanFlyKick()
    {
        return !sController.isGrounded && sController.IsMoving();
    }

    public void OnFlyingKickHit(ControllerColliderHit hit)
    {
        if (!hasAuthority) return;

        if (hit.gameObject.layer == 9 && flyingKick)
        {
            unitsHit.Add(hit.collider);
            Physics.IgnoreCollision(hit.collider, cController);
            Cmd_OnHit(hit.transform, Mathf.RoundToInt(abilityDamage * modifiers.data.AbilityDamage));
        }
        else if (hit.gameObject.layer == 0 && flyingKick) flyingKickStart = flyingKickEnd;
    }

    #endregion

    #region Kick

    public void OnKickHit(Collider hit)
    {
        if (!hasAuthority) return;
        if (hit.gameObject.layer == 9 && kicking || hit.gameObject.layer == 10 && kicking)
        {
            Cmd_OnHit(hit.transform.root, Mathf.RoundToInt((abilityDamage / 2) * modifiers.data.AbilityDamage));
            print(Mathf.RoundToInt((abilityDamage / 2) * modifiers.data.AbilityDamage));
        }
    }

    private IEnumerator Kick()
    {
        unitsHit.Clear();
        kicking = true;
        Kick(kicking);
        float speedModifier = modifiers.data.MovementSpeed / 2;
        modifiers.data.MovementSpeed -= speedModifier;
        GetComponentInParent<FullBodyBipedIK>().enabled = false;
        lowerLeg.enabled = true;

        yield return new WaitForSeconds(0.3f);

        kicking = false;
        modifiers.data.MovementSpeed += speedModifier;

        Kick(kicking);
        GetComponentInParent<FullBodyBipedIK>().enabled = true;
        lowerLeg.enabled = false;


    }

    private void Kick(bool kicking)
    {
        GetComponentInParent<SurvivorAnimationManager>().characerAnimator.SetBool("Kicking", kicking);
    }

    #endregion


    [Command]
    private void Cmd_OnHit(Transform hitObject, int dmg)
    {
        if (hitObject.TryGetComponent(out IDamagable damagable))
        {
            BaseStatManager statManagerBase = hitObject.GetComponent<BaseStatManager>(); 

            statManagerBase.onDied.AddListener(delegate { ApplyForce(hitObject); });

            damagable.Svr_Damage(dmg, transform.root);

            statManagerBase.onDied.RemoveListener(delegate { ApplyForce(hitObject); });
        }
    }

    private void ApplyForce(Transform hitObject)
    {
        Rigidbody[] rbColliderList = hitObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbColliderList)
        {
            rb.AddForce(transform.forward * 10, ForceMode.Impulse);
        }
    }
}