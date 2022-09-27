using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;
using UnityEngine.Events;

public class CharacterStatManager : NetworkBehaviour, IDamagable
{
    private SurvivorLevelManager level;
    private ReviveManager reviveManager;
    [SerializeField] private Animator animatorController = null;


    [Title("Stats")]
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int armor = 0;
    [SerializeField] private float movementSpeed = 0;


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

    private Transform cameraTransform;
    private Transform originalCameraTransformParent;

    [Header("Events")]
    public UnityEvent onDied = null;
    public UnityEvent onDamaged = null;
    public UnityEvent<bool> onDownChanged = null;

    [SerializeField] private Text pointsText = null;
    [SerializeField] private GameObject pointGainPrefab = null;


    [SyncVar(hook = nameof(pointsHook))] public int points;
    private void pointsHook(int oldVal, int newVal)
    {
        pointsText.text = "Points: " + newVal;
        StartCoroutine(PointsIE(newVal - oldVal));
        level.GainExp(newVal - oldVal);
    }

    public float MovementSpeed => movementSpeed;

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

    public int GetHealth => currentHealth;

    public int Health
    {
        get => currentHealth;
        private set
        {
            int prevHealth = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            healthBar.value = currentHealth;


            lowHealthImage.color = new Color(1, 1, 1, (maxHealth / 2 - (float)currentHealth) / 100 * 2);

            if (!healthLossBool)
            {
                StartCoroutine(HealthLossCo(prevHealth));
            }
            if (isServer)
            {
                if (currentHealth <= 0)
                {
                    IsDown = true;
                }
                Rpc_SyncStats(connectionToClient, currentHealth, armor);
            }
        }
    }

    public int Armor
    {
        get => armor;
        private set
        {
            armor = value;
            armorBar.value = armor;
            if (isServer)
            {
                Rpc_SyncStats(connectionToClient, currentHealth, armor);
            }
        }
    }

    public override void OnStartServer()
    {
        FindComponents();

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
        cameraTransform = transform.Find("Virtual Head(Clone)/PlayerCamera(Clone)");
        originalCameraTransformParent = transform.Find("Virtual Head(Clone)");
    }

    private async void FindCamera()
    {
        await JODSTime.WaitTime(0.2f);
        cameraTransform = transform.Find("Virtual Head(Clone)/PlayerCamera(Clone)");
        originalCameraTransformParent = transform.Find("Virtual Head(Clone)");
        inGameCanvas = transform.Find($"{inGameUIPath}").gameObject;
    }

    public void SetStats(int maxHealth, int armor, float movementSpeed)
    {
        level = GetComponent<SurvivorLevelManager>();

        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.armor = armor;
        this.movementSpeed = movementSpeed;
        GetComponent<ModifierManagerSurvivor>().data.MovementSpeed = movementSpeed;
        GetComponent<SurvivorAnimationIKManager>().anim.speed = movementSpeed;


        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        healthLossBar.maxValue = maxHealth;
        armorBar.value = armor;
    }

    // Why are we using an Rpc to update the player's armor instead of just making it a SyncVar?????
    [TargetRpc]
    public void Rpc_SyncStats(NetworkConnection target, int newHealth, int newArmor)
    {
        if (isServer) return;
        Health = newHealth;
        Armor = newArmor;
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

    public Teams Team => Teams.Player;

    [Server]
    public void Svr_Damage(int damage, Transform target = null)
    {
        Damage(damage, target);
    }

    [Command]
    public void Cmd_Damage(int damage)
    {
        Damage(damage);
    }

    void Damage(int damage, Transform source = null)
    {
        if (source)
        {
            if (source.GetComponent<IDamagable>().Team == Teams.Player)
            {
                float parsedDmg = damage;
                damage = Mathf.RoundToInt(parsedDmg /= 2);
            }
        }

        onDamaged?.Invoke();

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
    [TargetRpc]
    private void Rpc_SetCameraForDownedState(NetworkConnection target)
    {
        //cameraTransform.SetParent(fullBodyBipedIK.references.head.GetChild(0));
    }
    [TargetRpc]
    private void Rpc_SetCameraForRevivedState(NetworkConnection target)
    {
        cameraTransform.SetParent(originalCameraTransformParent);
        cameraTransform.localPosition = new Vector3(0f, 0.1f, 0f);
        cameraTransform.rotation = originalCameraTransformParent.rotation;
    }

    #endregion
}
