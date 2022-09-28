using UnityEngine;
using Mirror;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class SurvivorAnimationManager : NetworkBehaviour
{
	public Animator characerAnimator = null;
	[SerializeField] private FullBodyBipedIK fullBodyBipedIK = null;
	[Space]
	[SerializeField] private Transform rightHandEffector;
	[SerializeField] private Transform leftHandEffector;
	[Title("Prefab references")]
	[SerializeField] private Transform cameraTransform = null;
	[SerializeField] private Transform originalCameraTransformParent = null;
	[SerializeField] private Transform firstPersonRightShoulderEffector = null;
	[SerializeField] private Transform firstPersonLeftShoulderEffector = null;
	[Title("Aiming IK references")]
	[SerializeField] private Transform firstPersonRightShoulderAimingEffector = null;
	[SerializeField] private Transform firstPersonLeftShoulderAimingEffector = null;
	[SerializeField] private Transform secondPersonRightShoulderAimingEffector = null;
	[SerializeField] private Transform secondPersonLeftShoulderAimingEffector = null;
	[Space]
	[SerializeField] private HandPoser rightHandPoser = null;
	[SerializeField] private HandPoser leftHandPoser = null;

	private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

	private SurvivorStatManager characterStatManager;

    private void Awake()
	{
		GetComponent<SurvivorSetup>().onServerSpawnItem += GetReferences;
	}

    public override void OnStartClient()
	{
		// Other players should not see the shoulders locked in to the idle IK position.
        if (!hasAuthority)
        {
			SetupIKValues();
        }
		if (isServer)
        {
			characterStatManager = GetComponent<SurvivorStatManager>();
			characterStatManager.onDownChanged.AddListener(delegate (bool isDown) { Svr_OnDownChanged(isDown); });
        }
	}

    private void GetReferences(GameObject item)
    {
		if (item.TryGetComponent(out ItemName itemName))
		{
            switch (itemName.itemName)
            {
                case ItemNames.VirtualHead:
                    break;
                case ItemNames.ItemContainer:
                    break;
                case ItemNames.Camera:
						cameraTransform = transform.Find("Virtual Head(Clone)/PlayerCamera(Clone)");
						originalCameraTransformParent = transform.Find("Virtual Head(Clone)");
					break;
                case ItemNames.Equipment:
					if (item.TryGetComponent(out PlayerEquipment playerEquipment))
					{
						playerEquipment.onServerEquippedItemChange += OnServerEquippedItemChange;
					}
                    break;
            }
		}
	}

    private void OnServerEquippedItemChange(GameObject oldItem, GameObject newItem)
    {
        if (newItem)
        {
			if (newItem.TryGetComponent(out HandIKEffectors handIKEffectors))
			{
                Rpc_SetIKEffectors(handIKEffectors);
				return;
			}
		}
        Rpc_SetIKEffectors(null);
    }

	private void SetupIKValues()
    {
		fullBodyBipedIK.solver.rightShoulderEffector.positionWeight = 0f;
		fullBodyBipedIK.solver.rightShoulderEffector.rotationWeight = 0f;
		fullBodyBipedIK.solver.leftShoulderEffector.positionWeight = 0f;
		fullBodyBipedIK.solver.leftShoulderEffector.rotationWeight = 0f;
		Transform shoulderParent = firstPersonRightShoulderAimingEffector.parent;
	}

	public void SetFloat(string param, float value)
	{
		characerAnimator.SetFloat(param, value);
	}
	public void SetBool(string param, bool value)
	{
		characerAnimator.SetBool(param, value);
	}

	[Server]
	private void Svr_OnDownChanged(bool isDown)
    {
		if (isDown)
        {
			Rpc_SetCameraForDownedState(connectionToClient);
			fullBodyBipedIK.enabled = false;
			characerAnimator.SetBool("IsDown", true);
        }
		else
        {
			Rpc_SetCameraForRevivedState(connectionToClient);
			fullBodyBipedIK.enabled = true;
			characerAnimator.SetBool("IsDown", false);
        }
    }
	[TargetRpc]
	private void Rpc_SetCameraForDownedState(NetworkConnection target)
	{
        cameraTransform.SetParent(fullBodyBipedIK.references.head.GetChild(0));
    }
	[TargetRpc]
	private void Rpc_SetCameraForRevivedState(NetworkConnection target)
	{
		cameraTransform.SetParent(originalCameraTransformParent);
		cameraTransform.localPosition = new Vector3(0f, 0.1f, 0f);
		cameraTransform.rotation = originalCameraTransformParent.rotation;
	}

	[ClientRpc]
	public void Rpc_SetIKEffectors(HandIKEffectors handIKEffectors)
    {
		if (handIKEffectors != null)
        {
			// If an item is equipped.
			Transform effector = null;

			if (handIKEffectors.rightHandEffector)
			{
				// If item does have right hand effector.
				// Set right side IK.
				effector = handIKEffectors.rightHandEffector;
				fullBodyBipedIK.solver.rightHandEffector.target = effector;
				fullBodyBipedIK.solver.rightHandEffector.positionWeight = 1f;
				fullBodyBipedIK.solver.rightHandEffector.rotationWeight = 1f;
				fullBodyBipedIK.solver.rightShoulderEffector.positionWeight = 1f;
				fullBodyBipedIK.solver.rightShoulderEffector.rotationWeight = 1f;
				// Other players should see the players shoulder more forward.
				if (hasAuthority)
				{
                    fullBodyBipedIK.solver.rightShoulderEffector.target = firstPersonRightShoulderAimingEffector;
                }
				else
				{
					fullBodyBipedIK.solver.rightShoulderEffector.target = secondPersonRightShoulderAimingEffector;
				}
				rightHandPoser.poseRoot = effector;
				rightHandPoser.localPositionWeight = 1f;
				rightHandPoser.localRotationWeight = 1f;
            }
			else
            {
                // If item doesn't have right hand effector.
                // Set right side IK.
                ResetRightEffectors();
            }

            if (handIKEffectors.leftHandEffector)
			{
				// If item does have left hand effector.
				// Set left side IK.
				effector = handIKEffectors.leftHandEffector;
				fullBodyBipedIK.solver.leftHandEffector.target = effector;
				fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1f;
				fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 1f;
				if (hasAuthority)
				{
                    fullBodyBipedIK.solver.leftShoulderEffector.target = firstPersonLeftShoulderAimingEffector;
                    fullBodyBipedIK.solver.leftShoulderEffector.positionWeight = 0f;
				}
				else
				{
					fullBodyBipedIK.solver.leftShoulderEffector.positionWeight = 1f;
				    fullBodyBipedIK.solver.leftShoulderEffector.target = secondPersonLeftShoulderAimingEffector;
				}
				fullBodyBipedIK.solver.leftShoulderEffector.rotationWeight = 1f;
				leftHandPoser.poseRoot = effector;
				leftHandPoser.localPositionWeight = 1f;
				leftHandPoser.localRotationWeight = 1f;
            }
			else
            {
                // If item doesn't have left hand effector.
                // Set left side IK.
                ResetLeftEffectors();
            }
        }
		else
		{
            // If no item is equipped.
            // Set right side IK
            ResetRightEffectors();

			// Set left side IK
			ResetLeftEffectors();
		}
    }

    private void ResetLeftEffectors()
    {
        fullBodyBipedIK.solver.leftHandEffector.target = null;
        fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0f;
        fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0f;
        if (hasAuthority)
		{
			fullBodyBipedIK.solver.leftShoulderEffector.positionWeight = 1f;
            fullBodyBipedIK.solver.leftShoulderEffector.rotationWeight = 1f;
        }
        else
		{
			fullBodyBipedIK.solver.leftShoulderEffector.positionWeight = 0f;
            fullBodyBipedIK.solver.leftShoulderEffector.rotationWeight = 0f;
        }
        fullBodyBipedIK.solver.leftShoulderEffector.target = firstPersonLeftShoulderEffector;
        leftHandPoser.poseRoot = null;
        leftHandPoser.localPositionWeight = 0f;
        leftHandPoser.localRotationWeight = 0f;
    }

    private void ResetRightEffectors()
    {
        fullBodyBipedIK.solver.rightHandEffector.target = null;
        fullBodyBipedIK.solver.rightHandEffector.positionWeight = 0f;
        fullBodyBipedIK.solver.rightHandEffector.rotationWeight = 0f;
        if (hasAuthority)
		{
			fullBodyBipedIK.solver.rightShoulderEffector.positionWeight = 1f;
            fullBodyBipedIK.solver.rightShoulderEffector.rotationWeight = 1f;
        }
        else
		{
			fullBodyBipedIK.solver.rightShoulderEffector.positionWeight = 0f;
            fullBodyBipedIK.solver.rightShoulderEffector.rotationWeight = 0f;
        }
        fullBodyBipedIK.solver.rightShoulderEffector.target = firstPersonRightShoulderEffector;
        rightHandPoser.poseRoot = null;
        rightHandPoser.localPositionWeight = 0f;
        rightHandPoser.localRotationWeight = 0f;
    }
}
