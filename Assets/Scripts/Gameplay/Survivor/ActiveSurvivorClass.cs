using Mirror;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActiveSurvivorClass : NetworkBehaviour
{

    [Header("Debug")]
    public bool test;
    [Space]

    [Title("Data")]
    public SurvivorSO survivorSO;

    [SyncVar(hook = nameof(SetSurvivorClassSettings))] public Survivor sClass;
    private SurvivorClassStatManager sClassStatManager;
    private SurvivorStatManager characterStatManager;
    private SurvivorLevelManager survivorLvlManager;
    private ModifierManagerSurvivor survivorModifiers;


    [SerializeField] private SkinnedMeshRenderer bodyRenderer = null;
    [SerializeField] private SkinnedMeshRenderer headRenderer = null;
 

    [SerializeField] private GameObject starterWeapon;

    private Vector3 cameraRotationBeforeDown;
    private const string inGameUIPath = "UI/Canvas - In Game";

    public override void OnStartAuthority()
    {
        if (test) SetSurvivorClass(survivorSO);
    }

    #region Class Stuff
    [ClientRpc]
    public void Rpc_SetSurvivorClass(string _class)
    {
        List<SurvivorSO> survivorSOList = PlayableCharactersManager.Instance.GetAllSurvivors();
        foreach (SurvivorSO survivor in survivorSOList)
        {
            if (survivor.name == _class)
            {
                SetSurvivorClass(survivor);
                break;
            }
        }
    }

    public void SetSurvivorClass(SurvivorSO survivorSO)
    {
        this.survivorSO = survivorSO;

        GetComponent<SurvivorPlayerData>().classType = survivorSO.survivorName;

        if (hasAuthority)
        {
            Cmd_SpawnClass();
        }
    }

    private void SetSurvivorClassSettings(Survivor oldValue, Survivor newValue)
    {
        if (survivorSO.abilityObject && newValue)
        {
            newValue.abilityObject = survivorSO.abilityObject;
        }
        sClassStatManager = GetComponent<SurvivorClassStatManager>();
        characterStatManager = GetComponent<SurvivorStatManager>();
        survivorLvlManager = GetComponent<SurvivorLevelManager>();
        survivorModifiers = GetComponent<ModifierManagerSurvivor>();

        survivorModifiers.data.MovementSpeed = survivorSO.baseModifiers.MovementSpeed;
        survivorModifiers.data.Healing = survivorSO.baseModifiers.Healing;
        survivorModifiers.data.DamageResistance = survivorSO.baseModifiers.DamageResistance;
        survivorModifiers.data.FireResistance = survivorSO.baseModifiers.FireResistance;
        survivorModifiers.data.RangedDamage = survivorSO.baseModifiers.RangedDamage;
        survivorModifiers.data.MeleeDamage = survivorSO.baseModifiers.MeleeDamage;
        survivorModifiers.data.AbilityDamage = survivorSO.baseModifiers.AbilityDamage;
        survivorModifiers.data.Cooldown = survivorSO.baseModifiers.Cooldown;
        survivorModifiers.data.ReloadSpeed = survivorSO.baseModifiers.ReloadSpeed;
        survivorModifiers.data.Accuracy = survivorSO.baseModifiers.Accuracy;

        characterStatManager.SetStats(survivorSO.maxHealth, survivorSO.startingArmor);
        sClassStatManager.SetStats(survivorSO.abilityCooldown);

        bodyRenderer.sharedMesh = survivorSO.bodyMesh;
        headRenderer.sharedMesh = survivorSO.headMesh;

        bodyRenderer.material = survivorSO.characterMaterial;
        headRenderer.material = survivorSO.characterMaterial;
    }

    [Command]
    private void Cmd_SpawnClass()
    {
        StartCoroutine(SpawnClassObjects());
    }

    IEnumerator SpawnClassObjects()
    {
        yield return new WaitForSeconds(0.2f);

        GameObject selectedClass = Instantiate(survivorSO.classScript);
        NetworkServer.Spawn(selectedClass, gameObject);
        selectedClass.transform.SetParent(gameObject.transform);

        sClass = selectedClass.GetComponent<Survivor>();

        if (survivorSO.starterWeapon)
        {
            starterWeapon = Instantiate(survivorSO.starterWeapon, transform.position, transform.rotation);
            NetworkServer.Spawn(starterWeapon);
            yield return new WaitForSeconds(0.35f);
            starterWeapon.GetComponent<EquipmentItem>().Svr_PerformInteract(gameObject);
        }
    }
    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GetComponentInChildren<IHitter>()?.OnFlyingKickHit(hit);
    }

    private void OnGUI()
    {
        if (test)
        {
            GUI.TextField(new Rect(20, 20, 150, 20), "Active S Class Test ON");
        }
    }



}
