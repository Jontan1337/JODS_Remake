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

    public override void OnStartAuthority()
    {
        Debug.Log("on start authority");

        JODSInput.Controls.Survivor.Interact.performed += ctx => Interact();

    }

    public override void OnStopAuthority()
    {
        Debug.Log("on stop authority");

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
            Cmd_Interact(rayHit.collider.gameObject);
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
}
