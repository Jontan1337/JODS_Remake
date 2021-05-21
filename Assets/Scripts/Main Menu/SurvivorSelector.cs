using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.InputSystem;

public class SurvivorSelector : NetworkBehaviour
{
    #region Singleton
    public static SurvivorSelector instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private bool canSelect = false;
    private SurvivorSelectHighlight highlight;

    private void Start()
    {
        highlight = SurvivorSelectHighlight.instance;

        CanSelect = true;
    }

    public bool CanSelect
    {
        get { return canSelect; }
        set {
            canSelect = value;
            if (value == true)
            {
                JODSInput.Controls.MainMenu.LMB.performed += SelectSurvivor;
                RaycastCoroutine = StartCoroutine(SelectCoroutine());
            }
            else if (value == false)
            {
                JODSInput.Controls.MainMenu.LMB.performed -= SelectSurvivor;
                StopCoroutine(RaycastCoroutine);
            }
        }
    }

    private Coroutine RaycastCoroutine;
    private IEnumerator SelectCoroutine()
    {
        while (canSelect)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out SurvivorSelect select))
                {
                    highlight.Highlight(select.survivor);
                }
                else
                {
                    highlight.Highlight(null);
                }
            }
            else
            {
                highlight.Highlight(null);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void SelectSurvivor(InputAction.CallbackContext context)
    {
        Cmd_SelectSurvivor();
    }

    [Command]
    private void Cmd_SelectSurvivor()
    {
        if (!canSelect) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out SurvivorSelect select))
            {
                select.Svr_Select(1);
            }
        }
    }

}
