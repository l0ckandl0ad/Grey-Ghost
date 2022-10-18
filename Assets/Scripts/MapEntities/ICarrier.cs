using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarrier : IMapEntity, IDamageable
{
    int MaxAttackers { get; }
    int MaxAmmo { get; }
    /// <summary>
    /// How many attacker ability uses is currently available for this unit.
    /// </summary>
    int AttackersAvailable { get; } 
    /// <summary>
    /// Every attacker ability use/launch uses one ammo unit. This shows how many ammo is left.
    /// </summary>
    int Ammo { get; }
    /// <summary>
    /// Tells the carrier that the attacker is launched.
    /// </summary>
    void LaunchAttacker();
    /// <summary>
    /// Tells the carrier that the attacker is landed.
    /// </summary>
    void LandAttacker(bool stillHasAmmo);
    void Replenish(int attackersAmount, int ammoAmount);
    /// <summary>
    /// A way to toggle the ability of the carrier to attack, ie when it is being serviced by the naval base.
    /// </summary>
    /// <param name="trueOrFalse"></param>
    void CanAttack(bool trueOrFalse);
    bool IsAbleToAttack { get; }
}
