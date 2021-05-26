using Mirror;
using System;
using System.Collections;
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
        StartCoroutine(CreateOutlines());
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

    RaycastHit[] boxHit = new RaycastHit[0];
    private IEnumerator CreateOutlines()
    {
        while (true)
        {
            print(playerCamera);
            if (!playerCamera) yield return null;

            boxHit = Physics.BoxCastAll(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z) + transform.forward * interactionRange,
                new Vector3(0.45f, 0.45f, interactionRange / 2),
                transform.forward, playerCamera.rotation, interactionRange, ~layerMask);

            if (rayHit.collider.TryGetComponent(out Outline outline))
            {
                outline.ShowOutline(0.1f, 10f);
            }

            foreach (var item in boxHit)
            {
                if (rayHit.collider)
                    if (item.collider == rayHit.collider)
                        continue;

                if (item.collider.TryGetComponent(out Outline outline2))
                {
                    outline2.ShowOutline(0.1f, 3f);
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z) + playerCamera.forward * interactionRange / 2, new Vector3(0.9f, 0.9f, interactionRange));
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
