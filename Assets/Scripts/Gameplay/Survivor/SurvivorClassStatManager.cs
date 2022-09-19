using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorClassStatManager : NetworkBehaviour
{

    [SerializeField] private Image abilityCooldownUI = null;
    [SerializeField] private float abilityCooldown = 0;

    private bool abilityIsReady = true;
    private float abilityCooldownCount = 0;

    public bool AbilityIsReady
    {
        get { return abilityIsReady; }
        private set { abilityIsReady = value; }
    }

    public void SetStats(float abilityCooldown)
    {
        this.abilityCooldown = abilityCooldown;
    }


    public IEnumerator AbilityCooldown()
    {
        AbilityIsReady = false;
        abilityCooldownUI.fillAmount = 0;
        while (abilityCooldownCount < abilityCooldown)
        {

            abilityCooldownCount += (Time.deltaTime * GetComponent<ModifierManager>().Cooldown);
            abilityCooldownUI.fillAmount = abilityCooldownCount / abilityCooldown;
            yield return null;
        }
        abilityCooldownCount = 0;
        AbilityIsReady = true;
    }

    public void StartAbilityCooldownCo()
    {
        StartCoroutine(AbilityCooldown());
    }

    [TargetRpc]
    public void Rpc_StartAbilityCooldown(NetworkConnection conn, Transform owner)
    {
        owner.GetComponentInParent<SurvivorClassStatManager>()?.StartAbilityCooldownCo();
    }

    [Command]
    public void Cmd_StartAbilityCooldown(Transform owner)
    {
        Rpc_StartAbilityCooldown(GetComponent<NetworkIdentity>().connectionToClient, owner);
    }
}
