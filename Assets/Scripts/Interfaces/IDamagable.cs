using UnityEngine;
using Mirror;

public interface IDamagable
{
    [Server]
    void Svr_Damage(int damage, Transform source = null);
}