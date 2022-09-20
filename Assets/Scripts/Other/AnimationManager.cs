using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator itemContainerAnimatorController = null;
    [SerializeField] private SurvivorSetup survivorSetup = null;

    private void Awake()
    {
        survivorSetup.onClientSpawnItem += OnClientItemSpawned;
    }

    public void PlayItemContainerAnimation(bool walking)
    {
        if (itemContainerAnimatorController)
            itemContainerAnimatorController.SetBool("Walking", walking);
    }

    private void OnClientItemSpawned(GameObject item)
    {
        switch (item.name)
        {
            case "Item Container Animator(Clone)":
                itemContainerAnimatorController = item.GetComponent<Animator>();
                break;
            default:
                break;
        }
    }
}
