using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SurviveTimer : NetworkBehaviour
{
    [Header("Timer")]
    [SyncVar] public bool gameStarted;
    [SyncVar] public float time;
    [SyncVar] public int tier = 1;
    public Text text;
    public GameObject textCanvas;

    [Header("Weapons")]
    public GameObject[] tier1;
    public GameObject[] tier2;
    public GameObject[] tier3;
    public GameObject[] spawns;
    private GameObject[] medSpawns;
    private GameObject[] barrelSpawns;
    public int startAmountOfWeapons = 25;
    public int startAmountOfMeds = 10;
    private int amountToSpawn = 6;
    private int medsAmountToSpawn = 10;
    public int amountOfBarrels = 15;
    public GameObject flashlightPickup;
    public GameObject barrel;

    [Header("Other")]
    public List<GameObject> players;
    private bool t1;
    private bool t2;
    private bool t3;

    private void Start()
    {
        if (isServer)
        {
            spawns = GameObject.FindGameObjectsWithTag("WeaponSpawn");
            medSpawns = GameObject.FindGameObjectsWithTag("MedSpawn");
            barrelSpawns = GameObject.FindGameObjectsWithTag("BarrelSpawn");

            tier = 1;
            t1 = true;

            //int debugWeapons = startAmountOfWeapons;
            //int debugMeds = startAmountOfMeds;


            //startAmountOfWeapons = spawns.Length / startAmountOfWeapons;
            //Debug.Log("Start amount of weapons : " + startAmountOfWeapons + " |    " + spawns.Length + " spawns / " + debugWeapons + " (startAmountOfWeapons)");
            //startAmountOfMeds = medSpawns.Length / startAmountOfMeds;
            //Debug.Log("Start amount of meds : " + startAmountOfMeds + " |    " + medSpawns.Length + " spawns / " + debugMeds + " (startAmountOfMeds)");
            //Debug.Log(startAmountOfWeapons + " / " + spawns.Length);
            amountToSpawn = startAmountOfWeapons;
            medsAmountToSpawn = startAmountOfMeds;

            SpawnBarrels();

            if (t1)
            {
                //Update all weapon spawns to current tier, then spawn some weapons
                UpdateWeapons();
                NewWeapons();

                //Spawn Meds
                NewMeds();

                //Increase amount of weapons
                MoreWeapons();

                //Increase amount of meds
                MoreMeds();

                //Spawn flashlights
                SpawnFlashlights(3); //Change to players - 1
            }
        }
    }

    void Update()
    {
        //TEMPORARY --- MAYBE?
        //Amount of players alive
        //if (!gameStarted)
        players.Clear();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(g);
        }

        //Timer / Tiers
        // 5 minutes
        if (time > 300f && !t2)
        {
            t2 = true;
            tier = 2;
            UpdateWeapons();
            SpawnNewWeapons(amountToSpawn);
        }
        // 15 minutes
        if (time > 900f && !t3)
        {
            t3 = true;
            tier = 3;
            UpdateWeapons();
            SpawnNewWeapons(amountToSpawn);
        }

        if (gameStarted && players.Count != 0)
        {
            textCanvas.SetActive(true);
            time += Time.deltaTime;
            float minutes = Mathf.Floor(time / 60);
            float seconds = (time % 60);
            text.text = "Time : " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        if (gameStarted)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
        if (gameStarted && players.Count == 0)
        {
            float minutes = Mathf.Floor(time / 60);
            float seconds = (time % 60);
            text.text = "Game Over. Survived : " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    void UpdateWeapons()
    {
        //Debug.Log("Updating weapon spawns");
        for (int i = 0; i < spawns.Length; i++)
        {
            if (spawns[i] != null)
            {
                WeaponSpawn s = spawns[i].GetComponent<WeaponSpawn>();
                s.tier = tier;
                if (s.rare)
                {
                    List<GameObject> objs = new List<GameObject>();
                    switch (tier)
                    {
                        case 1:
                            objs.AddRange(tier1);
                            objs.AddRange(tier2);
                            break;
                        case 2:
                            objs.AddRange(tier2);
                            objs.AddRange(tier3);
                            break;
                        case 3:
                            objs.AddRange(tier3);
                            break;
                    }
                    s.obj = objs.ToArray();
                }
                else
                {
                    List<GameObject> objs = new List<GameObject>();
                    switch (tier)
                    {
                        case 1:
                            objs.AddRange(tier1);
                            break;
                        case 2:
                            objs.AddRange(tier1);
                            objs.AddRange(tier2);
                            break;
                        case 3:
                            objs.AddRange(tier2);
                            objs.AddRange(tier3);
                            break;
                    }
                    s.obj = objs.ToArray();
                }
            }
        }
    }

    void SpawnNewWeapons(int amount)
    {
        List<int> spawn = new List<int>();
        for(int i = 0; i < spawns.Length; i++)
        {
            if (spawns[i] != null)
            {
                spawn.Add(i);
            }
        }
        for (int i = spawn.Count - 1; i > 0; i--)
        {
            //Randomize the list
            int rdm = Random.Range(0, i);

            int temp = spawn[i];
            spawn[i] = spawn[rdm];
            spawn[rdm] = temp;
        }
        //remove items in list until there is the amount needed
        //Debug.Log(spawns.Length - amount);
        spawn.RemoveRange(0, spawns.Length - amount);

        for(int i = 0; i < spawn.Count; i++)
        {
            if (spawns[i] != null)
            {
                WeaponSpawn s = spawns[spawn[i]].GetComponent<WeaponSpawn>();
                s.SpawnWeapon(true);
            }
        }
    }

    void SpawnNewMeds(int amount)
    {
        List<int> spawn = new List<int>();
        for (int i = 0; i < medSpawns.Length; i++)
        {
            spawn.Add(i);
        }
        for (int i = spawn.Count - 1; i > 0; i--)
        {
            //Randomize the list
            int rdm = Random.Range(0, i);

            int temp = spawn[i];
            spawn[i] = spawn[rdm];
            spawn[rdm] = temp;
        }
        //remove items in list until there is the amount needed
        spawn.RemoveRange(0, medSpawns.Length - amount);

        for (int i = 0; i < spawn.Count; i++)
        {
            MedSpawn med = medSpawns[spawn[i]].GetComponent<MedSpawn>();
            med.SpawnMed();
        }
    }

    private void NewWeapons()
    {
        SpawnNewWeapons(amountToSpawn);
        //Invoke("NewWeapons", Random.Range(100f, 180f));
        Invoke("NewWeapons", Random.Range(5f, 10f));
    }
    private void NewMeds()
    {
        SpawnNewMeds(medsAmountToSpawn);
        Invoke("NewMeds", Random.Range(120f, 200f));
    }

    void SpawnFlashlights(int amount)
    {
        List<int> spawn = new List<int>();
        for (int i = 0; i < spawns.Length; i++)
        {
            if (!spawns[i].GetComponent<WeaponSpawn>().rare) //Only spawn in places that are not rare, because usually they are in dark areas
            {
                spawn.Add(i);
                //Debug.Log("flashlight spawn");
            }

        }
        for (int i = spawn.Count - 1; i > 0; i--)
        {
            //Randomize the list
            int rdm = Random.Range(0, i);

            int temp = spawn[i];
            spawn[i] = spawn[rdm];
            spawn[rdm] = temp;
        }
        //remove items in list until there is the amount needed
        spawn.RemoveRange(0, spawn.Count - amount);
        /*
        for(int i = 0; i < spawn.Count; i++)
        {
            Debug.Log(i);
            Debug.Log(spawn[i]);
            Debug.Log("Flashlight spawns " + spawns[spawn[i]].name);
        }
        */
        for (int i = 0; i < spawn.Count; i++)
        {
            //Debug.Log("Spawning flashlight on " + spawns[spawn[i]].name);
            WeaponSpawn s = spawns[spawn[i]].GetComponent<WeaponSpawn>();
            s.obj = new GameObject[1];
            s.obj[0] = flashlightPickup;
            s.SpawnWeapon(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !gameStarted)
        {
            StartGame();
        }
    }

    private void MoreWeapons()
    {
        amountToSpawn += Random.Range(3, 8);
        if (amountToSpawn > spawns.Length)
        {
            amountToSpawn = spawns.Length;
        }
        else
        {
            //Make it loop, calling the method every minute or so
            Invoke("MoreWeapons", Random.Range(50f, 80f));
            //Invoke("MoreWeapons", Random.Range(5f, 10f));
        }
    }

    private void MoreMeds()
    {
        medsAmountToSpawn += Random.Range(1, 4);
        if (medsAmountToSpawn > medSpawns.Length / 4)
        {
            medsAmountToSpawn = medSpawns.Length / 4;
        }
        else
        {
            //Make it loop, calling the method every minute or so
            Invoke("MoreMeds", Random.Range(60f, 100f));
        }
    }

    public void StartGame()
    {
        Debug.Log("Game start : " + Time.time);
        players.Clear();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(g);
        }
        gameStarted = true;
        GetComponent<AudioSource>().Play();
        GetComponent<BoxCollider>().enabled = false;
    }

    private void SpawnBarrels()
    {
        List<int> spawn = new List<int>();
        for (int i = 0; i < barrelSpawns.Length; i++)
        {
            if (barrelSpawns[i] != null)
            {
                spawn.Add(i);
            }
        }
        for (int i = spawn.Count - 1; i > 0; i--)
        {
            //Randomize the list
            int rdm = Random.Range(0, i);

            int temp = spawn[i];
            spawn[i] = spawn[rdm];
            spawn[rdm] = temp;
        }
        //remove items in list until there is the amount needed
        //Debug.Log(spawns.Length - amount);
        spawn.RemoveRange(0, barrelSpawns.Length - amountOfBarrels);

        for (int i = 0; i < spawn.Count; i++)
        {
            if (barrelSpawns[spawn[i]] != null)
            {
                var inst = Instantiate(barrel, barrelSpawns[spawn[i]].transform);
                NetworkServer.Spawn(inst);
            }
        }
    }
}
