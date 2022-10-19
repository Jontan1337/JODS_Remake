using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deployable", menuName = "Unit Master/New Deployable", order = 2)]
public class DeployableSO : ScriptableObject
{
    [Header("Deployable Settings")]
    public GameObject deployablePrefab;
    [Space]
    public int pointsToUnlock = 100;
    [Space]
    public int cooldownOnUse = 60;

    [Header("Details")]
    public new string name;
    [TextArea(1, 5)] public string description = "This is a deployable";
    public Sprite deployableSprite;
}
