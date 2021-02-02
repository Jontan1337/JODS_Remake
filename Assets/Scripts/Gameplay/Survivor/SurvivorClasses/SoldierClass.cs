using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierClass : SurvivorClass
{
    public string _class = "soldier";
    public override void ActiveAbility()
    {
        print("BANG");
    }
}
