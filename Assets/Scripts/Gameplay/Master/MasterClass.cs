using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Master Class", menuName = "Classes/Master/New Master Class", order = 1)]
public class MasterClass : ScriptableObject
{
    [Header("Necessities")]
    public string masterName = "Default Master";

    [Header("Units (In descending order)")]
    public SOUnit[] units;

    [Header("Sounds")]

    [Tooltip("This sound plays when the master spawns a unit.")]
    public AudioClip spawnSound;
    [Tooltip("This sound plays when the master either upgrades a unit, unlocks a unit or upgrades energy.")]
    public AudioClip globalSound;

    [Header("Visual")]
    public Color energyColor;
    public Color energyUseColor;
    [Space]
    public Color screenTintColor;

    [Header("Master Class Description")]
    [TextArea(10,20)]
    public string classDescription = "This master uses units";

    [Header("Master Class Special Description")]
    [TextArea(10,20)]
    public string classSpecialDescription = "This master can do special stuff";
}
