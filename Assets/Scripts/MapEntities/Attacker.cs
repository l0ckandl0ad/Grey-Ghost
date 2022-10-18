using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour, IMapEntity
{
    private SpriteRenderer unitIcon;
    public float DeckOpsDuration { get; private set; } = 1f; // how long does it take to take off or land this unit
    public float DeckOpsSlowDownRatio { get; private set; } = 0.1f; // how much will the carrier slow down during deck ops
    private float autonomyTimeInSeconds = 2.5f;
    private WaitForSeconds autonomyTimer; // how long can this entity stay autonomous before "expiring"
    public float Speed { get; private set; } = 4.5f;

    private ICarrier carrier; // parent carrier


    private Vector3 destination;
    private Vector3 originalDestination;
    private bool isOriginalDestinationReached = false; // to allow for ignoring targets until the original destination was reached
    private float distanceToTarget;

    private float searchRadius = 2f; // area to search for targets if nothing found at original attack location

    private Detection detection;
    private IDamageable target;
    private bool isTargetFound = false;

    private int damageMin = 1;
    private int damageMax = 3;
    private bool canAttack = true; // basic implementation of ammo
    private bool hasAmmo = true;

    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => gameObject.transform; }
    public IFF IFF { get; private set; } = IFF.EMPTY;
    public UnitType UnitType { get; private set; } = UnitType.AIR;
    public bool IsVisible { get; private set; } = false;

    public void MakeVisible(bool trueOrFalse)
    {
        IsVisible = trueOrFalse;
    }

    public void Initialize(string name, IFF iff, ICarrier carrier, Vector3 attackVector)
    {
        this.name = name;
        IFF = iff;
        this.carrier = carrier;
        this.destination = attackVector;
        originalDestination = attackVector;

        carrier.SlowDown(DeckOpsSlowDownRatio, DeckOpsDuration); // slow down carrier for takeoff
    }

    private void Awake()
    {
        unitIcon = GetComponentInChildren<SpriteRenderer>();
        detection = GetComponent<Detection>();
        autonomyTimer = new WaitForSeconds(autonomyTimeInSeconds);
    }

    private void Start()
    {
        StartCoroutine(EnduranceCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        HandleUnitIconVisibility();
        CheckForTargets();
        MoveEachFrame();
        AttackTarget();
        ReturnToBase();
    }

    private void HandleUnitIconVisibility()
    {
        if (!IsVisible)
        {
            unitIcon.enabled = false;
        }
        else
        {
            unitIcon.enabled = true;
        }
    }

    private void MoveEachFrame()
    {
        if (target!= null && !target.Equals(null) && canAttack)
        {
            destination = target.Transform.position;
        }

        distanceToTarget = Vector2.Distance(transform.position, destination);

        if (distanceToTarget > 0f)
        {
            // rotation
            OrientTowardsTarget();

            // movement
            transform.position = Vector2.MoveTowards(transform.position, destination, Speed * Time.deltaTime);
        }

        if (distanceToTarget < 0.1f && !isOriginalDestinationReached)
        {
            isOriginalDestinationReached = true;
        }

        // fly around if nothing is found
        if (distanceToTarget < 0.01f && !isTargetFound && canAttack)
        {
            destination = Random.insideUnitCircle * searchRadius;
            destination += originalDestination;

        }

    }

    private void OrientTowardsTarget()
    {
        if (distanceToTarget > 0.1)
        {
            //rotation version 1 (is buggy and wrong)
            //Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, attackVector.normalized);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 10000 * Time.deltaTime);

            //rotation version 2 (needs the sprite to look EASTWARD (z = -90)
            Vector3 directionToFace = destination - transform.position;
            directionToFace.Normalize();
            float rotationZ = Mathf.Atan2(directionToFace.y, directionToFace.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
    }

    private IEnumerator EnduranceCountdown()
    {
        yield return autonomyTimer;

        canAttack = false;
    }

    private void ReturnToBase()
    {
        if (canAttack) return;

        if (carrier == null || carrier.Equals(null))
        {
            Destroy(gameObject); // destroy this attacker if the parent carrier is dead
        }
        else
        {
            destination = carrier.Transform.position;

            if (Vector2.Distance(transform.position, destination) <= 0.01f)
            {
                carrier.LandAttacker(hasAmmo);
                carrier.SlowDown(DeckOpsSlowDownRatio, DeckOpsDuration);
                Destroy(gameObject);
            }
        }
    }

    private void CheckForTargets()
    {
        if (!isOriginalDestinationReached) return;
        if (isTargetFound) return;

        target = detection.SelectTarget(false); // do not use target sharing here, please!

        if (target == null || target.Equals(null)) return;

        destination = target.Transform.position;
        isTargetFound = true;
    }

    private void AttackTarget()
    {
        if (target == null || target.Equals(null) || !canAttack || !hasAmmo) return;

            if (Vector2.Distance(transform.position, target.Transform.position) < 0.01f && canAttack && target.Health > 0)
        {
            int damage = Random.Range(damageMin, damageMax);
            target.Damage(damage);
            canAttack = false;
            hasAmmo = false;
        }
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        Speed *= speedMultiplier;
    }

    public void SlowDown(float slowDownFactor, float durationinSeconds)
    {
        // do nothing
    }
}
