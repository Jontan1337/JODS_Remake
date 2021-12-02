using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Settings", menuName =  "New Map Settings")]
public class MapSettingsSO : ScriptableObject
{
    [Scene] public string gameplayScene;

    public Vector3[] spawnPoints;
}
