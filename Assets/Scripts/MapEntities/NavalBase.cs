using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavalBase : MonoBehaviour, IMapEntity, IDamageable
{
    public IFF IFF { get => iff; }
    [SerializeField]
    private IFF iff = IFF.EMPTY;
    public UnitType UnitType { get; private set; } = UnitType.BASE;
    public bool IsVisible { get; private set; } = true;
    private SpriteRenderer spriteRenderer;
    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => gameObject.transform; }

    public bool IsAttackable { get; private set; } = true;
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

    public void SlowDown(float slowDownFactor, float duration)
    {
        // do nothing
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

    private void Update()
    {
        health = Health;
        SelfRepairWhenDamaged();
    }
}
