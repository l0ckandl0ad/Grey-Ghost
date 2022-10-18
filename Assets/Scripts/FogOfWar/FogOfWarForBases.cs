using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarForBases : MonoBehaviour, IDetectable
{
    public IMapEntity MapEntity { get; private set; }
    public IFF IFF { get => MapEntity.IFF; }
    public CircleCollider2D DetectableTriggerCollider2D { get; private set; }
    public bool IsDetectedByEnemy { get => isDetectedByEnemy; }
    public event Action<IDetectable> OnDetection = delegate { };

    private bool isDetectedByEnemy;

    public void Reveal()
    {
        isDetectedByEnemy = true;
        //OnDetection?.Invoke(this); // they can't know they are being detected unless being hit, so no invoke needed here
    }

    private void Start()
    {
        CacheReferences();
        CreateDetectableTriggerCollider2D();
    }

    private void CacheReferences()
    {
        MapEntity = GetComponent<IMapEntity>();
    }
    private void CreateDetectableTriggerCollider2D()
    {
        if (DetectableTriggerCollider2D == null)
        {
            DetectableTriggerCollider2D = gameObject.AddComponent<CircleCollider2D>();
            DetectableTriggerCollider2D.isTrigger = true;
            DetectableTriggerCollider2D.radius = 0.001f;
        }
    }
}
