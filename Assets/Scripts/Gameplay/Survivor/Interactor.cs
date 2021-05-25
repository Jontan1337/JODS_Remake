using Mirror;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : NetworkBehaviour
{
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float interactionRange = 2f;
    [SerializeField]
    private LayerMask layerMask = 15;
    [SerializeField]
    private float inRangeOutline = 3f;
    [SerializeField]
    private float interactionOutline = 10f;

    private RaycastHit rayHit;
    private IInteractable currentInteractable;

    private void Awake()
    {
        transform.root.GetComponent<PlayerSetup>().onSpawnItem += GetCamera;
    }

    #region NetworkCallbacks
    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Interact.performed += ctx => Interact();
    }

    public override void OnStopAuthority()
    {
        transform.root.GetComponent<PlayerSetup>().onSpawnItem -= GetCamera;
        JODSInput.Controls.Survivor.Interact.performed -= ctx => Interact();
    }
    #endregion

    private void Update()
    {
        if (!hasAuthority) return;
        if (!playerCamera) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        Physics.Raycast(ray, out rayHit, interactionRange, ~layerMask);
        UnityEngine.Debug.DrawRay(playerCamera.position, playerCamera.forward * interactionRange);
    }

    RaycastHit[] boxHit = new RaycastHit[999];
    RaycastHit[] previousItems = new RaycastHit[999];
    private void FixedUpdate()
    {
        boxHit = Physics.BoxCastAll(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z) + transform.forward * interactionRange,
            new Vector3(0.45f, 0.45f, interactionRange / 2),
            transform.forward, transform.rotation, interactionRange, ~layerMask);

        for (int i = 0; i < boxHit.Length; i++)
        {
            if (boxHit[i].collider.TryGetComponent(out Outline outline))
            {
                for (int x = 0; x < previousItems.Length; x++)
                {
                    if (previousItems[i].collider == boxHit[i].collider)
                    {
                        // TODO: Check if previous found item is no longer found, then turn off outline.
                    }
                }
                previousItems[i] = boxHit[i];
                outline.OutlineWidth = 3;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z) + transform.forward * interactionRange / 2, new Vector3(0.9f, 0.9f, interactionRange));
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), transform.forward * interactionRange);
    }

    public void Interact()
    {
        if (rayHit.collider)
        {
            foreach (var item in boxHit)
            {
                UnityEngine.Debug.Log(item.collider.name);
            }
            // Check if the interacted object is networked.
            if (!rayHit.collider.GetComponent<NetworkIdentity>()) return;

            UnityEngine.Debug.Log($"Client: interact with {rayHit.collider}", this);
            Cmd_Interact(rayHit.collider.gameObject);
        }
    }

    [Command]
    public void Cmd_Interact(GameObject targetObject)
    {
        if (targetObject)
        {
            UnityEngine.Debug.Log($"Server: interact with {targetObject}", this);
            targetObject.TryGetComponent(out IInteractable interactable);
            interactable?.Svr_Interact(gameObject);
        }
    }

    private void GetCamera(GameObject item)
    {
        if (item.TryGetComponent(out ItemName itemName))
        {
            switch (itemName.itemName)
            {
                case ItemNames.Camera:
                    playerCamera = item.transform;
                    break;
                default:
                    break;
            }
        }
    }
}
