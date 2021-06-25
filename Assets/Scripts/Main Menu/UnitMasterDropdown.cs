using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMasterDropdown : MonoBehaviour
{
    private List<UnitMasterSO> masters = new List<UnitMasterSO>();
    private List<string> masterNames = new List<string>();

    private Dropdown dropdown;

    private MasterSelection masterSelection;

    private void Start()
    {
        masterSelection = MasterSelection.instance;

        masters = PlayableCharactersManager.instance.GetAllMasters();

        foreach(UnitMasterSO masterSO in masters)
        {
            masterNames.Add(masterSO.masterName);
        }

        dropdown = GetComponent<Dropdown>();

        dropdown.ClearOptions();

        dropdown.AddOptions(masterNames);

        SetMaster();
    }

    public void SetMaster()
    {
        masterSelection.SetMasterName(dropdown.options[dropdown.value].text);
    }
}
