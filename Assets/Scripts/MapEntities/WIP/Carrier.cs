using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : DamageableMapEntity, IMapEntity, IDamageable, ICarrier
{
    public Carrier()
    {
        UnitType = UnitType.NAVAL;
        slowDownTimer = new WaitForSeconds(2.5f); // how long will the carrier slow down for when launching/landing planes

        Speed = 1;
        maxSpeed = 1;

        Health = 6;
        MaxHealth = 6;
        VPCost = 200;
        VPPerDamage = 2;
    }

    public int MaxAttackers { get; protected set; } = 1;
    public int MaxAmmo { get; protected set; } = 6;
    public bool IsAbleToAttack { get; protected set; } = true;
    public int AttackersAvailable { get => attackersAvailable; }


    protected int attackersAvailable = 1;
    public int Ammo { get => ammo; }
    protected int ammo = 6;

    public void LaunchAttacker()
    {
        attackersAvailable -= 1;
        ammo -= 1;
    }

    public void LandAttacker(bool stillHasAmmo)
    {
        if (attackersAvailable < MaxAttackers)
        {
            attackersAvailable += 1;
        }

        if (stillHasAmmo && ammo < MaxAmmo)
        {
            ammo++;
        }
    }

    public void Replenish(int attackerAmount, int ammoAmount)
    {
        if (attackersAvailable + attackerAmount <= MaxAttackers)
        {
            attackersAvailable += attackerAmount;
        }

        if (ammo + ammoAmount <= MaxAmmo)
        {
            ammo += ammoAmount;
        }
    }

    public void CanAttack(bool trueOrFalse)
    {
        IsAbleToAttack = trueOrFalse;
    }

}
