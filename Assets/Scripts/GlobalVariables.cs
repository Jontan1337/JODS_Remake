using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
    public static Dictionary<string, Tags> SurfaceTypes = new Dictionary<string, Tags>
    {
        {"", Tags.BulletHole },
        {"Concrete", Tags.BulletHole },
        {"Metal", Tags.BulletMetalImpact },
        {"Flesh", Tags.BulletFleshImpact }
    };
}

