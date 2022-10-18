using UnityEngine;
using UnityEngine.AI;

public class IconDirectionForNavMeshAgent : IconDirectionForUnitBase
{
    protected NavMeshAgent agent;

    protected override void CacheReferences()
    {
        base.CacheReferences();
        agent = GetComponentInParent<NavMeshAgent>();
    }

    protected override Vector3 CheckMovementVector()
    {
        return agent.velocity;
    }

}
