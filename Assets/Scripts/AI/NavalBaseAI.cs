using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is not really an AI, I guess, but the base is a unit and it does interact with other units on its own.
/// With ships nearby, each frame the naval base would generate a certain amount of repair and replenishment points which would be accumulated.
/// When certain threshold would be reached, the points would be split between all units present.
/// </summary>
public class NavalBaseAI : MonoBehaviour
{
    private List<IMapEntity> nearbyShips = new List<IMapEntity>();
    private IFF scanerIFF;
    //private IMapEntity mapEntity;
    private IDamageable damageableBase;

    [SerializeField]
    private Material sensorMaterial; // used to draw a circle that depicts the scanning radius
    
    private float repairPointsPool;
    private float ammoPointsPool;
    private float attackerPointPool;

    // speed is in points per second
    // distance is in map units
    private float basicScanRadius = 1f; // how far out can the base detect ships
    private float slowDownFactor = 0.1f; // how much the ships within the range of the base would be slowed down
    private float slowDownDuration = 0.5f; // how long will the unit slow down for after each call
    private float repairSpeed = 0.25f; // how fast will the ships be repaired
    private float replenishmentSpeed = 0.25f;

    private void Start()
    {
        damageableBase = GetComponent<IDamageable>();
        scanerIFF = damageableBase.IFF;

        UILibrary.DrawCircle(this.gameObject, basicScanRadius, 0.01f, sensorMaterial); // temp
    }

    private void FixedUpdate()
    {
        if (damageableBase.Health <= 0) return;

        FindNearbyShips();
        AffectNearbyShips();
    }

    private void FindNearbyShips()
    {
        {
            nearbyShips.Clear();

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, basicScanRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger)
                {
                    if (collider.gameObject.TryGetComponent(out IMapEntity detectedEntity))
                    {
                        if (detectedEntity.IFF == scanerIFF && detectedEntity.UnitType == UnitType.NAVAL)
                        {
                            nearbyShips.Add(detectedEntity);
                        }
                    }
                }
            }
        }
    }

    private void AffectNearbyShips()
    {
        if (nearbyShips.Count == 0)
        {
            ResetRepairAndReplenishmentPool();
            return;
        }


        foreach (IMapEntity ship in nearbyShips)
        {
            if (ship == null) return;

            ApplyEffectToAShip(ship);
        }
    }

    private void ResetRepairAndReplenishmentPool()
    {
        repairPointsPool = 0f;
        ammoPointsPool = 0f;
        attackerPointPool = 0f;
    }

    private void GenerateRepairPoints()
    {
        repairPointsPool += repairSpeed * Time.fixedDeltaTime;
    }
    private void GenerateAmmoPoints()
    {
        ammoPointsPool += replenishmentSpeed * Time.fixedDeltaTime;
    }
    private void GenerateAttackerPoints()
    {
        attackerPointPool += replenishmentSpeed * Time.fixedDeltaTime;
    }

    private void ApplyEffectToAShip(IMapEntity ship)
    {
        ship.SlowDown(slowDownFactor, slowDownDuration);
        RepairDamagedShip(ship);
        ReplenishShip(ship);
    }

    private void RepairDamagedShip(IMapEntity ship)
    {
        if (ship is IDamageable damageableShip)
        {
            if (damageableShip.Health > 0 && damageableShip.Health < damageableShip.MaxHealth)
            {
                GenerateRepairPoints();

                if (repairPointsPool >= 1)
                {
                    damageableShip.Repair(1);
                    repairPointsPool--;
                }

            }
        }
    }

    private void ReplenishShip(IMapEntity ship)
    {
        if (ship is ICarrier carrier)
        {
            if (carrier.Ammo < carrier.MaxAmmo)
            {
                GenerateAmmoPoints();

                if (ammoPointsPool >= 1)
                {
                    carrier.Replenish(0, 1);
                    ammoPointsPool--;
                }

            }

            if (carrier.AttackersAvailable < carrier.MaxAttackers)
            {
                GenerateAttackerPoints();

                if (attackerPointPool >= 1)
                {
                    carrier.Replenish(1, 0);
                    attackerPointPool--;
                }
            }
        }
    }


}
