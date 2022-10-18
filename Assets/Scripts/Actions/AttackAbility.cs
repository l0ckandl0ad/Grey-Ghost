using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackAbility : MonoBehaviour
{
    public float MaxAttackRange { get; private set; } = 6.75f; // how far can an attack be made? 
    public bool IsAwaitingCraftToReturn { get; private set; }
    private WaitForSeconds waitDuration = new WaitForSeconds(10f); // how long will we wait for the attackers to return

    private bool isAttackUnderway;
    private float attackCooldownInSeconds = 1f; // how often can the attack be launched - not the same as attacker's autonomy!
    private WaitForSeconds attackCooldown; 

    [SerializeField]
    private GameObject attackBluforPrefab;
    [SerializeField]
    private GameObject attackOpforPrefab;
    private GameObject attackerObject;
    private Attacker attacker;
    private IMapEntity parent;
    private ICarrier carrier;

    private float distanceToTarget = 0f;

    public void InitiateAttack(Vector3 target)
    {
        if (isAttackUnderway) return;

        if (!carrier.IsAbleToAttack) return;

        if (carrier.Ammo <= 0 || carrier.AttackersAvailable <= 0 || carrier.Health <= 0 ) return;

        if (IsTargetReachable(target))
        {
            StartAttack(target);
        }
    }

    private void Awake()
    {
        parent = GetComponent<IMapEntity>();
        carrier = GetComponent<ICarrier>();

        attackCooldown = new WaitForSeconds(attackCooldownInSeconds);
    }

    private bool IsTargetReachable(Vector3 target)
    {
        distanceToTarget = Vector2.Distance(transform.position, target);

        if (distanceToTarget <= MaxAttackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartAttack(Vector3 target)
    {
        attackerObject = Instantiate(SelectPrefabBasedOnSide(parent.IFF), transform.position, Quaternion.identity);
        attacker = attackerObject.GetComponent<Attacker>();

        attacker.Initialize(parent.ToString() + "\'s Attack to " + target.ToString(), parent.IFF, carrier, target);
        carrier.LaunchAttacker();

        StartCoroutine(AttackCooldownTimer());
        StartCoroutine(WaitForAttackersToReturn());
    }

    private IEnumerator AttackCooldownTimer()
    {
        if (isAttackUnderway) yield break;
        isAttackUnderway = true;

        yield return attackCooldown;
        isAttackUnderway = false;
    }

    private IEnumerator WaitForAttackersToReturn()
    {
        IsAwaitingCraftToReturn = true;
        yield return waitDuration;
        
        if (!isAttackUnderway)
        {
            IsAwaitingCraftToReturn = false;
        }
        else
        {
            StartCoroutine(WaitForAttackersToReturn());// let's try again later
        }
    }

    private GameObject SelectPrefabBasedOnSide(IFF parentIFF)
    {
        switch (parentIFF)
        {
            case IFF.BLUFOR:
                return attackBluforPrefab;
            case IFF.OPFOR:
                return attackOpforPrefab;
            default:
                return attackOpforPrefab;
        }
    }
}
