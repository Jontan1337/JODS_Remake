using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterSelection : MonoBehaviour
{
    #region Singleton
    public static MasterSelection instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private int currentMasterIndex = 0;

    public void SetMaster(int index) => currentMasterIndex = index;

    public int GetMaster => currentMasterIndex;

}
