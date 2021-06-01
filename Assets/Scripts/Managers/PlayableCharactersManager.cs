using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharactersManager : MonoBehaviour
{
    #region Singleton

    public static PlayableCharactersManager instance;
    private void Awake()
    {
        instance = this;
    }

    #endregion

    [Header("Playable Characters Lists")]
    public List<SurvivorSO> survivorSOList = new List<SurvivorSO>();
    [Space]
    public List<MasterSO> masterSOList = new List<MasterSO>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
