using System;
using System.Collections;
using UnityEngine;

public class FogOfWar : MonoBehaviour, IDetectable
{
    private bool isFoWCoroutineRunning = false;

    private WaitForSeconds fogOfWarcoroutinePulseDelay = new WaitForSeconds(0.05f); // how often do we check for detection
    private WaitForSeconds detectionDecay = new WaitForSeconds(2f); // how long will the entity stay visible upon being detected

    public IMapEntity MapEntity { get; private set; }
    public IFF IFF { get => MapEntity.IFF; }
    public CircleCollider2D DetectableTriggerCollider2D { get; private set; }
    public bool IsDetectedByEnemy { get => isDetectedByEnemy; }
    [SerializeField]
    private bool isDetectedByEnemy;
    public event Action<IDetectable> OnDetection = delegate { };

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

    private void Update()
    {
        if (!isFoWCoroutineRunning)
        {
            StartCoroutine(FogOfWarCoroutine());
        }
    }

    private IEnumerator FogOfWarCoroutine()
    {
        isFoWCoroutineRunning = true;

        if (isDetectedByEnemy) // when detected
        {
            OnDetection?.Invoke(this);
            isDetectedByEnemy = false; // reset detected state until the next detection
            MapEntity.MakeVisible(true);
            yield return detectionDecay;

        }
        else
        {
            if (IFF == GameSettings.EnemySide)
            {
                MapEntity.MakeVisible(false); // hide if an undetected enemy
            }
            else
            {
                MapEntity.MakeVisible(true); // show everyone else
            }

            yield return fogOfWarcoroutinePulseDelay;
        }

        isFoWCoroutineRunning = false;
    }

    public void Reveal()
    {
        isDetectedByEnemy = true;
    }
}