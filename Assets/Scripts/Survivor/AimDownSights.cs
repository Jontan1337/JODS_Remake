using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDownSights : MonoBehaviour
{
    public Transform weaponAimHandlerParent = null;
    public Transform armPivotWeaponContainer = null;
    public Transform playerCamera = null;
    public Transform weaponSightsPointRef = null;
    public Transform originalWeaponSightsPointPos = null;
    public Quaternion tempQ = new Quaternion(0f, 0.08f, 0f, 1f);
    [SerializeField] private Shoot shootScript = null;
    [SerializeField] private float aimTime = 20f;
    private bool settingsSet = false;

    // TODO:
    // Set weaponAimHandlerParent to weaponSightsPointRef position, then set
    // the weaponAimHandlerParent as a parent to the weaponSightsPointRef and then
    // lerp the weaponAimHandlerParent to the ADSWeaponPoint.

    // TODO:
    // When stop aiming, lerp the weaponAimHandlerParent back to, the original 
    // weaponSightsPointRef start position, and 
    // Update is called once per frame
    void Update()
    {
        if (settingsSet)
        {
            if (shootScript.isZooming)
            {
                //Vector3 tempV = transform.localPosition + new Vector3(0.055f, 0.082f, 0.15f);
                Vector3 tempV = transform.localPosition + new Vector3(0f, 0.082f, 0.15f);
                weaponAimHandlerParent.localPosition = Vector3.Lerp(weaponAimHandlerParent.localPosition, new Vector3(0,0,0), Time.deltaTime * aimTime);
                weaponAimHandlerParent.localRotation = Quaternion.Lerp(weaponAimHandlerParent.localRotation, new Quaternion(0,0,0,1), Time.deltaTime * aimTime);
            }
            else
            {
                Vector3 tempV = transform.localPosition + new Vector3(0.3f, -0.1f, 0.3f);
                weaponAimHandlerParent.localPosition = Vector3.Lerp(weaponAimHandlerParent.localPosition, tempV, Time.deltaTime * aimTime);
            }
        }
    }

    public void SetAimSettings(Transform weaponSightsPoint)
    {
        originalWeaponSightsPointPos = weaponSightsPoint;
        Debug.Log(weaponSightsPoint.position);
        weaponAimHandlerParent.position = weaponSightsPoint.position;
        weaponAimHandlerParent.localRotation = weaponSightsPoint.localRotation;
        armPivotWeaponContainer.parent = weaponAimHandlerParent;
        settingsSet = true;
    }

    public void ResetAimSettings()
    {
        settingsSet = false;
    }
}
