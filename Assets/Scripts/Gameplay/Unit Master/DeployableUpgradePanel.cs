using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeployableUpgradePanel : MonoBehaviour
{
    bool unlocked = false;

    [Header("Master References")]
    [SerializeField] private int deployableIndex = 0;

    [Header("General References")]
    [SerializeField] private Image deployableImage = null;
    [SerializeField] private Text deployableName = null;
    [SerializeField] private GameObject unlockPanel = null;
    [SerializeField] private Button unlockButton = null;
    [SerializeField] private Text unlockButtonText = null;

    private DeployableSO deployableSO;
    private UnitMaster unitMaster;
    private DeployableList deployableListRef;

    public void InitializeUnitUpgradePanel(UnitMaster unitMaster, DeployableSO deployableSO, int index)
    {
        this.deployableSO = deployableSO;
        this.unitMaster = unitMaster;

        deployableIndex = index;
        deployableListRef = unitMaster.GetDeployableList(deployableIndex);
        unlocked = false;

        unlockPanel.SetActive(true);
        unlockButton.interactable = false;
        unlockButtonText.text = $"Unlock {deployableSO.name} \nRequired XP: {deployableSO.xpToUnlock}";

        deployableImage.sprite = deployableSO.deployableSprite;
        deployableName.text = deployableSO.name;
    }

    public void UnlockCheck(int xp)
    {
        if (unlocked) return;
        unlockButton.interactable = xp >= deployableSO.xpToUnlock;
    }

    public void UnlockDeployable()
    {
        unlocked = true;
        unlockPanel.SetActive(false);
        unitMaster.UnlockNew(null, deployableListRef);
    }
}
