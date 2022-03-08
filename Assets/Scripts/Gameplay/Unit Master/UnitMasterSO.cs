using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Master Class", menuName = "Classes/Master/New Unit Master Class", order = 1)]
public class UnitMasterSO : ScriptableObject
{
    [Header("Necessities")]
    public string masterName = "Default Master";
    public Object masterClass;
    public MapSettingsSO associatedMapSettings;

    [Header("Stats")]
    public int startEnergy = 25;
    public int startMaxEnergy = 100;
    public int energyRechargeIncrement = 1;
    public int maxEnergyUpgradeIncrement = 20;
    public int energyUpgradeInterval = 100;

    [Header("Units (In descending order)")]
    public UnitSO[] units;
    
    [Header("Deployables (In descending order)")]
    public DeployableSO[] deployables;

    [Header("Sounds")]

    [Tooltip("This sound plays when the master spawns a unit.")]
    public AudioClip spawnSound;
    [Tooltip("This sound plays when the master either upgrades a unit, unlocks a unit or upgrades energy.")]
    public AudioClip globalSound;

    [Header("Visual")]
    public Color energyColor;
    public Color energyUseColor;
    [Space]
    public Color unitSelectColor;
    [Space]
    public Color screenTintColor;
    public Color topdownLightColor;
    [Space]
    public Mesh selectPositionMarkerMesh;
    public Color selectPositionMarkerColor;
    [Space]
    public Mesh markerMesh;
    public Color markerColor;

    [Header("Master Class Description")]
    [TextArea(10,20)]
    public string classDescription = "This master uses units";

    [Header("Master Class Special Description")]
    [TextArea(10,20)]
    public string classSpecialDescription = "This master can do special stuff";
}
