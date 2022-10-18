using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AirBase : MonoBehaviour, IMapEntity, IDamageable, ICarrier
{
    public IFF IFF { get => iff; }
    [SerializeField]
    private IFF iff = IFF.EMPTY;
    public UnitType UnitType { get; private set; } = UnitType.BASE;
    public bool IsVisible { get; private set; } = true;
    private SpriteRenderer spriteRenderer;
    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => gameObject.transform; }
    public int MaxAttackers { get; private set; } = 1;
    public int MaxAmmo { get; private set; } = 24;
    public int AttackersAvailable { get => attackersAvailable; }
    private int attackersAvailable = 1;
    public bool IsAttackable { get; private set; } = true;
    public int Ammo { get => ammo; }
    [SerializeField]
    private int ammo = 24;
    public int MaxHealth { get; private set; } = 6;
    public int Health { get; private set; } = 6;

    [SerializeField]
    private float repairSpeed = 0.1f;
    [SerializeField]
    private float tempRepairPoints;
    [SerializeField]
    private int health;

    public float Speed { get; private set; } = 0f;
    private float maxSpeed = 0;
    public int VPCost { get; protected set; } = 20;
    public int VPPerDamage { get; protected set; } = 1;

    public event Action<IDamageable, int, int> OnHit = delegate { };
    public event Action<IDamageable, int, int> OnDestroyed = delegate { };
    public event Action<IDamageable, int> OnRepair = delegate { };
    public bool IsAbleToAttack { get; private set; } = true;

    private Color sideColor = Color.grey;
    private Color damagedColor = Color.grey;

    public void Damage(int amount)
    {
        if (Health <= 0) return;

        Health -= amount;
        tempRepairPoints = 0f;

        if (Health < 0) Health = 0; // to keep health from going below 0

        ChangeColorBasedOnHealth();

        OnHit?.Invoke(this, amount, VPPerDamage * amount);
        Debug.Log(name + " was damaged: " + amount.ToString());

        TakeAdditionalDamage(amount);

    }

    public void Repair(int amount)
    {
        if (Health >= MaxHealth || Health <= 0) return;

        Health += amount;
        OnRepair?.Invoke(this, amount);
        ChangeColorBasedOnHealth();
    }
    public void MakeVisible(bool trueOrFalse)
    {
        // nothing needed
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ChangeColorBasedOnSide(iff);
    }

    // Update is called once per frame
    void Update()
    {
        health = Health;
        SelfRepairWhenDamaged();
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        // nothing needed
    }

    private void ChangeColorBasedOnSide(IFF iff)
    {
        switch (iff)
        {
            case IFF.BLUFOR:
                sideColor = Color.blue;
                break;

            case IFF.OPFOR:
                sideColor = Color.red;
                break;

            default:
                //
                break;
        }

        ChangeColor(sideColor);
    }

    private void ChangeColor(Color color)
    {
        spriteRenderer.color = color;
    }

    private void ChangeColorBasedOnHealth()
    {
        if (Health <= 0)
        {
            ChangeColor(damagedColor);
        }
        else
        {
            ChangeColor(sideColor);
        }
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

            if (slowDownTimer <= 0) break;

            yield return null;
        }

        SetSpeedMultiplier(1);
        CanAttack(true);
    }

    private void SelfRepairWhenDamaged()
    {
        if (Health < MaxHealth)
        {
            tempRepairPoints += repairSpeed * Time.deltaTime;
            if (tempRepairPoints >= 1)
            {
                Repair(1);
                tempRepairPoints--;
            }
        }
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
        if (randomRoll >= 0.5f)
        {
            if (attackersAvailable >= 1)
            {
                attackersAvailable--;
            }
        }

        randomRoll = Random.Range(0f, 1f);

        if (randomRoll >= 0.5f)
        {
            if (ammo - amount >= 0)
            {
                ammo -= amount;
                Health -= amount;
            }
        }
    }
}
