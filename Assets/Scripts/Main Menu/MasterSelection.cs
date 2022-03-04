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
    [SerializeField] private string currentMasterSOName = null;

    public void SetMasterName(string masterName) => currentMasterSOName = PlayableCharactersManager.Instance.GetMasterName(masterName);
   
    public void SetMasterIndex(int index) => currentMasterIndex = index;

    public string GetMasterName => currentMasterSOName;
    public int GetMasterIndex => currentMasterIndex;

}
