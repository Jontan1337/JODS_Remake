using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUpgradePanel : MonoBehaviour
{
    [Header("General References")]
    [SerializeField] private Image unitImage = null;

    [Header("Health References")]
    [SerializeField] private Text upgradeHealthText= null;
    [SerializeField] private Text unlockHealthTraitText = null;
    [SerializeField] private Slider healthProgressSlider = null;
    [Header("Damage References")]
    [SerializeField] private Text upgradeDamageText = null;
    [SerializeField] private Text unlockDamageTraitText = null;
    [SerializeField] private Slider damageProgressSlider = null;
    [Header("Speed References")]
    [SerializeField] private Text upgradeSpeedText = null;
    [SerializeField] private Text unlockSpeedTraitText = null;
    [SerializeField] private Slider speedProgressSlider = null;

    private UnitSO unitSO;
    private UnitMaster unitMaster;

    public void InitializeUnitUpgradePanel(UnitMaster unitMaster, UnitSO unitSO)
    {
        this.unitSO = unitSO;
        this.unitMaster = unitMaster;

        unitImage.sprite = unitSO.unitSprite;

        upgradeHealthText.text = unitSO.upgrades.unitUpgradesHealth.upgradeAmount.ToString();
        unlockHealthTraitText.text = unitSO.upgrades.traitHealth;
        healthProgressSlider.maxValue = unitSO.upgrades.unitUpgradesHealth.amountOfUpgrades;
        healthProgressSlider.value = 0;

        upgradeDamageText.text = unitSO.upgrades.unitUpgradesDamage.upgradeAmount.ToString();
        unlockDamageTraitText.text = unitSO.upgrades.traitDamage;
        damageProgressSlider.maxValue = unitSO.upgrades.unitUpgradesDamage.amountOfUpgrades;
        damageProgressSlider.value = 0;

        upgradeSpeedText.text = unitSO.upgrades.unitUpgradesSpeed.upgradeAmount.ToString();
        unlockSpeedTraitText.text = unitSO.upgrades.traitSpeed;
        speedProgressSlider.maxValue = unitSO.upgrades.unitUpgradesSpeed.amountOfUpgrades;
        speedProgressSlider.value = 0;
    }
}
