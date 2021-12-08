﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShopSpawnPoint
{
    public Vector3 position;
    public Quaternion rotation;
}
[CreateAssetMenu(fileName = "New Map Settings", menuName =  "New Map Settings")]
public class MapSettingsSO : ScriptableObject
{
    [Scene] public string gameplayScene;

    public Vector3[] spawnPoints;
    [Space]
    public ShopSpawnPoint[] shopSpawnPoints;
}
