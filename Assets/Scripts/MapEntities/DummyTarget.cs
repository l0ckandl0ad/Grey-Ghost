using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTarget : MonoBehaviour, IDamageable
{
    public int VPCost { get; private set; } = 200;
    public int VPPerDamage { get; private set; } = 0;

    public int MaxHealth { get; private set; } = 999;
    public int Health { get; private set; } = 999;

    public bool IsAttackable { get; private set; } = false;

    public IFF IFF { get; private set; }

    public UnitType UnitType { get; private set; } = UnitType.NAVAL;

    public float Speed { get; private set; } = 0;

    public GameObject GameObject { get => gameObject; }

    public Transform Transform { get => transform; }

    public bool IsVisible { get; private set; } = false;

    public event Action<IDamageable, int, int> OnHit;
    public event Action<IDamageable, int, int> OnDestroyed;
    public event Action<IDamageable, int> OnRepair;

    private WaitForSeconds timer = new WaitForSeconds(1f);

    [SerializeField]
    private IFF iff; // for debug

    private void Update()
    {
        iff = IFF;
    }

    public void Initialize(IFF side, Vector3 position)
    {
        IFF = side;
        transform.position = position;
    }

    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    public void Damage(int amount)
    {
        //
    }

    public void MakeVisible(bool trueOrFalse)
    {
        //
    }

    public void Repair(int amount)
    {
        //
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        //
    }

    public void SlowDown(float slowDownFactor, float durationInSeconds)
    {
        //
    }

    private IEnumerator SelfDestruct()
    {
        yield return timer;
        Destroy(gameObject);
    }
}
