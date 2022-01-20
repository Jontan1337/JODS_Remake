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
    private Coroutine COCreateOutlines;

    public Transform PlayerCamera
    {
        get => playerCamera;
        private set
        {
            playerCamera = value;
            // Start the coroutine that outlines interactable objects.
            if (COCreateOutlines == null)
            {
                COCreateOutlines = StartCoroutine(CreateOutlines());
            }
        }
    }

    private void Awake()
    {
        transform.root.GetComponent<SurvivorSetup>().onClientSpawnItem += GetCamera;
    }

    #region NetworkCallbacks
    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Interact.performed += ctx => Interact();
    }

    public override void OnStopAuthority()
    {
        transform.root.GetComponent<SurvivorSetup>().onClientSpawnItem -= GetCamera;
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
            boxHit = Physics.BoxCastAll(
                new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z) + transform.forward,
                new Vector3(0.45f, 0.45f, interactionRange / 2),
                transform.forward, playerCamera.rotation, interactionRange, ~layerMask
            );

            if (rayHit.collider)
            {
                if (rayHit.collider.TryGetComponent(out Outline outline))
                {
                    outline.ShowOutline(0.1f, interactionOutline);
                }
            }

            foreach (var item in boxHit)
            {
                if (rayHit.collider)
                    if (item.collider == rayHit.collider)
                        continue;

                if (item.collider.TryGetComponent(out Outline outline))
                {
                    outline.ShowOutline(0.1f, inRangeOutline);
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
            // Check if the interacted object is networked.
            if (!rayHit.collider.GetComponent<NetworkIdentity>()) return;

            GameObject rayHitObject = rayHit.collider.gameObject;
            if (rayHitObject.TryGetComponent(out IInteractable interactable))
            {
                if (!interactable.IsInteractable) return;

                Cmd_Interact(rayHit.collider.gameObject);
            }

        }
    }

    [Command]
    public void Cmd_Interact(GameObject targetObject)
    {
        if (targetObject)
        {
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
                    PlayerCamera = item.transform;
                    break;
                default:
                    break;
            }
        }
    }
}
