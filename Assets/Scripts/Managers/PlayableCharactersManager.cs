using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharactersManager : MonoBehaviour
{
    #region Singleton

    public static PlayableCharactersManager instance;
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    #endregion

    [Header("Playable Characters Lists")]
    [SerializeField] private List<SurvivorSO> survivorSOList = new List<SurvivorSO>();
    [Space]
    [SerializeField] private List<UnitMasterSO> masterSOList = new List<UnitMasterSO>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public List<SurvivorSO> GetAllSurvivors()
    {
        return survivorSOList;
    }

    public List<UnitMasterSO> GetAllMasters()
    {
        return masterSOList;
    }
    public string GetMasterName(string master)
    {
        foreach(UnitMasterSO masterSO in masterSOList)
        {
            if (masterSO.masterName == master)
            {
                return masterSO.masterName;
            }
        }
        Debug.LogWarning($"No master SO with the name '{master}' could be found.");
        return null;
    }
}
