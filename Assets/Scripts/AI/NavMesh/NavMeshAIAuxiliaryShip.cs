using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAIAuxiliaryShip : MonoBehaviour
{
	public IDamageable target;

	[SerializeField]
	private Transform[] waypoints;
	private int currentWaypointID = 0;
	private Vector3 tempWaypoint;

	private Vector3 destination;
	private float navMeshHitDetectionMaxDistance = 4f;

	private bool isTimerRunning = false;
	private WaitForSeconds timeSpentAtTheBase = new WaitForSeconds(300f); // how long will the ship stay at the base before moving to the next destination

	private IMapEntity parent;

	private NavMeshAgent agent;

	void Awake()
	{
		parent = GetComponent<IMapEntity>();
		agent = GetComponent<NavMeshAgent>();
		agent.stoppingDistance = 0.1f;
		agent.speed = parent.Speed;
	}

    private void Start()
    {
		destination = GetNewDestination();
    }

    private void Update()
    {
		agent.speed = parent.Speed;

		MoveFromBaseToBase();
	}

	private void MoveFromBaseToBase()
    {
		if (Vector2.Distance(transform.position, destination) <= agent.stoppingDistance)
		{
			if (isTimerRunning) return;

			StartCoroutine(WaitAtTheBaseAndGoToNext());
		}

		agent.SetDestination(destination);
	}

	private Vector3 GetNewDestination()
    {
		tempWaypoint = waypoints[currentWaypointID].position;

		if (currentWaypointID == (waypoints.Length - 1))
        {
			currentWaypointID = 0; // go to the first waypoint in the array if we've reached the last waypoint
		}
        else
        {
			currentWaypointID++;
		}

		NavMeshHit hit;
		Vector3 finalPosition = Vector3.zero;
		if (NavMesh.SamplePosition(tempWaypoint, out hit, navMeshHitDetectionMaxDistance, 1))
		{
			finalPosition = hit.position;
		}

		return finalPosition;
	}

	private IEnumerator WaitAtTheBaseAndGoToNext()
	{
		isTimerRunning = true;

		yield return timeSpentAtTheBase;
		destination = GetNewDestination();
		isTimerRunning = false;
	}
}
