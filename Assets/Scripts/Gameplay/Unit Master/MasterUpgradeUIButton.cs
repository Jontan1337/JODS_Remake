using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MasterUpgradeType
{
    RechargeRate = 0,
    MaxEnergy = 1,
    UnitCapacity = 2,
    SurvivorOutlines = 3
}
public class MasterUpgradeUIButton : MonoBehaviour
{
    [SerializeField] private MasterUpgradeType upgradeType = 0;
    [Space]
    [SerializeField] private UnitMaster masterReference = null;
    [Space]
    [SerializeField] private int currentUpgrades = 0;
    [Space]
    [SerializeField] private int[] totalUpgrades = null; //The array contains what level the master is required to be to upgrade
    [Space]
    [SerializeField] private Transform imgCounterContainer = null;
    [SerializeField] private GameObject imgCounter = null;

    private List<Image> counters = new List<Image>();
    
    
    private Button btn = null;

    private void Start()
    {
        btn = GetComponent<Button>();
        for (int i = 0; i < totalUpgrades.Length; i++)
        {
            counters.Add(Instantiate(imgCounter, imgCounterContainer).GetComponent<Image>());
        }
    }

    public void UnlockButton(int masterLevel, bool enable)
    {
        btn.interactable = (enable ? masterLevel >= RequiredLevel : false);
    }

    public void Upgrade()
    {
        masterReference.UpgradeMaster(upgradeType);
        currentUpgrades++;

        for (int i = 0; i < totalUpgrades.Length; i++)
        {
            counters[i].color = currentUpgrades > i ? Color.red : Color.white;
        }

        ShowDescription();
    }

    private int RequiredLevel => currentUpgrades < totalUpgrades.Length ? totalUpgrades[currentUpgrades] : 0;


    [Header("Description")]
    [SerializeField] private string title = "";
    [SerializeField, TextArea(2, 10)] private string description = "";

    [Header("References")]
    [SerializeField] private Text titleText = null;
    [SerializeField] private Text descriptionText = null;

    public void ShowDescription()
    {
        titleText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);

        titleText.text = title;

        descriptionText.text = "Required Level: " + RequiredLevel + 
            "\n\n" + 
            description;
    }

    public void HideDescription()
    {
        titleText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);

        titleText.text = title;
        descriptionText.text = description;
    }
}
