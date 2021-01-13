using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UnitButton : NetworkBehaviour
{
    [Header("Buttons")]
    public GameObject upgradeButton = null; //These need to be public, because they are accessed by Master, to add onClick events.
    public GameObject unlockButton = null;
    [Header("Visual")]
    [SerializeField] private Text levelNum = null;
    [SerializeField] private Image unitImage = null;
    [Header("Data")]
    [SerializeField] private int unitIndex;
    public int UnitIndex
    {
        get { return unitIndex; }
        set { unitIndex = value; }
    }

    private void Start()
    {
        upgradeButton.SetActive(false);
        if (unlockButton != null) { unlockButton.SetActive(false); }
    }
    public void Choose(bool chosen)
    {
        unitImage.color = chosen ? Color.red : Color.white;
    }
    public void Unlock(bool unlocked)
    {
        unitImage.color = unlocked ? Color.white : Color.grey;
        ShowUnlockButton(false);
    }
    public void SetUnitLevel(int level)
    {
        levelNum.text = level.ToString();
    }
    public void SetImage(Sprite img)
    {
        unitImage.sprite = img;
    }

    public void ShowUnlockButton(bool enable)
    {
        unlockButton.SetActive(enable);
    }
    public void ShowUpgradeButton(bool enable)
    {
        upgradeButton.SetActive(enable);
    }

    public void Upgrade()
    {
        /*
        if (zm.xp >= xpToUpgrade)
        {
            zm.xp -= xpToUpgrade;
            zm.Upgrade_Zombies(unitNum);
            int newLevel = int.Parse(levelNum.text) + 1;
            levelNum.text = newLevel.ToString();
            xpToUpgrade = xpToUpgrade + Mathf.Round(xpToUpgrade * 0.1f);
        }
        */
    }
}
