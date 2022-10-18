using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageableMapEntity : MapEntity, IDamageable
{
    public int MaxHealth { get; protected set; } = 1;
    public int Health { get; protected set; } = 1;
    public int VPCost { get; protected set; } = 1;
    public int VPPerDamage { get; protected set; } = 1;
    public bool IsAttackable { get; private set; } = true;

    public event Action<IDamageable, int, int> OnHit = delegate { };
    public event Action<IDamageable, int, int> OnDestroyed = delegate { };
    public event Action<IDamageable, int> OnRepair = delegate { };

    protected bool isDestroyed;

    public void Damage(int amount)
    {
        Health -= amount;
        if (amount > 0)
        {
            OnHit?.Invoke(this, amount, VPPerDamage * amount);
        }
        Debug.Log(name + " was damaged: " + amount.ToString());
    }

    public void Repair(int amount)
    {
        if (Health >= MaxHealth || Health <= 0) return;

        Health += amount;
        OnRepair?.Invoke(this, amount);
    }
}
