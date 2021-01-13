using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Stats : NetworkBehaviour, IDamagable
{
	[Header("Data")]
	[Tooltip("SyncVar")]
	[SyncVar, SerializeField] private float healthPoints = 100;
	[Tooltip("SyncVar")]
	[SyncVar, SerializeField] private float armor = 0;
	[Tooltip("SyncVar")]
	[SyncVar, SerializeField] private float energy = 100;
    [Tooltip("SyncVar")]
    [SyncVar, SerializeField] private float specialCooldown = 0;
    public float maxCooldown;
    [Tooltip("SyncVar")]
	[SyncVar, SerializeField] private int damageReduction = 0;
	[Tooltip("SyncVar")]
	[SyncVar, SerializeField] private bool isDead = false;
	public string _class;

    [SerializeField]
    public float HealthPoints
    {
        get => healthPoints;
        set {
            healthPoints = Mathf.Clamp(value, 0f, 100f);
            isDead = healthPoints == 0f;
            if (isDead)
            {
                GetComponent<PlayerUnit>().Die();
            }
        }
    }
    public float Armor
    {
        get => armor;
        set => armor = Mathf.Clamp(value, 0f, 100f);
    }
    public float Energy
    {
        get => energy;
        set => energy = Mathf.Clamp(value, 0f, 100f);
    }
    public float SpecialCooldown
    {
        get => specialCooldown;
        set => specialCooldown = Mathf.Clamp(value, 0f, 100f);
    }
    public int DamageReduction
    {
        get => damageReduction;
        set => damageReduction = Mathf.Clamp(value, 0, 100);
    }
    public bool IsDead
    {
        get => isDead;
    }

    public Teams Team => Teams.Player;

    public bool isReloading;

	[Header("Canvas items")]
	public Slider hpSlider;
    public Slider armourSlider;
	public Text hpText;
	public Text ammoText;
	public Text grenadeText;
    public Image hurt;
    public Image specialImage;

	[Header("Other")]
	public GameObject armPivot;
	private PlayerUnit pUnit;
	private Shoot shoot;
    //private UseItem UseItemScript;
    private Movement movement;

    [Header("Audio")]
    public AudioClip[] hurtSounds;
    private AudioSource AS;

    void Start()
	{
		shoot = GetComponent<Shoot>();
		//UseItemScript = GetComponent<UseItem>();
		pUnit = GetComponent<PlayerUnit>();
        movement = GetComponent<Movement>();
        AS = GetComponent<AudioSource>();

    }
	void Update()
	{
		if (!hasAuthority) return;

        //reset red image when hurt slowly does ded
        Color tempColor = hurt.color;
        tempColor.a -= Time.deltaTime;
        hurt.color = tempColor;
        if (Energy < 100)
		{
			Energy += Time.deltaTime;
		}
		//if (UseItemScript.hasItem)
		//{
		//	grenadeText.text = UseItemScript.currentItem.GetComponent<Item>().itemName;
		//}
		//else
		//{
		//	grenadeText.text = "";
		//}
		if (shoot.hasWeapon && shoot.weaponType)
		{
            //print(shoot.weaponType);
            if (shoot.weaponType.weaponType != WeaponType.Type.melee)
            {
		        //Reload
                if (Input.GetKeyDown(KeyCode.R))
                {
			        ReloadStart(shoot.weaponStats.reloadTime);

                }
                if (shoot.weaponStats.isStarterWeapon)
                {
                    ammoText.text = shoot.weaponStats.magazineCurrent + " / ∞";
                }
                else
                {
                    ammoText.text = shoot.weaponStats.magazineCurrent + " / " + shoot.weaponStats.ammunition;
                }
			    
            }
		}
		else
		{
			ammoText.text = "";
		}
		//HP
		hpSlider.value = HealthPoints;
		hpText.text = HealthPoints.ToString("N0") + "/100 HP".ToString();

        armourSlider.value = Armor;

        specialImage.fillAmount = SpecialCooldown / maxCooldown;
	}

	public void Svr_Damage(int damage)
	{
        Debug.Log("Taking " + damage + " damage");
        if (hasAuthority)
        {
            // Red image when hurt will appear
            var tempColor = hurt.color;
            tempColor.a = 0.75f;
            hurt.color = tempColor;

            // I say oof
            AS.volume = 0.5f;
            AS.pitch = Random.Range(0.9f, 1.1f);
            AS.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);

            //tell erryon im taken have damaged
            CmdDamageTaken(damage);

            //Ow I got hit, better slow myself down so I can die
            movement.SlowDown();

            //if (HealthPoints <= 0 && !IsDead)
            //{
            //    Debug.Log("Should be dead");
            //    IsDead = true;
            //    GetComponent<PlayerUnit>().Die();
            //}
        }
	}

	[Command]
	void CmdDamageTaken(int damage)
	{
        RpcDamageTaken(damage);
	}
    [ClientRpc]
    void RpcDamageTaken(int damage)
    {
        //This doesnt work. It returns 0. Fix pls, I cba myself.
        //damage - damageReduction) / 100 * (100 - Armor));

        int dam = Mathf.RoundToInt(damage);
        //Debug.Log(dam);
        HealthPoints -= dam;
    }
	//---------RELOAD-------------
	public void ReloadStart(float reloadTime)
	{
		if (!isReloading)
		{
			if (shoot.weaponStats.ammunition > 0 && shoot.weaponStats.magazineCurrent < shoot.weaponStats.magazineMax)
			{
                // TODO: Fucc reload animation 
                Invoke(nameof(ReloadFinished), reloadTime);
				armPivot.GetComponent<Animator>().SetBool("Reload", true);
                CmdReloadStart();
			}
		}
	}	
    [Command]
    void CmdReloadStart()
    {
        isReloading = true;
    }
	private void ReloadFinished()
	{
        CmdReloadFinished();
		armPivot.GetComponent<Animator>().SetBool("Reload", false);
	}
    [Command]
    public void CmdReloadFinished()
    {
        if (shoot.hasWeapon)
        {
            if (shoot.weaponStats.ammunition > (shoot.weaponStats.magazineMax - shoot.weaponStats.magazineCurrent))
            {
                shoot.weaponStats.ammunition = shoot.weaponStats.ammunition - (shoot.weaponStats.magazineMax - shoot.weaponStats.magazineCurrent);
                shoot.weaponStats.magazineCurrent = shoot.weaponStats.magazineMax;
            }
            else
            {
                shoot.weaponStats.magazineCurrent = shoot.weaponStats.magazineCurrent + shoot.weaponStats.ammunition;
                shoot.weaponStats.ammunition = 0;
            }
        }
        else
        {
		    armPivot.GetComponent<Animator>().SetBool("Reload", false);
        }
        isReloading = false;
    }
}
