using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorLevelManager : NetworkBehaviour
{
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

    private void LevelUp()
    {
        if (experience >= level * 100)
        {
            level++;
        }
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
            level++;
            experience = 0;
        }
    }

}
