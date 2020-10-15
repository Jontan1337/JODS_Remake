using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearTrap : MonoBehaviour
{
    [Header("Trap Parts")]
    [SerializeField] private GameObject open = null;
    [SerializeField] private GameObject closed = null;

    [Header("Stats")]
    [SerializeField] private float durability = 10F;
    [SerializeField] private int damage = 50;

    private UnitBase zombie;




    private void Start()
    {
        open.SetActive(true);
        closed.SetActive(false);
    }

    //If the beartrap collides with a zombie, it closes & damages the zombie
    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Zombie"))
        {
            open.SetActive(false);
            closed.SetActive(true);
            //other.gameObject.GetComponent<ZBodyParts>().Trap(durability);
            zombie = other.gameObject.GetComponent<UnitBase>();
            other.gameObject.GetComponent<EnemyHealth>().Svr_Damage(damage);
            GetComponent<SphereCollider>().enabled = false;
            Trap();
        }
        */
    }

    //Here is traps the zombie in place
    public void Trap()
    {
        GetComponent<SFXPlayer>().PlaySFX();
        zombie.Trap();
        if (zombie) { zombie.navAgent.speed = 0; }
        Invoke("Remove", durability);
    }

    //Destroys itself
    public void Remove()
    {
        /*
        zombie.Trap();
        if (zombie) { zombie.navAgent.speed = zombie.speed; }
        Destroy(gameObject);
        */
    }
}
