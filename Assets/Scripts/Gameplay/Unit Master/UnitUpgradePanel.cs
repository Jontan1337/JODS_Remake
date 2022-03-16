using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUpgradePanel : MonoBehaviour
{
    [Header("Master References")]
    [SerializeField] private int unitIndex = 0;

    [Header("General References")]
    [SerializeField] private Image unitImage = null;
    [SerializeField] private Text unitName = null;
    [SerializeField] private Text upgradeText = null;

    [Header("Health References")]
    [SerializeField] private Text upgradeHealthText= null;
    [SerializeField] private Text unlockHealthTraitText = null;
    [SerializeField] private Slider healthProgressSlider = null;
    [SerializeField] private Text healthValueText = null;
    [Header("Damage References")]
    [SerializeField] private Text upgradeDamageText = null;
    [SerializeField] private Text unlockDamageTraitText = null;
    [SerializeField] private Slider damageProgressSlider = null;
    [SerializeField] private Text damageValueText = null;
    [Header("Speed References")]
    [SerializeField] private Text upgradeSpeedText = null;
    [SerializeField] private Text unlockSpeedTraitText = null;
    [SerializeField] private Slider speedProgressSlider = null;
    [SerializeField] private Text speedValueText = null;

    private UnitSO unitSO;
    private UnitMaster unitMaster;

    public void InitializeUnitUpgradePanel(UnitMaster unitMaster, UnitSO unitSO, int index)
    {
        this.unitSO = unitSO;
        this.unitMaster = unitMaster;
        unitIndex = index;

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
        damageValueText.text = $"{(unitSO.melee.meleeDamageMin + unitSO.melee.meleeDamageMax) / 2}";

        upgradeSpeedText.text = $"+ {unitSO.upgrades.unitUpgradesSpeed.upgradeAmount}x";
        unlockSpeedTraitText.text = unitSO.upgrades.traitSpeed;
        speedProgressSlider.maxValue = unitSO.upgrades.unitUpgradesSpeed.amountOfUpgrades;
        speedProgressSlider.value = 0;
        speedValueText.text = $"{unitSO.movementSpeed}";
    }

    private void SetUpgradeText(int amount)
    {
        upgradeText.text = $"Upgrade: Spawn {amount} {unitSO.name}s";
    }

    public void UpgradeUnitHealth()
    {
        int upgradesLeft = unitMaster.UpgradeUnit(unitIndex, 0, unitSO.upgrades.unitUpgradesHealth.upgradeAmount);

        UnitList unitList = unitMaster.GetUnitList(unitIndex);
        SetUpgradeText(unitList.upgradeMilestone);

        int newHealth = unitList.GetHealthStat();
        healthValueText.text = $"{newHealth}";

        if (healthProgressSlider.value == healthProgressSlider.maxValue) return;

        int max = (int)healthProgressSlider.maxValue;
        healthProgressSlider.value = max - upgradesLeft;
    }
    public void UpgradeUnitDamage()
    {
        int upgradesLeft = unitMaster.UpgradeUnit(unitIndex, 1, unitSO.upgrades.unitUpgradesDamage.upgradeAmount);

        UnitList unitList = unitMaster.GetUnitList(unitIndex);
        SetUpgradeText(unitList.upgradeMilestone);

        int newDamage = unitList.GetDamageStat();
        damageValueText.text = $"{newDamage}";

        if (damageProgressSlider.value == damageProgressSlider.maxValue) return;

        int max = (int)damageProgressSlider.maxValue;
        damageProgressSlider.value = max - upgradesLeft;
    }
    public void UpgradeUnitSpeed()
    {
        int upgradesLeft = unitMaster.UpgradeUnit(unitIndex, 2, unitSO.upgrades.unitUpgradesSpeed.upgradeAmount);

        UnitList unitList = unitMaster.GetUnitList(unitIndex);
        SetUpgradeText(unitList.upgradeMilestone);

        int newSpeed = unitList.GetSpeedStat();
        speedValueText.text = $"{newSpeed}";

        if (speedProgressSlider.value == speedProgressSlider.maxValue) return;

        int max = (int)speedProgressSlider.maxValue;
        speedProgressSlider.value = max - upgradesLeft;

    }
}
