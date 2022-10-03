using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class ReviveManager : NetworkBehaviour, IInteractable
{
    [Title("UI References")]
    [SerializeField] private Image reviveTimerImageUI = null;
    [SerializeField] private GameObject reviveTimerObjectUI = null;
    [SerializeField] private Image downImage = null;
    [SerializeField] private GameObject downCanvas = null;
    [SerializeField] private GameObject inGameCanvas = null;

    [SerializeField] private float reviveTime = 5;
    [SerializeField] private float downTime = 30;

    private float reviveTimeCount = 0;
    private float downImageOpacity = 0;
    private bool beingRevived = false;


    private NetworkConnection connectionToClientInteractor;


    [Header("Events")]
    public UnityEvent onDownTimerFinished = null;
    public UnityEvent onRevived = null;


    [SerializeField, SyncVar] private bool isInteractable = false;
    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    private SurvivorStatManager characterStatManager;

    public override void OnStartClient()
    {
        if (isServer)
        {
            characterStatManager = GetComponent<SurvivorStatManager>();
            characterStatManager.onDownChanged.AddListener(delegate (bool isDown) { OnDownChanged(isDown); });
        }
    }

    private void OnDamaged()
    {
        downTime -= 0.1f;
        downImageOpacity += (1f / 30f) * 0.1f;
    }

    private void OnDownChanged(bool isDown)
    {
        if (isDown)
        {
            Rpc_DisableEverythingButMenu(connectionToClient);
        }
        else
        {
            Rpc_EnableEverythingButMenu(connectionToClient);
        }

        if (isDown)
        {
            characterStatManager.onDamaged.AddListener(delegate { OnDamaged(); });
            DownCo = Down();
            StartCoroutine(DownCo);
        }
        else
        {
            characterStatManager.onDamaged.RemoveListener(delegate { OnDamaged(); });
        }
    }

    IEnumerator DownCo;
    private IEnumerator Down()
    {
        Rpc_Down(connectionToClient);
        //animatorController.SetBool("IsDown", true);
        IsInteractable = true;
        downImageOpacity = 0;
        downImage.color = new Color(1f, 1f, 1f, 0f);

        while (downTime > 0)
        {
            if (!beingRevived)
            {
                Rpc_UpdateDownImage(connectionToClient, downImageOpacity);
                downImageOpacity += (1f / 30f);
                downTime -= 1;
            }
            yield return new WaitForSeconds(1f);
        }
        onDownTimerFinished?.Invoke();
    }

    [TargetRpc]
    private void Rpc_Down(NetworkConnection target)
    {
        JODSInput.DisableCamera();
        JODSInput.DisableHotbarControl();
        //animatorController.SetBool("IsDown", true);
        inGameCanvas.SetActive(false);
        downCanvas.SetActive(true);
    }

    [TargetRpc]
    private void Rpc_UpdateDownImage(NetworkConnection target, float downImageOpacity)
    {
        downImage.color = new Color(1f, 1f, 1f, downImageOpacity);
    }

    IEnumerator BeingRevivedCo;
    public IEnumerator BeingRevived()
    {
        beingRevived = true;
        while (reviveTime > 0)
        {
            reviveTime -= 1;
            yield return new WaitForSeconds(1f);
        }
        Revived();
    }

    private void Revived()
    {
        onRevived?.Invoke();

        //animatorController.SetBool("IsDown", false);
        downTime = 30;
        reviveTime = 5;
        StopCoroutine(DownCo);
        IsInteractable = false;
        Rpc_Revived(connectionToClient);
        beingRevived = false;
    }

    [TargetRpc]
    private void Rpc_Revived(NetworkConnection target)
    {
        inGameCanvas.SetActive(true);
        downCanvas.SetActive(false);
        JODSInput.EnableCamera();
        JODSInput.EnableHotbarControl();
        //animatorController.SetBool("IsDown", false);
    }


    IEnumerator ReviveTimerCo;
    private IEnumerator ReviveTimer()
    {
        reviveTimeCount = 0;
        reviveTimerObjectUI.SetActive(true);
        reviveTimerImageUI.fillAmount = 0;
        while (reviveTimeCount < 5)
        {
            reviveTimeCount += (Time.deltaTime);
            reviveTimerImageUI.fillAmount = reviveTimeCount / 5;
            yield return null;
        }
        reviveTimerObjectUI.SetActive(false);
    }

    [TargetRpc]
    private void Rpc_StartReviveTimer(NetworkConnection target)
    {
        ReviveTimerCo = ReviveTimer();
        StartCoroutine(ReviveTimerCo);
    }

    [TargetRpc]
    private void Rpc_ReviveTimerCancelled(NetworkConnection target)
    {
        StopCoroutine(ReviveTimerCo);
        reviveTimerObjectUI.SetActive(false);
    }

    private void ReviveCanceled()
    {
        StopCoroutine(BeingRevivedCo);

        beingRevived = false;
        reviveTime = 5;
    }

    [Server]
    public void Svr_PerformInteract(GameObject interacter)
    {
        if (!beingRevived)
        {
            connectionToClientInteractor = interacter.GetComponent<NetworkIdentity>().connectionToClient;
            Rpc_DisableMovement(connectionToClientInteractor);
            interacter.GetComponent<ReviveManager>().Rpc_StartReviveTimer(connectionToClientInteractor);
            BeingRevivedCo = BeingRevived();
            StartCoroutine(BeingRevivedCo);
        }
    }
    [Server]
    public void Svr_CancelInteract(GameObject interacter)
    {
        Rpc_EnableMovement(connectionToClientInteractor);
        interacter.GetComponent<ReviveManager>().Rpc_ReviveTimerCancelled(connectionToClientInteractor);
        ReviveCanceled();
    }
    [TargetRpc]
    private void Rpc_DisableMovement(NetworkConnection target)
    {
        JODSInput.DisableMovement();
    }

    [TargetRpc]
    private void Rpc_EnableMovement(NetworkConnection target)
    {
        JODSInput.EnableMovement();
    }

    [TargetRpc]
    public void Rpc_EnableEverythingButMenu(NetworkConnection target)
    {
        JODSInput.EnableMovement();
        JODSInput.EnableJump();
        JODSInput.EnableDrop();
        JODSInput.EnableInteract();
        JODSInput.EnableReload();
        JODSInput.EnableLMB();
        JODSInput.EnableRMB();
        JODSInput.EnableHotbarControl();
        JODSInput.EnableCamera();
    }

    [TargetRpc]
    public void Rpc_DisableEverythingButMenu(NetworkConnection target)
    {
        JODSInput.DisableMovement();
        JODSInput.DisableJump();
        JODSInput.DisableDrop();
        JODSInput.DisableInteract();
        JODSInput.DisableReload();
        JODSInput.DisableLMB();
        JODSInput.DisableRMB();
        JODSInput.DisableHotbarControl();
        JODSInput.DisableCamera();
    }
}
