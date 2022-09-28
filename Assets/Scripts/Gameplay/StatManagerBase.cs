using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatManagerBase : NetworkBehaviour, IDamagable
{
    [BoxGroup("Health Stats")]
    [SerializeField, SyncVar(hook = nameof(HealthHook))] protected int health = 100;
    public virtual int Health
    {
        get => health;
        set
        {
            int prevHealth = health;
            health = Mathf.Clamp(value, 0, maxHealth);

            if (prevHealth < health)
            {
                onDamaged?.Invoke(health);
            }

            if (health < 0)
            {
                onDied?.Invoke();
            }
        }
    }
    protected virtual void HealthHook(int oldVal, int newVal) { }

    [BoxGroup("Health Stats")]
    [SerializeField, SyncVar(hook = nameof(MaxHealthHook))] protected int maxHealth = 100;

    protected virtual void MaxHealthHook(int oldVal, int newVal) { }
    public virtual int MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
        }
    }

    [Header("Events")]
    public UnityEvent onDied = null;
    public UnityEvent<int> onDamaged = null;

    public virtual void Svr_Damage(int damage, Transform source = null)
    {
        throw new System.NotImplementedException();
    }
}
