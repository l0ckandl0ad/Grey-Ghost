using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatsPanel : MonoBehaviour
{
    private Player player;
    private int scorePoints;

    private WaitForSeconds waitDelay = new WaitForSeconds(15f);// how long will the VP award will be delayed

    private List<IDamageable> enemyUnits = new List<IDamageable>();
    private Ship[] enemyShips;
    private AirBase[] airBases;
    private NavalBase[] navalBases;
    private EnemyCarrier[] enemyCarriers;

    private int enemyCarriersAtStart = 0;
    private int enemyCarriersDestroyed = 0;


    [SerializeField]
    private TMP_Text health;
    [SerializeField]
    private TMP_Text ammo;
    [SerializeField]
    private TMP_Text attackers;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text enemyCarriersTracker;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        FindAllDamageableEnemies();
        RegisterForEvents();
    }
    private void OnDestroy()
    {
        UnregisterForEvents();
    }

    private void FindAllDamageableEnemies()
    {
        enemyShips = FindObjectsOfType<Ship>();
        airBases = FindObjectsOfType<AirBase>();
        navalBases = FindObjectsOfType<NavalBase>();
        enemyCarriers = FindObjectsOfType<EnemyCarrier>();

        enemyCarriersAtStart = enemyCarriers.Length;

        AddUnitsFromArrayToList(enemyShips, enemyUnits, IFF.OPFOR);
        AddUnitsFromArrayToList(airBases, enemyUnits, IFF.OPFOR);
        AddUnitsFromArrayToList(navalBases, enemyUnits, IFF.OPFOR);
    }

    private void AddUnitsFromArrayToList(object[] array, List<IDamageable> list, IFF side)
    {
        foreach (IDamageable unit in array)
        {
            if (unit.IFF == side)
            {
                list.Add(unit);
            }
        }
    }

    private void RegisterForEvents()
    {
        foreach (IDamageable unit in enemyUnits)
        {
            unit.OnHit += OnUnitHit;
            unit.OnDestroyed += OnUnitDestoyed;
        }
    }

    private void UnregisterForEvents()
    {
        foreach (IDamageable unit in enemyUnits)
        {
            unit.OnHit -= OnUnitHit;
            unit.OnDestroyed -= OnUnitDestoyed;
        }
    }

    private void OnUnitHit(IDamageable unit, int damage, int vpAmount)
    {
        StartCoroutine(AwardPointsWithDelay(unit, vpAmount));
    }
    private void OnUnitDestoyed(IDamageable unit, int damage, int vpAmount)
    {
        StartCoroutine(AwardPointsWithDelay(unit, vpAmount, true));

        unit.OnHit -= OnUnitHit;
        unit.OnDestroyed -= OnUnitDestoyed;
        enemyUnits.Remove(unit);
    }

    private void Update()
    {
        UpdatePlayerStatsDisplayed();
    }

    private void UpdatePlayerStatsDisplayed()
    {
        if (player == null || player.Equals(null)) return;

        health.text = player.Health.ToString();
        ammo.text = player.Ammo.ToString();
        attackers.text = player.AttackersAvailable.ToString();
        scoreText.text = scorePoints.ToString();

        DisplayEnemyCarriersTrackerText();
    }

    private IEnumerator AwardPointsWithDelay(IDamageable unit, int vpAmount, bool isDestroyed = false)
    {
        yield return waitDelay;

        scorePoints += vpAmount;

        if (isDestroyed)
        {
            CheckIfEnemyCarrierWasDestroyed(unit);
        }
    }

    private void CheckIfEnemyCarrierWasDestroyed(IDamageable unit)
    {
        if (unit is EnemyCarrier)
        {
            enemyCarriersDestroyed++;
        }
    }

    private void DisplayEnemyCarriersTrackerText()
    {
        enemyCarriersTracker.text = enemyCarriersDestroyed.ToString() + "/" + enemyCarriersAtStart.ToString();
    }
}
