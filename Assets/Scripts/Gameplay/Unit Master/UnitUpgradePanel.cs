using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUpgradePanel : MonoBehaviour
{
    bool unlocked = false;

    [Header("Master References")]
    [SerializeField] private int unitIndex = 0;

    [Header("General References")]
    [SerializeField] private Image unitImage = null;
    [SerializeField] private Text unitName = null;
    [SerializeField] private Text upgradeText = null;
    [SerializeField] private Text unitTypeText = null;
    [SerializeField] private GameObject unlockPanel = null;
    [SerializeField] private Button unlockButton = null;
    [SerializeField] private Text unlockButtonText = null;

    [Header("Health References")]
    [SerializeField] private Button upgradeHealthButton= null;
    [SerializeField] private Text upgradeHealthText= null;
    [SerializeField] private Text unlockHealthTraitText = null;
    [SerializeField] private Slider healthProgressSlider = null;
    [SerializeField] private Text healthValueText = null;
    [Header("Damage References")]
    [SerializeField] private Button upgradeDamageButton = null;
    [SerializeField] private Text upgradeDamageText = null;
    [SerializeField] private Text unlockDamageTraitText = null;
    [SerializeField] private Slider damageProgressSlider = null;
    [SerializeField] private Text damageValueText = null;
    [Header("Speed References")]
    [SerializeField] private Button upgradeSpeedButton = null;
    [SerializeField] private Text upgradeSpeedText = null;
    [SerializeField] private Text unlockSpeedTraitText = null;
    [SerializeField] private Slider speedProgressSlider = null;
    [SerializeField] private Text speedValueText = null;

    private UnitSO unitSO;
    private UnitMaster unitMaster;
    private UnitList unitListRef;

    public void InitializeUnitUpgradePanel(UnitMaster unitMaster, UnitSO unitSO, int index)
    {
        this.unitSO = unitSO;
        this.unitMaster = unitMaster;
        unitIndex = index;
        unitListRef = unitMaster.GetUnitList(unitIndex);
        unitTypeText.text = $"Unit Type: \n{unitSO.unitDamageType}";
        unlocked = unitSO.starterUnit;

        unlockPanel.SetActive(!unitSO.starterUnit);
        unlockButton.interactable = false;
        unlockButtonText.text = $"Unlock {unitSO.name} \nRequired XP: {unitSO.xpToUnlock}";

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
    }

    public void EnableUpgrades(bool enable)
    {
        upgradeHealthButton.interactable = enable;
        upgradeDamageButton.interactable = enable;
        upgradeSpeedButton.interactable = enable;
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

    public void SetUpgradeText(int amount)
    {
        upgradeText.text = $"Next Upgrade: Spawn {amount} {unitSO.name}s";
    }


    #region Button Functions
    public void UpgradeUnitHealth()
    {
        int upgradesLeft = unitMaster.UpgradeUnit(unitIndex, 0, unitSO.upgrades.unitUpgradesHealth.upgradeAmount);

        SetUpgradeText(unitListRef.UpgradeMilestone);

        int newHealth = unitListRef.GetHealthStat();
        healthValueText.text = $"{newHealth}";

        if (healthProgressSlider.value == healthProgressSlider.maxValue) return;

        int max = (int)healthProgressSlider.maxValue;
        healthProgressSlider.value = max - upgradesLeft;

        EnableUpgrades(false);
    }
    public void UpgradeUnitDamage()
    {
        int upgradesLeft = unitMaster.UpgradeUnit(unitIndex, 1, unitSO.upgrades.unitUpgradesDamage.upgradeAmount);

        SetUpgradeText(unitListRef.UpgradeMilestone);

        int newDamage = unitListRef.GetDamageStat();
        damageValueText.text = $"{newDamage}";

        if (damageProgressSlider.value == damageProgressSlider.maxValue) return;

        int max = (int)damageProgressSlider.maxValue;
        damageProgressSlider.value = max - upgradesLeft;

        EnableUpgrades(false);
    }
    public void UpgradeUnitSpeed()
    {
        int upgradesLeft = unitMaster.UpgradeUnit(unitIndex, 2, unitSO.upgrades.unitUpgradesSpeed.upgradeAmount);

        SetUpgradeText(unitListRef.UpgradeMilestone);

        float newSpeed = unitListRef.GetSpeedStat();
        speedValueText.text = $"{newSpeed}";

        if (speedProgressSlider.value == speedProgressSlider.maxValue) return;

        int max = (int)speedProgressSlider.maxValue;
        speedProgressSlider.value = max - upgradesLeft;

        EnableUpgrades(false);
    }
    #endregion
}
