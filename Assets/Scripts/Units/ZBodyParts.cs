using UnityEngine;

public class ZBodyParts : MonoBehaviour, IExplodable
{
    public UnitBase zombie;
    public bool head, body, limb;

    /// <summary>
    /// Bool damagepart should be true for weapons and not explosions
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="damagePart"></param>

    //public void Damage(int dmg, bool damagePart)
    //{
    //    Debug.LogError("Enemy Take Damage(" + dmg + " | " + damagePart + ")");
    //    if (damagePart)
    //    {
    //        if (head) { zombie.gameObject.GetComponent<IDamagable>().Damage(dmg * 2); }
    //        else if (body) { zombie.gameObject.GetComponent<IDamagable>().Damage(dmg); }
    //        else if (limb) { zombie.gameObject.GetComponent<IDamagable>().Damage (dmg / 2); }
    //        else { zombie.gameObject.GetComponent<IDamagable>().Damage(dmg / 4); }
            
    //        /*
    //        if (head) { zombie.Damage(dmg * 2); }
    //        else if (body) { zombie.Damage(dmg); }
    //        else if (limb) { zombie.Damage(dmg / 2); }
    //        else { zombie.Damage(dmg / 4); }
    //        */
    //    }
    //    else
    //    { 

    //      //  zombie.Damage(dmg);
    //    }
    //}

    public void Explode(Transform explosionSource = null)
    {
        /*
        if (transform.root.GetComponent<EnemyHealth>().health <= 0)
        {
            transform.localScale = new Vector3(0f, -0.3f, 0f);
        }
        */
    }
}
