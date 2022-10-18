using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IMapEntity, IDamageable, ICarrier
{

    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => gameObject.transform; }
    public IFF IFF { get; private set; } = IFF.BLUFOR;
    public UnitType UnitType { get; private set; } = UnitType.NAVAL;
    public bool IsVisible { get; private set; } = false;

    private WaitForSeconds slowDownForDeckOps = new WaitForSeconds(1.5f); // how long will the carrier slow down for when launching/landing planes
    private float slowDownFactor = 0.1f; // how much the carrier should slow down during deck ops

    /// <summary>
    /// How long will the unit stay alive after getting 0 health.
    /// </summary>
    protected WaitForSeconds destructionDelay = new WaitForSeconds(1.5f);
    public bool IsAttackable { get; private set; } = true;
    public int MaxAttackers { get; private set; } = 2;
    public int MaxAmmo { get; private set; } = 8;
    public int AttackersAvailable { get => attackersAvailable; }
    private int attackersAvailable = 2;

    public int Ammo { get => ammo; }
    private int ammo = 8;

    public int MaxHealth { get; private set; } = 10;
    public int Health { get; private set; } = 10;
    public float Speed { get; private set; } = 0.7f;

    private float maxSpeed = 0.7f;

    public int VPCost { get; private set; } = 200;
    public int VPPerDamage { get; private set; } = 2;

    public event Action<IDamageable, int, int> OnHit = delegate { };
    public event Action<IDamageable, int, int> OnDestroyed = delegate { };
    public event Action<IDamageable, int> OnRepair = delegate { };

    public bool IsAbleToAttack { get; private set; } = true;

    private bool isDestroyed;
    public bool IsBusyDoingOps { get; private set; } = false;

    public void Damage(int amount)
    {
        if (Health <= 0) return;

        Health -= amount;

        if (Health < 0) Health = 0; // to keep health from going below 0

        OnHit?.Invoke(this, amount, VPPerDamage * amount);
        Debug.Log(name + " was damaged: " + amount.ToString());

        TakeAdditionalDamage(amount);
    }

    public void Repair(int amount)
    {
        if (Health >= MaxHealth || Health <= 0) return;

        Health += amount;
        OnRepair?.Invoke(this, amount);
    }

    public void MakeVisible(bool trueOrFalse)
    {
        IsVisible = trueOrFalse;
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        Speed = maxSpeed * speedMultiplier;
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

    private void Update()
    {
        if (isDestroyed) return;

        if (Health <= 0)
        {
            isDestroyed = true;
            OnDestroyed?.Invoke(this, 1, VPCost);
            StartCoroutine(DestructionWithDelay());
        }
    }

    public void SlowDown(float slowDownFactor, float durationInSeconds)
    {
        StartCoroutine(SlowDownUnit(slowDownFactor, durationInSeconds));

    }

    private IEnumerator SlowDownUnit(float slowDownFactor, float timer)
    {
        float slowDownTimer = timer;

        while (slowDownTimer >= 0)
        {
            slowDownTimer -= Time.deltaTime;
            SetSpeedMultiplier(slowDownFactor);
            CanAttack(false);
            IsBusyDoingOps = true;

            if (slowDownTimer <= 0) break;

            yield return null;
        }

        SetSpeedMultiplier(1);
        CanAttack(true);
        IsBusyDoingOps = false;
    }

    public void CanAttack(bool trueOrFalse)
    {
        IsAbleToAttack = trueOrFalse;
    }

    private void TakeAdditionalDamage(int amount)
    {
        if (amount <= 1) return;

        float randomRoll = Random.Range(0f, 1f);

        /// take damage to ammo and attackers if any
        if (randomRoll >= 0.7f)
        {
            if (attackersAvailable >= 1)
            {
                attackersAvailable--;
            }
        }

        randomRoll = Random.Range(0f, 1f);

        if (randomRoll >= 0.7f)
        {
            if (ammo - amount >= 0)
            {
                ammo -= amount;
                Health -= amount;
            }
        }
    }

    private IEnumerator DestructionWithDelay()
    {
        yield return destructionDelay;
        Destroy(gameObject);
    }

}
