using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
    public bool pickUp = false;
    public GameObject weaponPrefab = null;
	public new string name = null;
    public Transform sightsPointRef = null;
    public GameObject muzzleFlash = null;
    public Sprite crossHair = null;
    public ParticleSystem smoke = null;
    public Type weaponType = Type.none;
    public Transform rightHandTarget = null;
    public Transform leftHandTarget = null;

    public enum Type
    {
        pistol,
        shotgun,
        rifle,
        melee,
        knife,
        none
    }
}
