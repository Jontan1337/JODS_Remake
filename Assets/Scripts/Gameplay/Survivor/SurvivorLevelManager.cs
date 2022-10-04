using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorLevelManager : NetworkBehaviour
{
    [SerializeField] private int baseExpRequired = 100;

    private ModifierManagerSurvivorData modifiers;
    private ModifierManagerSurvivorData levelUpModifiers;
    private SurvivorPlayerData survivorPlayerData;

    public override void OnStartServer()
    {
        modifiers = GetComponent<ModifierManagerSurvivor>().data;
        levelUpModifiers = GetComponent<ActiveSurvivorClass>().survivorSO.levelUpModifiers;
        survivorPlayerData = GetComponent<SurvivorPlayerData>();
        survivorPlayerData.onLevelChanged += Svr_OnLevelChanged;
        survivorPlayerData.BaseExpRequired = baseExpRequired;
    }

    [Server]
    private void Svr_OnLevelChanged(int level)
    {
        // Add where modify based on level up or down.
        modifiers.MovementSpeed += levelUpModifiers.MovementSpeed;
        modifiers.Healing += levelUpModifiers.Healing;
        modifiers.DamageResistance += levelUpModifiers.DamageResistance;
        modifiers.FireResistance += levelUpModifiers.FireResistance;
        modifiers.RangedDamage += levelUpModifiers.RangedDamage;
        modifiers.MeleeDamage += levelUpModifiers.MeleeDamage;
        modifiers.AbilityDamage += levelUpModifiers.AbilityDamage;
        modifiers.Cooldown += levelUpModifiers.Cooldown;
        modifiers.ReloadSpeed += levelUpModifiers.ReloadSpeed;
        modifiers.Accuracy += levelUpModifiers.Accuracy;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 150, 50, 20), "exp"))
        {
            survivorPlayerData.Exp += 500;
        }
    }
}
