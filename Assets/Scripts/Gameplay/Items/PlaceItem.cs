﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceItem : NetworkBehaviour
{
    public LookController look;
    public GameObject placeHolderPrefab;
    private GameObject placeHolder;
    private bool placeholderActive;

    private void Start()
    {
        look = GetComponentInParent<LookController>();
    }

    public IEnumerator PlaceHolderActive()
    {
        RaycastHit hit;
        Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out hit, 5f, 13);
        placeHolder = Instantiate(placeHolderPrefab, hit.point, transform.rotation);
        while (true)
        {
            Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out hit, 5f, 13);
            placeHolder.transform.position = hit.point;

            yield return new WaitForSeconds(0.1f);
        }

        placeholderActive = false;
    }

    public void Place(GameObject thing, GameObject self)
    {
        RaycastHit hit;
        if (Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out hit, 5f, 13))
        {
            print(hit.transform.name);
            CmdPlaceItem(thing.name, placeHolder.transform.position, placeHolder.transform.rotation);
            //CmdPlaceItem(thing.name, hit.point, self.transform.rotation);
        }
    }

    void CmdPlaceItem(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject newItem = Instantiate(Resources.Load<GameObject>($"Spawnables/{prefabName}"), position, rotation);
        NetworkServer.Spawn(newItem);
    }

}