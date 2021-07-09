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
    public static MainMenuVisuals instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private GameObject defaultWorld = null;
    [Space]
    [SerializeField] private List<WorldVisual> worldVisuals = new List<WorldVisual>();

    private void Start()
    {
        foreach(WorldVisual world in worldVisuals)
        {
            world.visual.SetActive(false);
        }
        defaultWorld.SetActive(true);
    }
    public void LoadVisual()
    {
        int value = MasterSelection.instance.GetMasterIndex;

        bool worldLoaded = false;
        foreach(var world in worldVisuals)
        {
            if(world.index == value)
            {
                world.visual.SetActive(true);
                defaultWorld.SetActive(false);
                worldLoaded = true;
            }
            else { world.visual.SetActive(false); }
        }
        if (!worldLoaded)
        {
            Debug.LogWarning($"No world with index: ({value}) could be found. Make sure the indexes match." +
                $" Loading Default World.");
            defaultWorld.SetActive(true);
        }
    }
}
