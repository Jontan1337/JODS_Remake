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

	public void SetFloat(string param, float value)
	{
		anim.SetFloat(param, value);
	}
	public void SetBool(string param, bool value)
	{
		anim.SetBool(param, value);
	}

	public void SpeedUp(float speedMultiplier)
	{
		anim.speed *= speedMultiplier;
	}
	public void SlowDown(float speedMultiplier)
	{
		anim.speed /= speedMultiplier;
	}
}
