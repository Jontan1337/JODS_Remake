using UnityEngine;
using Mirror;

public class MenuHandler : NetworkBehaviour
{
    public Camera playerCamera;
    public GameObject playerMenu;
    public GameObject playerUI;
    public KeyCode menuToggleButton;
    [Header("Submenus")]
    public GameObject options;

    private Interactor interact;
    [Space]
    [SerializeField] private HandSway handSway = null;

    private bool isOpen;
    private bool inSubMenu;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    private void Start()
    {
        interact = GetComponent<Interactor>();
    }

    private void Update()
    {
        if (!hasAuthority)
            return;

        if (Input.GetKeyDown(menuToggleButton))
        {
            if (inSubMenu)
            {
                options.SetActive(false);

                playerMenu.SetActive(true);
                inSubMenu = false;
            }
            else
            {
                ToggleMenu();
            }
        }
    }

    public void ToggleMenu()
    {
        if (!hasAuthority)
            return;

        isOpen = !IsOpen;
        if (IsOpen)
        {
            // Unlock the cursor and make it visible to use the menu.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Lock the cursor and make it invisible to control player camera.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        playerUI.SetActive(!IsOpen);
        playerMenu.SetActive(IsOpen);

        if (interact)
        {
            interact.enabled = !IsOpen;
        }
        if (handSway)
        {
            //handSway.enabled = !IsOpen;
        }
    }

    public void ToggleSubMenu()
    {
        inSubMenu = true;
    }
}
