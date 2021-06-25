using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldVisual
{
    public string name;
    public int index;
    public GameObject visual;
}

public class MainMenuVisuals : MonoBehaviour
{
    [SerializeField] private List<WorldVisual> worldVisuals = new List<WorldVisual>();

    public void LoadVisual()
    {
        int value = MasterSelection.instance.GetMasterIndex;

        Debug.Log("Loading world visuals with index " + value);

        bool worldLoaded = false;
        foreach(var world in worldVisuals)
        {
            if(world.index == value)
            {
                world.visual.SetActive(true);
                worldLoaded = true;
            }
            else { world.visual.SetActive(false); }
        }
        if (!worldLoaded)
        {
            Debug.LogWarning($"No world with index: ({value}) could be found. Make sure the indexes match.");
        }
    }
}
