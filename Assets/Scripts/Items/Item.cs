using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Stats")]
    public Type type = Type.none;
    public string itemName;
    public bool stopsMovement;
    public bool isInstant;

    public GameObject itemPrefab;

    public enum Type
    {
        grenade,
        heal,
        placable,
        none
    }
}
