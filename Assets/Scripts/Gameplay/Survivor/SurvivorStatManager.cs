using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;
using UnityEngine.Events;

public class SurvivorStatManager : BaseStatManager, IDamagableTeam
{
    private SurvivorLevelManager level;
    private ReviveManager reviveManager;
    [SerializeField] private Animator animatorController = null;


    [Title("Stats")]
    [SerializeField, SyncVar(hook = nameof(ArmorHook))] private int armor = 0;
    public int Armor
    {
        get => armor;
        private set
        {
            armor = value;
        }
    }
    private void ArmorHook(int oldVal, int newVal)
    {
        armorBar.value = newVal;
    }

    public override int Health
    {
        get => health;
        protected set
        {
            health = Mathf.Clamp(value, 0, maxHealth);

            if (isServer)
            {
                if (health <= 0)
                {
                    IsDown = true;
                }
            }
        }
    }
    protected override void HealthHook(int oldVal, int newVal)
    {
        if (!hasAuthority) return;

        healthBar.value = health;

        lowHealthImage.color = new Color(1, 1, 1, (maxHealth / 2 - (float)health) / 100 * 2);

        if (!healthLossBool)
        {
            StartCoroutine(HealthLossCo(oldVal));
        }
    }


    [Title("UI References")]
    [SerializeField] private Slider healthBar = null;
    [SerializeField] private Slider healthLossBar = null;
    [SerializeField] private Slider armorBar = null;
    [SerializeField] private Image lowHealthImage = null;
    [SerializeField] private Image damagedImage = null;
    [SerializeField] private GameObject inGameCanvas = null;
    [Space]
    [SerializeField] private PlayerEquipment playerEquipment = null;
    //[SerializeField] private FullBodyBipedIK fullBodyBipedIK = null;
    [SerializeField] private SurvivorSetup survivorSetup = null;


    private const string inGameUIPath = "UI/Canvas - In Game";


    public UnityEvent<bool> onDownChanged = null;

    [SerializeField] private Text pointsText = null;
    [SerializeField] private GameObject pointGainPrefab = null;
    public Teams Team => Teams.Player;


    [SyncVar(hook = nameof(pointsHook))] public int points;
    private void pointsHook(int oldVal, int newVal)
    {
        pointsText.text = "Points: " + newVal;
        StartCoroutine(PointsIE(newVal - oldVal));
        level.GainExp(newVal - oldVal);
    }

    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        set
        {
            //Update scoreboard stat
            GamemodeBase.Instance.Svr_ModifyStat(GetComponent<NetworkIdentity>().netId, 0, PlayerDataStat.Alive);

            isDead = value;
            onDied?.Invoke();
            NetworkServer.Destroy(gameObject);
        }
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            reviveManager = GetComponent<ReviveManager>();
            reviveManager.onDownTimerFinished.AddListener(delegate () { OnDownTimerFinished(); });
            reviveManager.onRevived.AddListener(delegate () { OnRevived(); });
        }

    }

    private async void FindComponents()
    {
        await JODSTime.WaitTime(0.1f);
        playerEquipment = GetComponentInChildren<PlayerEquipment>();
        survivorSetup = GetComponent<SurvivorSetup>();
    }

    private async void FindCamera()
    {
        await JODSTime.WaitTime(0.2f);
        inGameCanvas = transform.Find($"{inGameUIPath}").gameObject;
    }

    public void SetStats(int maxHealth, int armor)
    {
        level = GetComponent<SurvivorLevelManager>();

        this.maxHealth = maxHealth;
        health = maxHealth;
        this.armor = armor;
        //GetComponent<ModifierManagerSurvivor>().data.MovementSpeed = movementSpeed;
        //GetComponent<SurvivorAnimationManager>().characerAnimator.speed = movementSpeed;


        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        healthLossBar.maxValue = maxHealth;
        armorBar.value = armor;
    }

    private IEnumerator PointsIE(int pointGain)
    {
        GameObject pText = Instantiate(pointGainPrefab, inGameCanvas.transform);
        Text text = pText.GetComponent<Text>();
        text.text = "+ " + pointGain;
        float time = 1;
        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
            text.color = new Color(1, 1, 1, time);
            text.transform.Translate(new Vector3(0, 0.5f, 0));
        }
        Destroy(pText);
    }

    #region Damaged

    private bool healthLossBool = false;

    private IEnumerator HealthLossCo(int prevHealth)
    {
        healthLossBool = true;
        healthLossBar.value = prevHealth;
        yield return new WaitForSeconds(1);
        while (healthLossBar.value > Health)
        {
            healthLossBar.value -= Time.deltaTime * 15;

            yield return null;
        }
        healthLossBool = false;
    }

    [TargetRpc]
    private void Rpc_DamageImage(NetworkConnection target, int damageTaken)
    {
        float dmgTaken = damageTaken;
        if ((dmgTaken / 100) > damagedImage.color.a)
        {
            if (damagedImageBool)
            {
                StopCoroutine(DamangedImageCo);
                damagedImageBool = false;
            }
            DamangedImageCo = DamagedImage(dmgTaken);
            StartCoroutine(DamangedImageCo);
        }
    }

    IEnumerator DamangedImageCo;
    private bool damagedImageBool = false;
    private IEnumerator DamagedImage(float damageTaken)
    {
        float time = 1;
        float roundedValue = Mathf.Ceil(damageTaken / 20) * 20;
        float baseValue = roundedValue / 100;
        damagedImageBool = true;
        damagedImage.color = new Color(1, 1, 1, baseValue);
        yield return new WaitForSeconds(1f);
        while (time > 0)
        {
            damagedImage.color = new Color(1, 1, 1, (time * baseValue));

            time -= Time.deltaTime;
            yield return null;
        }

        damagedImageBool = false;
    }


    [Server]
    public override void Svr_Damage(int damage, Transform target = null)
    {
        if (target)
        {
            if (target.TryGetComponent(out IDamagableTeam ITeam))
            {
                if (ITeam.Team == Teams.Player)
                {
                    float parsedDmg = damage;
                    damage = Mathf.RoundToInt(parsedDmg /= 2);
                }
            }
        }

        if (!IsDown)
        {
            if (armor > 0)
            {
                float armorPercent = (float)armor / 100;

                int armorLoss = Mathf.Clamp(Mathf.RoundToInt(damage * armorPercent), 0, 100);
                int healthLoss = damage - armorLoss;

                Health -= healthLoss;
                Armor = Mathf.Clamp(Armor -= armorLoss, 0, 100);
            }
            else
            {
                Health -= damage;
            }
            Rpc_DamageImage(connectionToClient, damage);
        }
    }
    #endregion

    [SyncVar] private bool isDown;
    public bool IsDown
    {
        get { return isDown; }
        set
        {
            isDown = value;
            onDownChanged?.Invoke(value);
            //if (isDown)
            //{

            //    var equip = playerEquipment.EquipmentItem;
            //    if (equip)
            //    {
            //        equip.Svr_Unequip();
            //    }
            //    fullBodyBipedIK.enabled = false;
            //    //survivorSetup.Rpc_ToggleHead(connectionToClient);
            //    Rpc_SetCameraForDownedState(connectionToClient);
            //}
            //else
            //{
            //    playerEquipment.EquipmentItem?.Svr_Equip();
            //    fullBodyBipedIK.enabled = true;
            //    //survivorSetup.Rpc_ToggleHead(connectionToClient);
            //    Rpc_SetCameraForRevivedState(connectionToClient);
            //}
        }
    }


    private void OnDownTimerFinished()
    {
        IsDead = true;
    }

    private void OnRevived()
    {
        Health = 50;
        IsDown = false;
    }

    #region ViewModel

    #endregion
}
