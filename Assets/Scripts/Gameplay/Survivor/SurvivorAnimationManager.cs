using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorAnimationManager : NetworkBehaviour
{
	private Animator anim;
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

	private void WalkAnimationStart()
	{
		anim.Play("");
	}
}
