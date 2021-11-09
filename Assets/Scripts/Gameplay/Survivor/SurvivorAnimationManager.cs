using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RootMotion.FinalIK;

public class SurvivorAnimationManager : NetworkBehaviour, IInitializable<SurvivorSetup>
{
	public Animator anim;
	public FullBodyBipedIK fullBodyIK;

	private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    // Start is called before the first frame update
    void Start()
	{
		if (!hasAuthority) return;
		anim = GetComponent<Animator>();
	}

	public void HasWeaponAnimation(bool enable)
	{
		anim.SetBool("HasWeapon", enable);

		//Do some IK stuff here to get the hands to fit onto the weapon.
		Debug.LogWarning("IK stuff here");
	}

	public void SetFloat(string param, float value)
	{
		anim.SetFloat(param, value);
	}
	public void SetBool(string param, bool value)
	{
		anim.SetBool(param, value);
	}

	public void SetIKRightHandEffector(Transform effector)
    {
		if (effector != null)
        {
			fullBodyIK.solver.rightHandEffector.target = effector;
			fullBodyIK.solver.rightHandEffector.positionWeight = 1f;
        }
		else
        {
			fullBodyIK.solver.rightHandEffector.target = null;
			fullBodyIK.solver.rightHandEffector.positionWeight = 0f;
        }
    }
	public void SetIKLeftHandEffector(Transform effector)
    {

		if (effector != null)
		{
			fullBodyIK.solver.leftHandEffector.target = effector;
			fullBodyIK.solver.leftHandEffector.positionWeight = 1f;
		}
		else
		{
			fullBodyIK.solver.leftHandEffector.target = null;
			fullBodyIK.solver.leftHandEffector.positionWeight = 0f;
		}
	}

    public void Init(SurvivorSetup initializer)
    {
        throw new System.NotImplementedException();
    }
}
