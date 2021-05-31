using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.InputSystem;

public class SurvivorSelector : NetworkBehaviour
{

    [SerializeField] private bool canSelect = false;
    private SurvivorSelectHighlight highlight;

    //private void Start()
    //{
    //    if (!hasAuthority) return;

    //    highlight = SurvivorSelectHighlight.instance;

    //    CanSelect = true;
    //}
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        highlight = SurvivorSelectHighlight.instance;

        CanSelect = true;
    }

    public bool CanSelect
    {
        get { return canSelect; }
        set
        {
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
            if (highlight == null) highlight = SurvivorSelectHighlight.instance;

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
        if (!canSelect) return;
        bool hasSelected = GetComponent<LobbyPlayer>().hasSelectedACharacter;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out SurvivorSelect select))
            { 
                Cmd_SelectSurvivor(select.gameObject, netId, hasSelected);
            }

        }
    }

    [Command]
    private void Cmd_SelectSurvivor(GameObject select, uint id, bool hasSelected)
    {
        select.GetComponent<SurvivorSelect>().Rpc_SelectSurvivor((int)id, hasSelected);
    }

    public void ChangeCharacter(string characterName)
    {
        if (!hasAuthority) return;

        Cmd_ChangeCharacter(characterName);
        PlayerPrefs.SetString("Survivor", characterName);
    }

    [Command]
    private void Cmd_ChangeCharacter(string characterName)
    {
        GetComponent<LobbyCharacters>().Rpc_ChangeCharacter(characterName);
    }

}
