using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    [SerializeField]
    private bool isScanRangeVisible;
    [SerializeField]
    private GameObject scan;
    [SerializeField]
    private Material sensorMaterial;
    private IFF scanerIFF;
    private bool isBasicSearchCoroutineRunning = false;
    [SerializeField]
    private bool isFarSearchCoroutineRunning = false;
    private float basicScanRadius = 1f;
    private float farScanRadius = 9f;
    private float tempScanRadius = 0f;
    private float farScanSpeed = 2.5f;
    private WaitForSeconds sensorPulseDelay = new WaitForSeconds(0.05f);

    private float deckOpsSlowDownRatio = 0.1f; // how much would the carrier slowdown during scout deployment
    private float deckOpsDuration = 1.5f; // how long does it take for scouts to takeoff and land for far search

    public IMapEntity MapEntity { get; private set; }
    public ICarrier Carrier { get; private set; }
    public List<IMapEntity> DetectedEntityList = new List<IMapEntity>();
    public List<IDamageable> AttackableTargetsList = new List<IDamageable>();
    private IDamageable tempTarget;


    private AISharedData sharedData;
    private ITargetingPattern targetingPattern;
    private List<TargetData> targetData;

    public void FarSearch()
    {
        if (isFarSearchCoroutineRunning || Carrier.Health <= 0 || !Carrier.IsAbleToAttack) return;

        StartCoroutine(FarSearchPulse());
    }

    private void Start() // we do this in start to allow parent entity to initialize IMapEntity on this gameObject
    {
        CacheReferences();

        if (!isScanRangeVisible) return;
        UILibrary.DrawCircle(this.gameObject, basicScanRadius, 0.01f, sensorMaterial); // temp
        ChangeCircle(0); // hide scanning circle at start
    }

    private void CacheReferences()
    {
        MapEntity = GetComponent<IMapEntity>();

        if (TryGetComponent<ICarrier>(out ICarrier carrier))
        {
            Carrier = carrier;
        }

        sharedData = FindObjectOfType<AISharedData>();
        scanerIFF = MapEntity.IFF;

        targetingPattern = new DefaultTargetingPattern();
    }

    private void Scan(float scanRadius)
    {
        DetectedEntityList.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, scanRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.isTrigger)
            {
                if (collider.gameObject.TryGetComponent(out IDetectable detectableEntity))
                {
                    if (detectableEntity.IFF == GameSettings.GetOpponentSide(scanerIFF))
                    {
                        detectableEntity.Reveal();
                        DetectedEntityList.Add(detectableEntity.MapEntity);
                    }
                }
            }
        }

        AssesNavalTargets();
    }

    private void AssesNavalTargets()
    {
        AttackableTargetsList.Clear();

        if (DetectedEntityList.Count == 0) return;

        foreach (IMapEntity target in DetectedEntityList)
        {
            if (target is IDamageable damagebleTarget && damagebleTarget.Health > 0)
            {
                AttackableTargetsList.Add(damagebleTarget);
                sharedData.ReportTarget(scanerIFF, damagebleTarget);
            }
            
        }
    }
    /// <summary>
    /// Should either return a target to attack/chase or null if nothing good found. Boolean useSharedData dictates wether the shared list of targets 
    /// known to all other agents would be used (detected targets network).
    /// </summary>
    /// <returns></returns>
    public IDamageable SelectTarget(bool useSharedData)
    {
        if (AttackableTargetsList.Count == 0 && !useSharedData) return null;

        if (AttackableTargetsList.Count == 0 && useSharedData)
        {
            tempTarget = sharedData.GetTarget(MapEntity);
        }

        if (AttackableTargetsList.Count > 0)
        {
            if (AttackableTargetsList[0] != null || !AttackableTargetsList[0].Equals(null))
            {
                targetData = targetingPattern.GenerateTargetData(MapEntity, AttackableTargetsList);

                if (targetData.Count == 0) return null;
                
                if (targetData[0].DamageableEnemy != null || !targetData[0].DamageableEnemy.Equals(null))
                {
                    tempTarget = targetData[0].DamageableEnemy;
                }
            }

        }

        return tempTarget;
    }

    private void Update()
    {
        BasicSearch();
    }

    private void BasicSearch()
    {
        if (!isBasicSearchCoroutineRunning)
        {
            StartCoroutine(BaseSearchPulse());
        }
    }
    private IEnumerator BaseSearchPulse()
    {
        isBasicSearchCoroutineRunning = true;
        Scan(basicScanRadius);
        yield return sensorPulseDelay;
        isBasicSearchCoroutineRunning = false;
    }

    private IEnumerator FarSearchPulse()
    {
        isFarSearchCoroutineRunning = true;

        Carrier.SlowDown(deckOpsSlowDownRatio, deckOpsDuration); // slow down the entity while scouts take off

        tempScanRadius = 0;

        while (tempScanRadius < farScanRadius-0.1f)
        {
            tempScanRadius += 0.1f * farScanSpeed;
            ChangeCircle(tempScanRadius * 2f);
            Scan(tempScanRadius);
            yield return sensorPulseDelay;
        }

        while (tempScanRadius > 0 + 0.1f)
        {
            tempScanRadius -= 0.1f * farScanSpeed;
            ChangeCircle(tempScanRadius * 2f);
            Scan(tempScanRadius);
            yield return sensorPulseDelay;
        }

        Carrier.SlowDown(deckOpsSlowDownRatio, deckOpsDuration); // slow down the entity while scouts land

        ChangeCircle(0);

        isFarSearchCoroutineRunning = false;
    }

    private void ChangeCircle(float scale)
    {
        if (scan != null)
        {
            scan.transform.localScale = new Vector3(scale, scale, 1);
        }
    }

}
