using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAICarrier : MonoBehaviour
{
	public IDamageable target;

	private Vector3 originalPosition; // where this agents spawns. will be used for patrol around this point at certain radius
	private Vector3 destination;
	private Vector3 tempVector;
	private float patrolRadius = 20f;
	private float defaultStoppingDistance = 3f;
	private float shortStoppingDistance = 0.1f; // how close would the agent try to go for certain destinations, ie friendly naval base
	private float defaultAvoidanceRadius = 0.5f;
	private float shortAvoidanceRadius = 0.1f;

	private IMapEntity parent;
	private ICarrier carrier;

	private NavMeshAgent agent;
	private Detection detection;
	private AttackAbility attackAbility;

	private WaitForSeconds aiAttackingTimer = new WaitForSeconds(3f); // how often will the ai re-evaluate combat directives and movement

	private NavalBase friendlyBase;
	[SerializeField]
	private int decisionPoints;
	[SerializeField]
	private bool isTryingToAttack;
	[SerializeField]
	private bool isReturningToBase;
	[SerializeField]
	private bool isAllowedToLeaveTheBase = true; // to trigger when the carrier should depart from the base after RTBing
	[SerializeField]
	private float debugHealth; // for debug

	[SerializeField]
	private string debugDestination;
	[SerializeField]
	private string debugTarget;

	/// <summary>
	/// THIS IS NEEDED FOR NAVMESH AI FIX
	/// https://github.com/h8man/NavMeshPlus/wiki/HOW-TO#known-issues
	/// </summary>
	private float agentDrift = 0.0001f;


	void Awake()
	{
		parent = GetComponent<IMapEntity>();
		carrier = GetComponent<ICarrier>();
		agent = GetComponent<NavMeshAgent>();
		agent.stoppingDistance = defaultStoppingDistance;
		agent.radius = defaultAvoidanceRadius;
		agent.speed = parent.Speed;

		detection = GetComponent<Detection>();
		attackAbility = GetComponent<AttackAbility>();
	}

    private void Start()
    {
		destination = GetNewPatrolDestination();
		originalPosition = transform.position;

    }



    private void Update()
    {
		agent.speed = parent.Speed;

		EvaluateReturnToBase();
		ReturnToBase();

		CheckForTargetsAndAttack();

		Move();

		debugHealth = carrier.Health; // debug!
		debugDestination = destination.ToString(); // debug!
		debugTarget = target?.ToString(); // debug!
	}

	private void CheckForTargetsAndAttack()
    {
        if (isTryingToAttack) return;

		target = detection.SelectTarget(true);

		if (target == null || target.Equals(null)) return;

		Debug.Log(this + "'s target: " + target);

		StartCoroutine(AttackTimer());

		if (!isReturningToBase)
        {
			destination = target.Transform.position;
		}

		if (!target.IsAttackable) return; // but do not engage the target when it's a dummy
		Debug.Log(this + " trying to attack " + target);
		attackAbility.InitiateAttack(target.Transform.position);
	}

	private IEnumerator AttackTimer()
	{
		isTryingToAttack = true;

		yield return aiAttackingTimer;

		isTryingToAttack = false;
	}

	private void Move()
	{
		if (!isReturningToBase && isAllowedToLeaveTheBase)
        {
			if (Vector2.Distance(transform.position, destination) <= agent.stoppingDistance)
			{
				destination = GetNewPatrolDestination();
				detection.FarSearch(); // do a search at the end of each destination point
			}
		}

		SetDestination(destination);
	}

	private Vector3 GetNewPatrolDestination()
    {
		tempVector = RandomNavmeshLocation(patrolRadius);

		return tempVector;
    }

	public Vector3 RandomNavmeshLocation(float radius)
	{
		Vector3 randomDirection = Random.insideUnitSphere * radius;
		randomDirection += originalPosition;
		NavMeshHit hit;
		Vector3 finalPosition = Vector3.zero;
		if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
		{
			finalPosition = hit.position;
		}
		return finalPosition;

		// credits go to Selzier
		// https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
	}

	private void EvaluateReturnToBase()
    {
		decisionPoints = 0;

		if (carrier.Health <= (carrier.MaxHealth / 2))
        {
			decisionPoints += 5;
        }

		if (carrier.Ammo <= 1)
        {
			decisionPoints += 5;
        }

		if (carrier.AttackersAvailable <= 0 && !attackAbility.IsAwaitingCraftToReturn)
        {
			decisionPoints += 5;
		}

		if (decisionPoints >= 5)
		{
			isReturningToBase = true;
			agent.stoppingDistance = shortStoppingDistance;
			agent.radius = shortAvoidanceRadius;
		}

		if (isReturningToBase)
		{
			if (carrier.Ammo < carrier.MaxAmmo || carrier.Health < carrier.MaxHealth)
            {
				isAllowedToLeaveTheBase = false;
			}
			else
			{
				isAllowedToLeaveTheBase = true;
				isReturningToBase = false;
				agent.stoppingDistance = defaultStoppingDistance;
				agent.radius = defaultAvoidanceRadius;
				destination = GetNewPatrolDestination();
			}
		}
		
	}

	private void ReturnToBase()
    {
		if (!isReturningToBase) return;

		if (friendlyBase == null || friendlyBase.Equals(null) || friendlyBase.Health == 0)
        {
			FindFriendlyBase();
		}
        else if (friendlyBase != null && !friendlyBase.Equals(null))
        {
			agent.stoppingDistance = shortStoppingDistance;
			destination = friendlyBase.transform.position;
		}
	}

	private void FindFriendlyBase()
	{
		NavalBase[] bases = FindObjectsOfType<NavalBase>();
		List<NavalBase> friendlyBases = new List<NavalBase>();

		foreach (NavalBase navalBase in bases)
		{
			if (navalBase.IFF == parent.IFF && navalBase.Health > 0)
            {
				friendlyBases.Add(navalBase);
            }
        }

		if (friendlyBases.Count == 0) return;

		friendlyBase = friendlyBases[0];
	}

	private void SetDestination(Vector3 target)
    {
		var driftPos = target + (Vector3)(agentDrift * Random.insideUnitCircle);
		agent.SetDestination(driftPos);
	}
}
