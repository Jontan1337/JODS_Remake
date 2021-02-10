using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorStats : NetworkBehaviour, IDamagable
{
    public int health = 100;
    public int armor = 0;

    Teams IDamagable.Team => Teams.Player;

    [Server]
    void IDamagable.Svr_Damage(int damage)
    {
        if (armor > 0) armor -= damage;
        else health -= damage;
    }
}
