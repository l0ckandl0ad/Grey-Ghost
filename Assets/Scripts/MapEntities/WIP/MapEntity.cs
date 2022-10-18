using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not in use yet! WIP!
/// </summary>
public abstract class MapEntity : MonoBehaviour, IMapEntity
{
    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => gameObject.transform; }
    public IFF IFF { get; protected set; }
    public UnitType UnitType { get; protected set; }
    public bool IsVisible { get; protected set; } = false;
    public float Speed { get; protected set; } = 1f;

    protected float maxSpeed = 1f;
    protected WaitForSeconds slowDownTimer = new WaitForSeconds(5f); // how long will the carrier slow down for when launching/landing planes
    protected float slowDownFactor = 0.1f; // how much the carrier should slow down during deck ops

    public virtual void Update()
    {

    }

    public virtual void MakeVisible(bool trueOrFalse)
    {
        IsVisible = trueOrFalse;
    }

    public virtual void SetSpeedMultiplier(float speedMultiplier)
    {
        Speed = maxSpeed * speedMultiplier;
    }


    public virtual void SlowDown(float slowDownFactor, float durationInSeconds)
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
            Debug.Log("slowDownTimer: " + slowDownTimer.ToString());

            if (slowDownTimer <= 0) break;

            yield return null;
        }

        SetSpeedMultiplier(1);
    }
}
