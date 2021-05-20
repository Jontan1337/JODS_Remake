using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Interacter : NetworkBehaviour
{
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float interactionRange = 2f;
    [SerializeField]
    private LayerMask layerMask = 15;

    private RaycastHit rayHit;
    private IInteractable currentInteractable;

    public override void OnStartServer()
    {
        transform.root.GetComponent<PlayerSetup>().onSpawnItem += GetCamera;
    }
    public override void OnStopServer()
    {
        transform.root.GetComponent<PlayerSetup>().onSpawnItem -= GetCamera;
    }

    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Interact.performed += ctx => Interact();
    }

    public override void OnStopAuthority()
    {
        JODSInput.Controls.Survivor.Interact.performed -= ctx => Interact();
    }

    private void Update()
    {
        if (!hasAuthority) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        Physics.Raycast(ray, out rayHit, interactionRange, ~layerMask);
        Debug.DrawRay(playerCamera.position, playerCamera.forward * interactionRange);
    }

    public void Interact()
    {
        if (rayHit.collider)
        {
            // Check if the interacted object is networked.
            if (!rayHit.collider.GetComponent<NetworkIdentity>()) return;

            Debug.Log($"Client: interact with {rayHit.collider}", this);
            Cmd_Interact(rayHit.collider.gameObject);
        }
    }

    [Command]
    public void Cmd_Interact(GameObject targetObject)
    {
        if (targetObject)
        {
            Debug.Log($"Server: interact with {targetObject}", this);
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
