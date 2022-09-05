using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUpgradePanel : MonoBehaviour
{
    bool unlocked = false;

    [Header("Master References")]
    [SerializeField] private int unitIndex = 0;

    [Header("Upgrades")]
    private int upgradesAvailable = 0;

    public int UpgradesAvailable
    {
        get { return upgradesAvailable; }
        set 
        {
            upgradesAvailable = value;
            upgradesAvailableText.text = "Upgrades: \n"+upgradesAvailable;
            if (value > 0)
            {
                EnableUpgrades(true);
            }
        }
    }

    [Header("General References")]
    [SerializeField] private Image unitImage = null;
    [SerializeField] private Text unitName = null;
    [SerializeField] private Text upgradeText = null;
    [SerializeField] private Text unitTypeText = null;
    [SerializeField] private Text upgradesAvailableText = null;
    [SerializeField] private GameObject unlockPanel = null;
    [SerializeField] private Button unlockButton = null;
    [SerializeField] private Text unlockButtonText = null;
    [SerializeField] private Text unityEnergyCostText = null;
    [SerializeField] private GameObject traitDescriptionPanel = null;
    [SerializeField] private Text traitDescriptionText = null;

    [Header("Health References")]
    [SerializeField] private Button upgradeHealthButton = null;
    [SerializeField] private Text upgradeHealthText= null;
    [SerializeField] private Button unlockHealthTraitButton = null;
    [SerializeField] private Text unlockHealthTraitText = null;
    [SerializeField] private Slider healthProgressSlider = null;
    [SerializeField] private Text healthValueText = null;
    [Header("Damage References")]
    [SerializeField] private Button upgradeDamageButton = null;
    [SerializeField] private Text upgradeDamageText = null;
    [SerializeField] private Button unlockDamageTraitButton = null;
    [SerializeField] private Text unlockDamageTraitText = null;
    [SerializeField] private Slider damageProgressSlider = null;
    [SerializeField] private Text damageValueText = null;
    [Header("Speed References")]
    [SerializeField] private Button upgradeSpeedButton = null;
    [SerializeField] private Text upgradeSpeedText = null;
    [SerializeField] private Button unlockSpeedTraitButton = null;
    [SerializeField] private Text unlockSpeedTraitText = null;
    [SerializeField] private Slider speedProgressSlider = null;
    [SerializeField] private Text speedValueText = null;

    private UnitSO unitSO;
    private UnitMaster unitMaster;
    private UnitList unitListRef;

    private bool[] traitUnlocked = new bool[3];
    private string[] traitDescription = new string[3];

    public void InitializeUnitUpgradePanel(UnitMaster unitMaster, UnitSO unitSO, int index)
    {
        this.unitSO = unitSO;
        this.unitMaster = unitMaster;
        unitIndex = index;
        unitListRef = unitMaster.GetUnitList(unitIndex);
        unitTypeText.text = $"Unit Type: \n{unitSO.unitDamageType}";
        upgradesAvailableText.text = "Upgrades: \n0";
        unlocked = unitSO.starterUnit;

        unlockPanel.SetActive(!unitSO.starterUnit);
        unlockButton.interactable = false;
        unlockButtonText.text = $"Unlock {unitSO.name} \nRequired XP: {unitSO.xpToUnlock}";

        unityEnergyCostText.text = "Energy Cost: " + unitSO.energyCost;

        EnableUpgrades(false);

        unitImage.sprite = unitSO.unitSprite;
        unitName.text = unitSO.name;
        SetUpgradeText(unitSO.upgrades.unitsToPlace);

        upgradeHealthText.text = $"+ {unitSO.upgrades.unitUpgradesHealth.upgradeAmount}x";
        unlockHealthTraitText.text = unitSO.upgrades.traitHealth;
        healthProgressSlider.maxValue = unitSO.upgrades.unitUpgradesHealth.amountOfUpgrades;
        healthProgressSlider.value = 0;
        healthValueText.text = $"{unitSO.health}";

        upgradeDamageText.text = $"+ {unitSO.upgrades.unitUpgradesDamage.upgradeAmount}x";
        unlockDamageTraitText.text = unitSO.upgrades.traitDamage;
        damageProgressSlider.maxValue = unitSO.upgrades.unitUpgradesDamage.amountOfUpgrades;
        damageProgressSlider.value = 0;
        damageValueText.text = $"{unitListRef.GetDamageStat()}";

        upgradeSpeedText.text = $"+ {unitSO.upgrades.unitUpgradesSpeed.upgradeAmount}x";
        unlockSpeedTraitText.text = unitSO.upgrades.traitSpeed;
        speedProgressSlider.maxValue = unitSO.upgrades.unitUpgradesSpeed.amountOfUpgrades;
        speedProgressSlider.value = 0;
        speedValueText.text = $"{unitSO.movementSpeed}";

        traitDescription[0] = unitSO.upgrades.traitHealthDescription;
        traitDescription[1] = unitSO.upgrades.traitDamageDescription;
        traitDescription[2] = unitSO.upgrades.traitSpeedDescription;

        traitDescriptionPanel.SetActive(false);
    }

    public void EnableUpgrades(bool enable)
    {
        upgradeHealthButton.interactable = enable;
        upgradeDamageButton.interactable = enable;
        upgradeSpeedButton.interactable = enable;

        unlockHealthTraitButton.interactable = traitUnlocked[0] == false ? (healthProgressSlider.value == healthProgressSlider.maxValue ? enable : false) : false;
        unlockDamageTraitButton.interactable = traitUnlocked[1] == false ? (damageProgressSlider.value == damageProgressSlider.maxValue ? enable : false) : false;
        unlockSpeedTraitButton.interactable = traitUnlocked[2] == false ? (speedProgressSlider.value == speedProgressSlider.maxValue ? enable : false) : false;
    }
    
    public void UnlockCheck(int xp)
    {
        if (unlocked) return;
        unlockButton.interactable = xp >= unitSO.xpToUnlock;
    }

    public void UnlockUnit()
    {
        unlocked = true;
        unlockPanel.SetActive(false);
        unitMaster.UnlockNew(unitListRef);
    }

    public void SetTraitDescriptions(string[] descs)
    {
        traitDescription = descs;
    }

    public void SetDescriptionText(int descId)
    {
        traitDescriptionText.text = traitDescription[descId];
    }

    public void SetUpgradeText(int amount)
    {
        upgradeText.text = $"Next Upgrade: Spawn {amount} {unitSO.name}s";
    }


    #region Button Functions
    public void UpgradeUnitHealth()
    {
        unitMaster.UpgradeUnit(unitIndex, 0, unitSO.upgrades.unitUpgradesHealth.upgradeAmount);

        EnableUpgrades(false);
    }
    public void UpgradeUnitDamage()
    {
        unitMaster.UpgradeUnit(unitIndex, 1, unitSO.upgrades.unitUpgradesDamage.upgradeAmount);

        EnableUpgrades(false);
    }
    public void UpgradeUnitSpeed()
    {
        unitMaster.UpgradeUnit(unitIndex, 2, unitSO.upgrades.unitUpgradesSpeed.upgradeAmount);

        EnableUpgrades(false);
    }

    public void UnlockHealthTrait()
    {
        unitMaster.UnlockTrait(unitIndex,0);
        unlockHealthTraitButton.GetComponent<Image>().color = Color.green;
        unlockHealthTraitButton.interactable = false;
        traitUnlocked[0] = true;

        EnableUpgrades(false);
    }
    public void UnlockDamageTrait()
    {
        unitMaster.UnlockTrait(unitIndex, 1);
        unlockDamageTraitButton.GetComponent<Image>().color = Color.green;
        unlockDamageTraitButton.interactable = false;
        traitUnlocked[1] = true;

        EnableUpgrades(false);
    }
    public void UnlockSpeedTrait()
    {
        unitMaster.UnlockTrait(unitIndex, 2);
        unlockSpeedTraitButton.GetComponent<Image>().color = Color.green;
        unlockSpeedTraitButton.interactable = false;
        traitUnlocked[2] = true;

        EnableUpgrades(false);
    }
    #endregion

    #region UI visual
    
    //The Server sends the new unit information to the Client via these methods
    //The Client then updates the text on the UI to reflect the new stats of the unit

    public void UpdateHealthText(float newHealth, int upgradesLeft)
    {
        healthValueText.text = $"{newHealth}";

        if (healthProgressSlider.value == healthProgressSlider.maxValue) return;

        int max = (int)healthProgressSlider.maxValue;
        healthProgressSlider.value = max - upgradesLeft;
    }

    public void UpdateDamageText(float newDamage, int upgradesLeft)
    {
        damageValueText.text = $"{newDamage}";

        if (damageProgressSlider.value == damageProgressSlider.maxValue) return;

        int max = (int)damageProgressSlider.maxValue;
        damageProgressSlider.value = max - upgradesLeft;
    }

    public void UpdateSpeedText(float newSpeed, int upgradesLeft)
    {
        speedValueText.text = $"{newSpeed}";

        if (speedProgressSlider.value == speedProgressSlider.maxValue) return;

        int max = (int)speedProgressSlider.maxValue;
        speedProgressSlider.value = max - upgradesLeft;
    }

    #endregion

    private void OnDisable()
    {
        traitDescriptionPanel.SetActive(false);
    }
}
