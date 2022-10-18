using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable : IMapEntity
{
    /// <summary>
    /// How much VPs would the opponent score by destroying this platform
    /// </summary>
    int VPCost { get; }
    /// <summary>
    /// How much VPs would be granted for damaging this unit, in points per 1 point of damage.
    /// </summary>
    int VPPerDamage { get; }
    /// <summary>
    /// To be used for restoring entities' health to max
    /// </summary>
    int MaxHealth { get; }

    /// <summary>
    /// Current health.
    /// </summary>
    int Health { get;  }
    /// <summary>
    /// To diffirentiate between targets. Mainly to allow for having non-attackable CONTACT! type of units that would force the ai to act like there's a real enemy.
    /// </summary>
    bool IsAttackable { get; }
    void Damage(int amount);
    void Repair(int amount);
    /// <summary>
    /// This event triggers when the unit is hit and returns the unit, amount of damage and the amount of VPs that was scored by the attack.
    /// </summary>
    event Action<IDamageable, int, int> OnHit;
    /// <summary>
    /// Triggers on unit being destroyed (or disabled when it comes to bases), returning unit, damage and the amount of VPs scored.
    /// </summary>
    event Action<IDamageable, int, int > OnDestroyed;
    /// <summary>
    /// Triggers when unit is being repaired, passing the unit and the amount of repaired health.
    /// </summary>
    event Action<IDamageable, int> OnRepair;
}
