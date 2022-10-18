using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCarrier : Ship, IMapEntity, IDamageable, ICarrier
{
    public int MaxAttackers { get; protected set; } = 2;
    public int MaxAmmo { get; protected set; } = 8;
    public int AttackersAvailable { get => attackersAvailable; }
    protected int attackersAvailable = 2;

    public int Ammo { get => ammo; }
    [SerializeField]
    protected int ammo = 8;

    //public bool IsAbleToAttack { get; private set; } = true;
    
    private EnemyCarrier()
    {
        VPCost = 200;
        VPPerDamage = 2;
        Health = 8;
        MaxHealth = 8;
        Speed = 0.7f;
        maxSpeed = 0.7f;
    } 


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

    public override void Damage(int amount)
    {
        base.Damage(amount);
        TakeAdditionalDamage(amount);
    }


    private void TakeAdditionalDamage(int amount)
    {
        if (amount <= 1) return;

        float randomRoll = Random.Range(0f, 1f);

        /// take damage to ammo and attackers if any
        if (randomRoll >= 0.5f)
        {
            if (attackersAvailable >= 1)
            {
                attackersAvailable--;
            }
        }

        randomRoll = Random.Range(0f, 1f);

        if (randomRoll >= 0.4f)
        {
            if (ammo - amount >= 0)
            {
                ammo -= amount;
                Health -= amount;
            }
        }
    }
}
