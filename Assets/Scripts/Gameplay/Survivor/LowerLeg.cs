using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerLeg : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		transform.root.GetComponentInChildren<TaekwondoAbility>().OnKickHit(col);
	}
}
