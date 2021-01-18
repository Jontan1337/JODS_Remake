using Mirror;
using UnityEngine;

public class Interacter : NetworkBehaviour
{
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float interactionRange = 2f;

    private IInteractable currentInteractable;
    private Controls control;

    private void Awake()
    {
        control = new Controls();
    }

    public override void OnStartAuthority()
    {
        print(hasAuthority);
        if (!hasAuthority) return;

        Debug.Log("on start authority");
        control.Survivor.Interact.performed += ctx => Interact();
    }

    public override void OnStopAuthority()
    {
        if (!hasAuthority) return;

        Debug.Log("on stop authority");
        control.Survivor.Interact.performed -= ctx => Interact();
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
