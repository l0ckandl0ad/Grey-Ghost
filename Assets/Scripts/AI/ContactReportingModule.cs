using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attach this script to map entities that need to report enemy contact (dummy) when they are attacked or detected and have but can't detect enemies on their own.
/// </summary>
public class ContactReportingModule : MonoBehaviour
{
    private IDamageable reporter;
    private Detection detection;
    private IFF iff;

    private IDetectable detectable;
    private AISharedData sharedData;

    private WaitForSeconds reportingDelay = new WaitForSeconds(0.5f); 

    private void Start()
    {
        reporter = GetComponent<IDamageable>();
        detection = GetComponent<Detection>();
        iff = reporter.IFF;
        detectable = GetComponent<IDetectable>();
        sharedData = FindObjectOfType<AISharedData>();

        RegisterForEvents();
    }

    private void RegisterForEvents()
    {
        reporter.OnHit += OnHitEventHandler;
        detectable.OnDetection += OnDetectionEventHandler;
    }

    private void UnregisterFromEvents()
    {
        reporter.OnHit -= OnHitEventHandler;
        detectable.OnDetection -= OnDetectionEventHandler;
    }

    private void OnHitEventHandler(IDamageable reporter, int whatever, int whateverElse)
    {
        ReportContact();
    }
    private void OnDetectionEventHandler(IDetectable reporter)
    {
        ReportContact();
    }

    private void ReportContact()
    {
        if (detection.SelectTarget(true) == null)
        {
            StartCoroutine(ReportContactRoutine());
        }
    }

    private void OnDestroy()
    {
        UnregisterFromEvents();
    }

    private IEnumerator ReportContactRoutine()
    {
        yield return reportingDelay;
        sharedData.ReportContact(iff, transform.position);
    }

}
