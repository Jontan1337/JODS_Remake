using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorLevelManager : NetworkBehaviour
{
    private ModifierManagerSurvivor modifiers;

    [SyncVar, SerializeField] private int level = 1;
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

    public override void OnStartAuthority()
    {
        
    }

    [Command]
    private void Cmd_LevelUp()
    {
        modifiers = GetComponentInParent<ModifierManagerSurvivor>();
        level++;
        modifiers.data.MovementSpeed += 0.05f;
        experience = 0;
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 150, 50, 20), "exp"))
        {
            GainExp(50);
        }
    }

    public void GainExp(int exp)
    {
        experience += exp;
        if (experience >= level * 100)
        {
            Cmd_LevelUp();            
        }
    }

}
