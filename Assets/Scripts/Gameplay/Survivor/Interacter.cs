using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Interacter : NetworkBehaviour
{
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float interactionRange = 2f;

    private RaycastHit rayHit;
    private IInteractable currentInteractable;

    public override void OnStartAuthority()
    {
        if (!hasAuthority) return;

        Debug.Log("on start authority");
        JODSInput.Controls.Survivor.Interact.performed += ctx => Cmd_Interact();
    }

    public override void OnStopAuthority()
    {
        if (!hasAuthority) return;

        Debug.Log("on stop authority");
        JODSInput.Controls.Survivor.Interact.performed -= ctx => Cmd_Interact();
    }

    private void Update()
    {
        if (!hasAuthority) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        Physics.Raycast(ray, out rayHit, interactionRange);
        Debug.DrawRay(playerCamera.position, playerCamera.forward * interactionRange);
    }

    [Command]
    public void Cmd_Interact()
    {
        Debug.Log("helo");
        if (rayHit.collider)
        {
            rayHit.collider.TryGetComponent(out IInteractable interactable);
            interactable?.Svr_Interact(gameObject);
        }
    }
}
