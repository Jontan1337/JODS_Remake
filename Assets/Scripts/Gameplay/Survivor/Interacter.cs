using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Interacter : NetworkBehaviour
{
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float interactionRange = 2f;

    private IInteractable currentInteractable;

    public override void OnStartAuthority()
    {
        if (!hasAuthority) return;

        Debug.Log("on start authority");
        JODSInput.Controls.Survivor.Interact.performed += ctx => Interact();
    }

    public override void OnStopAuthority()
    {
        if (!hasAuthority) return;

        Debug.Log("on stop authority");
        JODSInput.Controls.Survivor.Interact.performed -= ctx => Interact();
    }

    private void Update()
    {
        if (!hasAuthority) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, interactionRange))
        {
            if (rayHit.collider.TryGetComponent(out IInteractable interactable))
            {
                currentInteractable = interactable;
            }
        }
    }

    [Command]
    public void Cmd_Interact()
    {
        Debug.Log("helo");
        currentInteractable?.Svr_Interact(gameObject);
    }
    public void Interact()
    {
        Debug.Log("helo");
    }
}
