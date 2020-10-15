using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Linq;

public class ZombieMaster : NetworkBehaviour
{
    public Camera cam;
    public UnitButton unit;
    public GameObject zombie;

    [Header("Stats")]
    public float energy;
    public float xp;
    public float maxEnergy = 100f;
    public float energyUpgradeTime = 150f;
    private readonly float energyUpgradeReset = 12000f;
    private readonly float maxEnergyIncrease = 20f;
    public float energyRechargeRate = 3f;

    [Header("Zombie Levels")]
    [SyncVar] public int _levelZombie = 1;
    [SyncVar] public int _levelSlav = 1;
    [SyncVar] public int _levelSpitter = 1;
    [SyncVar] public int _levelStronk = 1;
    [SyncVar] public int _levelTentacle = 1;

    [Header("Zombie Amounts")]
    [SerializeField] private int maxAmount_Zombie = 0; // 0 = no limit
    [SerializeField] private int maxAmount_Slav = 0;
    [SerializeField] private int maxAmount_Spitter = 5;
    [SerializeField] private int maxAmount_Tentacle = 3;
    [SerializeField] private int maxAmount_Stronk = 1;
    [Space]
    [SerializeField] private int totalMaxAmountOfZombies = 50;

    //Remove this
    [Header("Zombie Refunds")]
    public float zombieRefund = 2f;
    public float slavRefund = 4f;
    public float spitterRefund = 5f;
    public float stronkRefund = 8f;
    public float tentacleRefund = 8f;
    //

    [Header("UI")]
    [SerializeField] private Text energyText = null;
    [SerializeField] private Slider energySlider = null;
    [SerializeField] private Slider energyUseSlider = null;
    [SerializeField] private Text xpText = null;
    [SerializeField] private Text spawnText = null;
    [SerializeField] private GameObject RechargeRateButton = null;
    [SerializeField] private GameObject MaxEnergyButton = null;

    bool hover = false;

    [Header("Navigation")]
    [SerializeField] private GameObject[] floors = new GameObject[0];
    [SerializeField] private int floor = 1;
    [SerializeField] private KeyCode floorUp = KeyCode.LeftShift;
    [SerializeField] private KeyCode floorDown = KeyCode.LeftControl;
    [SerializeField] private int positionChange = 5;

    [SerializeField] private LayerMask ignoreOnRaycast;

    [Header("Cheats")]
    [SerializeField] private KeyCode xpCheatButton = KeyCode.KeypadPlus;
    [SerializeField] private KeyCode energyCheatButton = KeyCode.KeypadMinus;
    [SerializeField] private KeyCode maxEnergyCheatButton = KeyCode.KeypadMultiply;
    [SerializeField] private bool cheatsOn = false;

    private void Start()
    {
        name += " (ZM)";
        if (hasAuthority)
        {
            energy = Random.Range(45f, 55f);
            energyUpgradeTime = energyUpgradeReset;
            energyRechargeRate = 3f;

            //Get all floors in level
            floors = GameObject.FindGameObjectsWithTag("Floor");
            Debug.Log("Floors : " + floors.Length);
            //Start on floor 1
            floor = 1;

            ChangeFloor();
        }
    }

    void Update()
    {
        if (hasAuthority == false)
        {
            return;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ////////INPUT
        //Left Mouse Click / Spawn Zombie
        if (Input.GetKeyDown(KeyCode.Mouse0) && !hover)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f , ~ignoreOnRaycast))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    SpawnZombie(hit);
                }
                else if (hit.collider.CompareTag("Icon"))
                {
                    UseIcon(hit);
                }
                else
                {
                    SetSpawnText(spawnText.text = "Cannot spawn unit here");
                }
            }
        }
        //Right Mouse Click / Alternate Interact
        if (Input.GetKeyDown(KeyCode.Mouse1) && !hover)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f, ~ignoreOnRaycast))
            {
                if (hit.collider.CompareTag("Icon"))
                {
                    UseIconAlt(hit);
                }
                else if (hit.collider.CompareTag("Zombie"))
                {
                    RefundZombie(hit);
                }
                else
                {
                    //reset unit selection
                    if (unit != null)
                    {
                        //unit.chosen = false;
                        unit = null;
                    }
                }
            }
        }

        //Floor navigation
        if (Input.GetKeyDown(floorUp))
        {
            floor = Mathf.Clamp(floor += 1, 1, floors.Length);
            ChangeFloor();
        }
        if (Input.GetKeyDown(floorDown))
        {
            floor = Mathf.Clamp(floor -= 1, 1, floors.Length);
            ChangeFloor();
        }

        //-----UI-----
        energy = Mathf.Clamp(energy += Time.deltaTime * energyRechargeRate,0f,maxEnergy);
        energyText.text = energy.ToString("N0") + "/" + maxEnergy;
        energySlider.value = energy;
        xpText.text = xp.ToString("N0") + "xp";

        energyUseSlider.gameObject.SetActive(unit);
        if (energyUseSlider.gameObject.activeSelf)
        {
            //energyUseSlider.value = unit.energyUse;
        }

        //-----Energy Upgrade-----
        energyUpgradeTime -= Time.deltaTime;
        if (energyUpgradeTime <= 0)
        {
            MaxEnergyButton.SetActive(true);
            RechargeRateButton.SetActive(true);
        }


        //-----CHEATS-----
        if (Input.GetKeyDown(xpCheatButton) && cheatsOn)
        {
            Debug.Log("CHEAT : XP");
            xp += 100;
        }
        if (Input.GetKeyDown(energyCheatButton) && cheatsOn)
        {
            Debug.Log("CHEAT : ENERGY");
            energy = maxEnergy;
        }
        if (Input.GetKeyDown(maxEnergyCheatButton) && cheatsOn)
        {
            Debug.Log("CHEAT : MAX ENERGY");
            maxEnergy += 50;
            energySlider.maxValue = maxEnergy;
        }
    }

    private void FixedUpdate()
    {
        if (hasAuthority == false) { return; }
        //Movement
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        transform.Translate(x, 0, z);

        //Mouse Scroll / Camera Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * 5, 10, 20);
        }
    }

    void ChangeFloor()
    {
        /*
        for (int i = 0; i < floors.Length; i++)
        {
            if (floors[i].name != "Floor" + floor) { floors[i].SetActive(false); }
            else { floors[i].SetActive(true); }
        }
        */
        cam.transform.position = new Vector3(cam.transform.position.x, positionChange * floor - 0.05f, cam.transform.position.z);
    }

    public void MouseOverButton(bool hover)
    {
        this.hover = hover;
    }

    public void Upgrade_Zombies(int which)
    {
        CmdUpgradeZombie(which);
        CmdUpgradeSound();
    }

    public void UnlockNewUnit()
    {
        CmdUnlockSound();
    }

    void SpawnTextReset()
    {
        spawnText.text = "";
        spawnText.gameObject.SetActive(false);
    }

    void SetSpawnText(string newText)
    {
        spawnText.text = newText;
        spawnText.gameObject.SetActive(true);
        Invoke("SpawnTextReset", 1f);
    }

    /*
    [Command]
    private void Cmd_TellToSmash(GameObject stronk, GameObject wall)
    {
        Rpc_TellToSmash(stronk,wall);
    }
    [ClientRpc]
    private void Rpc_TellToSmash(GameObject stronk, GameObject wall)
    {
        stronk.GetComponent<Stronk>().Smash(wall);
    }
    */

    void UseIcon(RaycastHit hit)
    {
        ObjectIcon icon = hit.collider.gameObject.GetComponent<ObjectIcon>();
        bool can = icon.InteractWithObject(this);
        if (can)
        {
            if (icon.isDoor)
            {
                CmdLockDoor(icon.door);
            }
            else if (icon.isWall)
            {
                GameObject[] stronks = GameObject.FindGameObjectsWithTag("Zombie_Stronk");
                foreach (GameObject s in stronks)
                {
                    if (Vector3.Distance(icon.wall.transform.position, s.transform.parent.position) < 20)
                    {
                        //Cmd_TellToSmash(s.transform.parent.gameObject, icon.wall);
                    }
                }
            }
        }
        else
        {
            SetSpawnText("Cannot use " + (icon.isDoor ? "Door" : "Wall"));
        }
    }

    void UseIconAlt(RaycastHit hit)
    {
        ObjectIcon icon = hit.collider.gameObject.GetComponent<ObjectIcon>();
        bool can = icon.AltInteractWithObject(this);
        if (can)
        {
            if (icon.isDoor)
            {
                CmdUnlockDoor(icon.door);
            }
        }
        else
        {
            if (icon.isDoor)
            {
                SetSpawnText("Cannot use Door");
            }
        }
    }

    void SpawnZombie(RaycastHit hit)
    {
        if (unit != null)
        {
            bool canSpawn = true;
            int curAmount = 0;
            int totalAmount = 0;
            //switch (unit.unitNum)
            //{
            //    case 1:
            //        if (maxAmount_Zombie != 0)
            //        {
            //            curAmount = GameObject.FindGameObjectsWithTag("Zombie_Default").Length;
            //            if (curAmount >= maxAmount_Zombie)
            //            {
            //                SetSpawnText("Max amount reached");
            //                canSpawn = false;
            //            }

            //        }
            //        break;
            //    case 2:
            //        if (maxAmount_Slav != 0)
            //        {
            //            curAmount = GameObject.FindGameObjectsWithTag("Zombie_Slav").Length;
            //            if (curAmount >= maxAmount_Slav)
            //            {
            //                SetSpawnText("Max amount reached");
            //                canSpawn = false;
            //            }
            //        }
            //        break;
            //    case 3:
            //        if (maxAmount_Spitter != 0)
            //        {
            //            curAmount = GameObject.FindGameObjectsWithTag("Zombie_Spitter").Length;
            //            if (curAmount >= maxAmount_Spitter)
            //            {
            //                SetSpawnText("Max amount reached");
            //                canSpawn = false;
            //            }
            //        }
            //        break;
            //    case 4:
            //        if (maxAmount_Stronk != 0)
            //        {
            //            curAmount = GameObject.FindGameObjectsWithTag("Zombie_Stronk").Length;
            //            if (curAmount >= maxAmount_Stronk)
            //            {
            //                SetSpawnText("Max amount reached");
            //                canSpawn = false;
            //            }
            //        }
            //        break;
            //    case 5:
            //        if (maxAmount_Tentacle != 0)
            //        {
            //            curAmount = GameObject.FindGameObjectsWithTag("Zombie_Tentacle").Length;
            //            if (curAmount >= maxAmount_Tentacle)
            //            {
            //                SetSpawnText("Max amount reached");
            //                canSpawn = false;
            //            }
            //        }
            //        break;
            //}
            totalAmount += GameObject.FindGameObjectsWithTag("Zombie_Default").Length;
            totalAmount += GameObject.FindGameObjectsWithTag("Zombie_Slav").Length;
            totalAmount += GameObject.FindGameObjectsWithTag("Zombie_Spitter").Length;
            totalAmount += GameObject.FindGameObjectsWithTag("Zombie_Stronk").Length;
            totalAmount += GameObject.FindGameObjectsWithTag("Zombie_Tentacle").Length;
            if (totalAmount >= totalMaxAmountOfZombies)
            {
                SetSpawnText("Max amount of zombies reached");
                canSpawn = false;
            }
            if (canSpawn)
            {/*
                if (energy >= unit.energyUse)
                {
                    canSpawn = true;
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    RaycastHit newhit;
                    Vector3 pos = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
                    foreach (GameObject p in players)
                    {
                        Vector3 pPos = new Vector3(p.transform.position.x, p.transform.position.y + 2, p.transform.position.z);
                        Vector3 dir = pPos - pos;
                        Debug.DrawRay(pos, dir * 10, Color.red, 100f);
                        if (Physics.Raycast(pos, dir, out newhit, 100f))
                        {
                            dir = pos - pPos;
                            float angle = Vector3.Angle(dir, p.transform.forward);
                            //can zombie see the player
                            if (newhit.collider.CompareTag("Player"))
                            {
                                //is he inside the fov of the player
                                if (angle < 60)
                                {
                                    canSpawn = false;
                                    SetSpawnText("Must spawn out of view of players");
                                }
                                //is the zombie far enough away
                                if (Vector3.Distance(pos, newhit.collider.transform.position) <= 5f)
                                {
                                    canSpawn = false;
                                    SetSpawnText("Must spawn further away from players");
                                }
                            }
                        }
                    }
                    if (canSpawn)
                    {
                        SpawnSmoke(hit.point);
                        //Lol
                        //string name = unit.unitName;
                        if (name == "Zombie")
                        {
                            name += (Random.value > 0.5f) ? "_v1" : "_v2";
                            CmdSpawnMyUnit(hit.point, name);
                        }
                        else
                        {
                            CmdSpawnMyUnit(hit.point, name);
                        }
                        //energy -= unit.energyUse;
                        //xp += unit.energyUse;
                    }
                }
                else
                {
                    SetSpawnText("Not enough energy");
                }
                */
            }
        }
    }

    void RefundZombie(RaycastHit hit)
    {
        /*
        var enemyH = hit.collider.gameObject.GetComponent<EnemyHealth>();
        if (enemyH.health == enemyH.maxHealth)
        {
            string type = hit.collider.transform.Find("Zombie Type Tracker").tag;
            SpawnSmoke(hit.point);

            switch (type)
            {
                case "Zombie_Default":
                    CmdRefundZombie(hit.collider.gameObject, zombieRefund);
                    break;
                case "Zombie_Slav":
                    CmdRefundZombie(hit.collider.gameObject, slavRefund);
                    break;
                case "Zombie_Spitter":
                    CmdRefundZombie(hit.collider.gameObject, spitterRefund);
                    break;
                case "Zombie_Stronk":
                    CmdRefundZombie(hit.collider.gameObject, stronkRefund);
                    break;
                case "Zombie_Tentacle":
                    CmdRefundZombie(hit.collider.gameObject, tentacleRefund);
                    break;
            }
        }
        else
        {
            SetSpawnText("Zombie has been damaged");
        }
        */
    }

    public void UpgradeEnergy(bool rate)
    {
        MaxEnergyButton.SetActive(false);
        RechargeRateButton.SetActive(false);
        CmdUpgradeSound();
        energyUpgradeTime = energyUpgradeReset;

        if (rate)
        {
            energyRechargeRate *= 1.25f;
        }
        else
        {
            maxEnergy += maxEnergyIncrease;
            energySlider.maxValue = maxEnergy;
        }
    }

    private void SpawnSmoke(Vector3 point)
    {
        GameObject smoke = (GameObject)Resources.Load("Spawnables/FX/ZombieSmoke");
        var s = Instantiate(smoke, point, Quaternion.identity);
        s.transform.parent = null;
        Destroy(s, 1f);
    }

    ///////////COMMANDS
    
    [Command]
    void CmdSpawnMyUnit(Vector3 pos, string name)
    {
        pos = new Vector3(pos.x, pos.y + 1, pos.z);
        GameObject[] newUnit = Resources.LoadAll("Spawnables/Zombies").Cast<GameObject>().ToArray();
        List<GameObject> zombies = new List<GameObject>();
        foreach (GameObject z in newUnit)
        {
            if (z.name.Contains(name))
            {
                zombies.Add(z);
            }
        }
        GameObject zUnit = zombies[Random.Range(0, zombies.Count)];
        GameObject zomb = Instantiate(zUnit, pos, zUnit.transform.rotation);
        zomb.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        NetworkServer.Spawn(zomb);
    }
    [Command]
    void CmdUpgradeZombie(int which)
    {
        switch (which)
        {
            case 1:
                _levelZombie++;
                break;
            case 2:
                _levelSlav++;
                break;
            case 3:
                _levelSpitter++;
                break;
            case 4:
                _levelStronk++;
                break;
            case 5:
                _levelTentacle++;
                break;
        }
    }
    [Command]
    void CmdUpgradeSound()
    {
        GameObject sound = (GameObject)Resources.Load("Spawnables/FX/ZombieMaster_UpgradeSound");
        var s = Instantiate(sound);
        NetworkServer.Spawn(s);
    }
    [Command]
    void CmdUnlockSound()
    {
        GameObject sound = (GameObject)Resources.Load("Spawnables/FX/ZombieMaster_UnlockSound");
        var s = Instantiate(sound);
        NetworkServer.Spawn(s);
    }
    [Command]
    void CmdLockDoor(GameObject door)
    {
        door.GetComponent<Door>().RpcLockDoor(false);
    }
    [Command]
    void CmdUnlockDoor(GameObject door)
    {
        door.GetComponent<Door>().RpcLockDoor(true);
    }
    [Command]
    void CmdRefundZombie(GameObject zom, float refund)
    {
        Debug.Log("Refunded " + zom.name + " for " + refund + " energy");
        energy = Mathf.Clamp(energy += refund,0f,maxEnergy);
        NetworkServer.Destroy(zom);
    }
}