using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatManagerBase : NetworkBehaviour, IDamagable
{
    [SerializeField, SyncVar] protected int health = 100;
    [SerializeField, SyncVar] protected int maxHealth = 100;

    public virtual int Health
    {
        get => health;
        protected set
        {
            int prevHealth = health;
            health = Mathf.Clamp(value, 0, maxHealth);

            if (prevHealth < health)
            {
                onDamaged?.Invoke();
            }

            if (health < 0)
            {
                onDied?.Invoke();
            }
        }
    }


    [Header("Events")]
    public UnityEvent onDied = null;
    public UnityEvent onDamaged = null;

    public virtual void Svr_Damage(int damage, Transform source = null)
    {
        throw new System.NotImplementedException();
    }
}
