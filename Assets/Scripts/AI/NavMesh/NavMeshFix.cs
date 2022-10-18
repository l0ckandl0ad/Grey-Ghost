using UnityEngine;

public class NavMeshFix : MonoBehaviour
{
	void Start()
	{
		var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
	}
}
