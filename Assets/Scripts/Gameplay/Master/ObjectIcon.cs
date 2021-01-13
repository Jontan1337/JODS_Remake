using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIcon : MonoBehaviour
{
    public float energyUse = 20f;
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
    }

    [Header("Door")]
    public bool isDoor;
    public GameObject door;

    [Header("Destructable Wall")]
    public bool isWall;
    public GameObject wall;

    public bool InteractWithObject(ZombieMaster zm)
    {
        if (zm.energy >= energyUse)
        {
            if (isDoor)
            {
                if (!door.GetComponent<Door>().locked)
                {
                    zm.energy -= energyUse;
                    NewColour(Color.red);
                    Invoke("ResetColour", 10f);
                    return true;
                }
                else {
                    Debug.Log("Door is already locked");
                    return false;
                }
            }
            else if (isWall)
            {
                NewColour(Color.red);
                Invoke("ResetColour", 1f);
                return true;
            }
            else { return false; }
        }
        else 
        {
            Debug.Log("Not enough energy to use door");
            return false; 
        }
    }
    public bool AltInteractWithObject(ZombieMaster zm)
    {
        if (zm.energy >= energyUse)
        {
            if (isDoor)
            {
                if (!door.GetComponent<Door>().locked)
                {
                    zm.energy -= energyUse;
                    NewColour(Color.red);
                    Invoke("ResetColour", 10f);
                    return true;
                }
                else {
                    Debug.Log("Door is already locked"); 
                    return false;
                }
            }
            else { return false; }
        }
        else {
            Debug.Log("Not enough energy to use door");
            return false; }
    }
    void NewColour(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
    }
    void ResetColour()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white);
    }
}
