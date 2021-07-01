using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImpacter
{
    Action<float> OnImpact { get; set; }
}
