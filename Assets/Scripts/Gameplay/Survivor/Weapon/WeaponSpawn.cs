using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class WeaponSpawn : MonoBehaviour
{
    public GameObject[] obj;
    public GameObject ammo;
    public GameObject armour;
    public GameObject currentWeapon;
    [Header("Other")]
    public bool rare;
    public int tier = 1;
    [Header("Testing")]
    public bool spawnOnceOnStart;
    public bool onlySpawnWeapon;
    bool isAmmo;
    bool isArmour;

    private void Start()
    {
        if (spawnOnceOnStart)
        {
            Debug.Log(name + "Is on SpawnOnceOnStart");
            switch (tier)
            {
                case 1:
                    isAmmo = Random.value > 0.8f;
                    if (!isAmmo)
                    {
                        isArmour = Random.value > 0.85f;
                    }
                    break;
                case 2:
                    isAmmo = Random.value > 0.7f;
                    if (!isAmmo)
                    {
                        isArmour = Random.value > 0.85f;
                    }
                    break;
                case 3:
                    isAmmo = Random.value > 0.6f;
                    if (!isAmmo)
                    {
                        isArmour = Random.value > 0.85f;
                    }
                    break;
            }
            if (isAmmo && ammo && !onlySpawnWeapon)
            {
                GameObject inst = Instantiate(ammo, transform.position, Quaternion.identity);
                inst.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                inst.transform.parent = transform.parent;
                //inst.GetComponent<Ammo>().tier = tier;
                //Debug.Log(name + " - Spawning ammo");
                NetworkServer.Spawn(inst);
                NetworkServer.Destroy(gameObject);
            }
            else if (isArmour && armour && !onlySpawnWeapon)
            {
                GameObject inst = Instantiate(armour, transform.position, Quaternion.identity);
                inst.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                inst.transform.parent = transform.parent;
                //Debug.Log(name + " - Spawning armour");
                NetworkServer.Spawn(inst);
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                if (obj.Length != 0)
                {
                    GameObject inst = Instantiate(obj[Random.Range(0, obj.Length)], transform.position, Quaternion.identity);
                    inst.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                    inst.transform.parent = transform.parent;
                    //Debug.Log(name + " - Spawning new weapon from tier " + tier + " (" + inst.name + ")");
                    NetworkServer.Spawn(inst);
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }
    public void SpawnWeapon(bool makeCurrent)
    {
        if (currentWeapon)
        {
            Destroy(currentWeapon);
        }

        switch (tier)
        {
            case 1:
                isAmmo = Random.value > 0.8f;
                if (!isAmmo)
                {
                    isArmour = Random.value > 0.85f;
                }
                break;
            case 2:
                isAmmo = Random.value > 0.7f;
                if (!isAmmo)
                {
                    isArmour = Random.value > 0.85f;
                }
                break;
            case 3:
                isAmmo = Random.value > 0.6f;
                if (!isAmmo)
                {
                    isArmour = Random.value > 0.85f;
                }
                break;
        }
        if (isAmmo && ammo && !onlySpawnWeapon)
        {
            GameObject inst = Instantiate(ammo, transform.position, Quaternion.identity);
            inst.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
            inst.transform.parent = transform.parent;
            //inst.GetComponent<Ammo>().tier = tier;
            currentWeapon = makeCurrent ? inst : null;
            NetworkServer.Spawn(inst);
        }
        else if (isArmour && armour && !onlySpawnWeapon)
        {
            GameObject inst = Instantiate(armour, transform.position, Quaternion.identity);
            inst.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
            inst.transform.parent = transform.parent;
            //Debug.Log(name + " - Spawning armour");
            currentWeapon = makeCurrent ? inst : null;
            NetworkServer.Spawn(inst);
        }
        else
        {
            if (obj.Length != 0)
            {
                GameObject inst = Instantiate(obj[Random.Range(0, obj.Length)], transform.position, Quaternion.identity);
                inst.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                inst.transform.parent = transform.parent;
                currentWeapon = makeCurrent ? inst : null;
                NetworkServer.Spawn(inst);
            }
        }
    }
}
