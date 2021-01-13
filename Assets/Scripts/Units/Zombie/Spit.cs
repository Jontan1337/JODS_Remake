using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spit : MonoBehaviour
{
    [SerializeField] private int spitDamage = 10;

    private void OnTriggerEnter(Collider other)
    {
        //If the spitball collides with a gameobject with the tag "Player" then it damages it
        if (other.tag == "Player") { other.GetComponent<Stats>().Svr_Damage(Random.Range(spitDamage - 3, spitDamage + 4)); }
        //If it collides with something that isn't a zombie then it destroys itself
        if (other.gameObject.layer != 16 && other.gameObject.layer != 9) { NetworkServer.Destroy(gameObject); }
    }
}
