using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaekwondoClass : SurvivorClass
{
    Coroutine flyingKick;
    CharacterController cController;
    SurvivorController sController;
    LookController lController;

    public float flyingKickStart = 0;
    public float flyingKickEnd = 100;
    public float flightSpeed = 1.5f;
    public int flyingKickDamage = 100;
    public int flyingDistance = 100;
    bool flying = false;

    public List<Collider> unitsHit;


    private void Start()
    {
        cController = GetComponent<CharacterController>();
        sController = GetComponent<SurvivorController>();
        lController = GetComponent<LookController>();

    }
    public override void ActiveAbility()
    {
        if (!sController.isGrounded && sController.isSprinting && sController.IsMoving())
        {
            flyingKick = StartCoroutine(FlyingKick());
        }
    }

    private IEnumerator FlyingKick()
    {
        unitsHit = new List<Collider>();
        flying = true;
        sController.enabled = false;
        lController.DisableLook();
        while (flyingKickStart < flyingKickEnd)
        {
            flyingKickStart += 1;
            cController.Move(transform.forward * flyingDistance * Time.deltaTime);

            yield return new WaitForSeconds(1 / flightSpeed / flyingKickEnd);
        };

        lController.EnableLook();
        sController.enabled = true;

        flyingKickStart = 0;
        flying = false;

        foreach (Collider item in unitsHit)
        {
            if (item)
            {
                Physics.IgnoreCollision(item, cController, false);
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 9 && flying)
        {
            print(hit.gameObject.name);
            unitsHit.Add(hit.collider);
            Physics.IgnoreCollision(hit.collider, cController);
            hit.gameObject.GetComponent<IDamagable>()?.Svr_Damage(flyingKickDamage);
        }
        else if (hit.gameObject.layer == 0 && flying)
        {
            flyingKickStart = flyingKickEnd;
        }
    }
}
