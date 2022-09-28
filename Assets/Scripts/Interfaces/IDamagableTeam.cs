using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagableTeam : IDamagable
{
    Teams Team { get; }
}
