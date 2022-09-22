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
        survivorModifiers.data.MovementSpeed *= 1 + levelUpModifiers.MovementSpeed;
        survivorModifiers.data.Healing *= 1 + levelUpModifiers.Healing;
        survivorModifiers.data.Damage *= 1 + levelUpModifiers.Damage;
        survivorModifiers.data.DamageResistance *= 1 + levelUpModifiers.DamageResistance;
        survivorModifiers.data.FireResistance *= 1 + levelUpModifiers.FireResistance;
        survivorModifiers.data.RangedDamage *= 1 + levelUpModifiers.RangedDamage;
        survivorModifiers.data.Cooldown *= 1 + levelUpModifiers.Cooldown;
        survivorModifiers.data.ReloadSpeed *= 1 + levelUpModifiers.ReloadSpeed;
        survivorModifiers.data.Accuracy *= 1 + levelUpModifiers.Accuracy;

        print("Level up");

        experience = 0;
    }

    public void GainExp(int exp)
    {
        if (level < 10)
        {
            experience += exp;
            if (experience >= level * 100)
            {
                Cmd_LevelUp();
            }
        }
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 150, 50, 20), "exp"))
        {
            GainExp(50);
        }
    }

}
