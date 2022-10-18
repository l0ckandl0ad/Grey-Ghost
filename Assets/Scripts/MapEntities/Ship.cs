using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IMapEntity, IDamageable
{
    public IFF IFF { get; protected set; } = IFF.OPFOR;
    public UnitType UnitType { get; protected set; } = UnitType.NAVAL;
    public bool IsVisible { get; protected set; } = false;

    protected SpriteRenderer spriteRenderer;

    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => gameObject.transform; }
    public bool IsAttackable { get; private set; } = true;
    public int MaxHealth { get; protected set; } = 2;
    public int Health { get; protected set; } = 2;

    public float Speed { get; protected set; } = 0.2f;

    protected WaitForSeconds slowDownForDeckOps = new WaitForSeconds(1.5f); // how long will the carrier slow down for when launching/landing planes
    protected float slowDownFactor = 0.1f; // how much the carrier should slow down during deck ops

    /// <summary>
    /// How long will the unit stay alive after getting 0 health.
    /// </summary>
    protected WaitForSeconds destructionDelay = new WaitForSeconds(1.5f);

    protected float maxSpeed = 0.2f;
    public int VPCost { get; protected set; } = 5;
    public int VPPerDamage { get; protected set; } = 1;

    public event Action<IDamageable, int, int> OnHit = delegate { };
    public event Action<IDamageable, int, int> OnDestroyed = delegate { };
    public event Action<IDamageable, int> OnRepair = delegate { };

    protected bool isDestroyed;
    [SerializeField]
    protected int health; // for debug

    [SerializeField]
    protected IFF iff;

    public bool IsAbleToAttack { get; private set; } = true;
    public virtual void Damage(int amount)
    {
        if (Health <= 0) return;

        Health -= amount;

        if (Health < 0) Health = 0; // to keep health from going below 0

        OnHit?.Invoke(this, amount, VPPerDamage * amount);
        Debug.Log(name + " was damaged: " + amount.ToString());
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

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        IFF = iff;
        //ChangeColorBasedOnSide(iff);
    }


    void Update()
    {
        health = Health; // for debug

        if (!IsVisible)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
        }

        if (isDestroyed) return;

        if (Health <= 0)
        {
            OnDestroyed?.Invoke(this, 1, VPCost);
            isDestroyed = true;
            StartCoroutine(DestructionWithDelay());
        }
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        Speed = maxSpeed * speedMultiplier;
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

    public virtual void CanAttack(bool trueOrFalse)
    {
        IsAbleToAttack = trueOrFalse;
    }

    private void ChangeColorBasedOnSide(IFF iff)
    {
        switch (iff)
        {
            case IFF.BLUFOR:
                spriteRenderer.color = Color.blue;
                break;

            case IFF.OPFOR:
                spriteRenderer.color = Color.red;
                break;

            default:
                //
                break;
        }
    }

    private IEnumerator DestructionWithDelay()
    {
        yield return destructionDelay;
        Destroy(gameObject);
    }
}
