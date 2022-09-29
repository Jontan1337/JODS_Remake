using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseStatManager : NetworkBehaviour, IDamagable
{
    [Title("Base Stat Manager", titleAlignment: TitleAlignments.Centered)]
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
                onDamaged?.Invoke();
            }

            if (health < 0)
            {
                onDied?.Invoke();
            }
        }
    }
    protected virtual void HealthHook(int oldVal, int newVal) { }

    public bool IsDead => health > 0;

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
    public UnityEvent onDamaged = null;


    public virtual void Svr_Damage(int damage, Transform source = null)
    {
        Health -= damage;
    }
}
