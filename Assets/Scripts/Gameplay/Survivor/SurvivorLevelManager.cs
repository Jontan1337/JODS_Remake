using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorLevelManager : NetworkBehaviour
{
    private ModifierManagerSurvivor survivorModifiers;
    public ModifierManagerSurvivorData levelUpModifiers;

    [SyncVar, SerializeField] private int level = 0;
    public int Level
    {
        get { return level; }
        set { level = Mathf.Clamp(value, 0, 10); ; }
    }

    [SyncVar, SerializeField] int experience = 0;
    public int Experience
    {
        get { return experience; }
        set { experience = value; }
    }

    public void LevelUpModifiersSetup(ModifierManagerSurvivorData levelUpModifiersData)
    {
        levelUpModifiers = levelUpModifiersData;
    }

    [Command]
    private void Cmd_LevelUp()
    {
        survivorModifiers = GetComponentInParent<ModifierManagerSurvivor>();
        level++;

        survivorModifiers.data.MovementSpeed += levelUpModifiers.MovementSpeed;
        survivorModifiers.data.Healing += levelUpModifiers.Healing;
        survivorModifiers.data.DamageResistance += levelUpModifiers.DamageResistance;
        survivorModifiers.data.FireResistance += levelUpModifiers.FireResistance;
        survivorModifiers.data.RangedDamage += levelUpModifiers.RangedDamage;
        survivorModifiers.data.MeleeDamage += levelUpModifiers.MeleeDamage;
        survivorModifiers.data.AbilityDamage += levelUpModifiers.AbilityDamage;
        survivorModifiers.data.Cooldown += levelUpModifiers.Cooldown;
        survivorModifiers.data.ReloadSpeed += levelUpModifiers.ReloadSpeed;
        survivorModifiers.data.Accuracy += levelUpModifiers.Accuracy;

        print("Level up");

        experience = 0;
    }

    public void GainExp(int exp)
    {
        if (level < 10)
        {
            experience += exp;
            if (experience >= 100 + (level * 125))
            {
                Cmd_LevelUp();
            }
        }
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 150, 50, 20), "exp"))
        {
            GainExp(500);
        }
    }

}
