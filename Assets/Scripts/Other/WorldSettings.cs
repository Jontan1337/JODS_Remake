using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : MonoBehaviour
{
    #region Singleton
    public static WorldSettings instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log(name + ": Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    #endregion

    #region World Settings

    [Header("World Settings")]
    public int amountOfFloors = 1;

    #endregion
}
