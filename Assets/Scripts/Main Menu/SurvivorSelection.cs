using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[System.Serializable]
public struct SurvivorsSelection{
    public string name;
    public int index;
    public GameObject selection;
}
public class SurvivorSelection : MonoBehaviour
{
    #region Singleton
    public static SurvivorSelection instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region Survivor Selection
    [SerializeField]
    private List<SurvivorsSelection> survivors = new List<SurvivorsSelection>();

    public SurvivorSelect[] availableSurvivors;

    public void LoadSelection()
    {
        int value = MasterSelection.instance.GetMaster;

        Debug.Log("Loading selection with index " + value);

        bool selectionLoaded = false;
        foreach (var survivor in survivors)
        {
            if (survivor.index == value)
            {
                survivor.selection.SetActive(true);
                selectionLoaded = true;

                availableSurvivors = survivor.selection.transform.GetComponentsInChildren<SurvivorSelect>();
            }
            else { survivor.selection.SetActive(false); }
        }
        if (!selectionLoaded)
        {
            Debug.LogWarning($"No selection with index: ({value}) could be found. Make sure the indexes match.");
        }
    }

    [Server]
    public void Svr_OnPlayerDisconnect(uint index)
    {
        foreach(SurvivorSelect select in availableSurvivors)
        {
            if (select.playerIndex == (int)index)
            {
                select.Select(false,false);
            }
        }
    }
}
    #endregion