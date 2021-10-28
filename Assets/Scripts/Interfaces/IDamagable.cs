using UnityEngine;
using Mirror;

public interface IDamagable
{
    Teams Team { get; }
    int GetHealth { get; }
    bool IsDead { get; }

    [Server]
    void Svr_Damage(int damage, Transform source = null);

}