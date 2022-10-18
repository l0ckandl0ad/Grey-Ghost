using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBaseAI : MonoBehaviour
{
    private bool isFarSearchRunning = false;
    private bool isAttacking = false;
    private WaitForSeconds searchCooldown = new WaitForSeconds(10f);
    private WaitForSeconds attackDelay = new WaitForSeconds(1.5f);

    private Detection detection;
    private AttackAbility attackAbility;
    private ICarrier carrier;

    private IDamageable target;

    [SerializeField]
    private float ammoRestockPool;
    [SerializeField]
    private float ammoRestockSpeed = 0.01f; // amount of ammo restock per second
    [SerializeField]
    private float attackersRestockPool;
    [SerializeField]
    private float attackersRestockSpeed = 0.01f;

    private void Awake()
    {
        detection = GetComponent<Detection>();
        attackAbility = GetComponent<AttackAbility>();
        carrier = GetComponent<ICarrier>();
    }

    private void Update()
    {
        DoFarSearch();
        AttackDetectedUnits();
        RestockAmmo();
        RestockAttackers();
    }

    private void DoFarSearch()
    {
        if (!isFarSearchRunning)
        {
            StartCoroutine(FarSearchPulse());
        }
    }

    private void AttackDetectedUnits()
    {
        if (isAttacking) return;

        target = detection.SelectTarget(true);

        if (target == null || target.Equals(null)) return;

        if (!target.IsAttackable) return;

        StartCoroutine(AttackDetectedUnitsCoroutine(target.Transform.position));
    }
    private IEnumerator AttackDetectedUnitsCoroutine(Vector3 target)
    {
        isAttacking = true;
        yield return attackDelay;

        attackAbility.InitiateAttack(target);
        isAttacking = false;
    }

    private IEnumerator FarSearchPulse()
    {
        isFarSearchRunning = true;
        detection.FarSearch();

        yield return searchCooldown;

        isFarSearchRunning = false;
    }

    private void RestockAmmo()
    {
        if (carrier.Ammo < carrier.MaxAmmo)
        {
            ammoRestockPool += ammoRestockSpeed * Time.deltaTime;

            if (ammoRestockPool >= 1 && carrier.Ammo +1 <= carrier.MaxAmmo)
            {
                carrier.Replenish(0, 1);
                ammoRestockPool--;
            }
        }
    }

    private void RestockAttackers()
    {
        if (carrier.AttackersAvailable < carrier.MaxAttackers)
        {
            attackersRestockPool += attackersRestockSpeed * Time.deltaTime;

            if (attackersRestockPool >= 1 && carrier.AttackersAvailable + 1 <= carrier.MaxAttackers)
            {
                carrier.Replenish(1, 0);
                attackersRestockPool--;
            }
        }
    }
}
