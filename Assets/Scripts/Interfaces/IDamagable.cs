using UnityEngine;
using Mirror;

public interface IDamagable
{
    Teams Team { get; }

    [Server]
    void Svr_Damage(int damage);

    int GetHealth();

    bool IsDead();
}