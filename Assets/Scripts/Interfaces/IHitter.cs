using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitter
{
	void OnFlyingKickHit(ControllerColliderHit hit);
}
