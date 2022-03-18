using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class MasterUIGameplayButton : NetworkBehaviour
{
    Button btn;

    [Header("Ui References - Shared")]
    [SerializeField] private Image imageRef = null;
    [SerializeField] private GameObject detailsBox = null;
    [SerializeField] private TMP_Text nameText = null;
    [SerializeField] private TMP_Text descriptionText = null;

    [Header("Ui References - Unit")]
    [SerializeField] private Slider powerSlider = null;
    [SerializeField] private Slider healthSlider = null;

    [Header("Ui References - Deployable")]
    [SerializeField] private Image cooldownImageRef = null;

    private void Awake()
    {
        btn = GetComponent<Button>();

        //Details box
        detailsBox.SetActive(false); //Deactivate it on start
    }
    public void Choose(bool chosen)
    {
        imageRef.color = chosen ? Color.red : Color.white;
    }

    public void SetDetails(Sprite img, string unitName, string desc, int power = 0, int health = 0)
    {
        imageRef.sprite = img;

        nameText.text = unitName;
        descriptionText.text = desc;
        
        if (power != 0) powerSlider.value = power;
        if (health != 0) healthSlider.value = health;
    }

    public void Unlock(bool unlock)
    {
        btn.interactable = unlock;
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
