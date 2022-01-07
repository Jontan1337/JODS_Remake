using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class MasterUIGameplayButton : NetworkBehaviour
{
    [Header("Buttons")]
    public GameObject upgradeButton = null; //These need to be public, because they are accessed by Master, to add onClick events.
    public GameObject unlockButton = null;

    [Header("Ui References - Shared")]
    [SerializeField] private Image imageRef = null;
    [SerializeField] private GameObject detailsBox = null;
    [SerializeField] private TMP_Text nameText = null;
    [SerializeField] private TMP_Text descriptionText = null;

    [Header("Ui References - Unit")]
    [SerializeField] private Text levelNumText = null;
    [SerializeField] private Slider powerSlider = null;
    [SerializeField] private Slider healthSlider = null;

    [Header("Ui References - Deployable")]
    [SerializeField] private Image cooldownImageRef = null;

    [Header("Data")]
    [SerializeField] private int buttonIndex;

    public int ButtonIndex
    {
        get { return buttonIndex; }
        set { buttonIndex = value; }
    }

    private void Start()
    {
        if (upgradeButton != null) upgradeButton.SetActive(false);
        if (unlockButton != null) unlockButton.SetActive(false); 

        //Details box
        detailsBox.SetActive(false); //Deactivate it on start
    }
    public void Choose(bool chosen)
    {
        imageRef.color = chosen ? Color.red : Color.white;
    }
    public void Unlock(bool unlocked)
    {
        imageRef.color = unlocked ? Color.white : Color.grey;
        ShowUnlockButton(false);
    }
    public void SetUnitLevel(int level)
    {
        levelNumText.text = level.ToString();
    }
    public void SetImage(Sprite img)
    {
        imageRef.sprite = img;
    }
    public void ShowUnlockButton(bool enable)
    {
        unlockButton.SetActive(enable);
    }
    public void ShowUpgradeButton(bool enable)
    {
        upgradeButton.SetActive(enable);
    }
    public void SetDetails(string unitName, string desc, int power = 0, int health = 0)
    {
        nameText.text = unitName;
        descriptionText.text = desc;

        if (power != 0) powerSlider.value = power;
        if (health != 0) healthSlider.value = health;
    }

    public void StartCooldown(int time)
    {
        StartCoroutine(CooldownUI((float)time));
    }
    //This coroutine is purely visual
    public IEnumerator CooldownUI(float time)
    {
        float refTime = time;

        cooldownImageRef.fillAmount = 1;

        while (time > 0)
        {
            yield return new WaitForSeconds(1f);
            time--;
            cooldownImageRef.fillAmount = time / refTime;
        }
    }
}
