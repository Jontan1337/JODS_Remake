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

    [Header("Other")]
    public Color energyColor; //lol
}
