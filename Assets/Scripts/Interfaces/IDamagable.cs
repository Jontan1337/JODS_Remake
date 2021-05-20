using UnityEngine;
using Mirror;

public interface IDamagable
{
    Teams Team { get; }

    [Server]
    void Svr_Damage(int damage, Transform target = null);

    int GetHealth();

    bool IsDead();
}