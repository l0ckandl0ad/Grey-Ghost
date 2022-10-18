using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This should be a singleton, but I don't have time to implement it. Only one instance of this script should be in the scene.
/// </summary>
public class AISharedData : MonoBehaviour
{
    [SerializeField]
    private GameObject dummyTargetPrefab;

    public List<IDamageable> BLUFORTargetList { get; private set; }
    public List<IDamageable> OPFORTargetList { get; private set; }

    private float targetDecayTimeInSeconds = 5f; // how much time will pass before the target will disappear from shared network list
    private WaitForSeconds targetDecayTimer;

    private IDamageable tempTarget;
    private List<TargetData> tempBLUFORTargetData;
    private List<TargetData> tempOPFORTargetData;
    private ITargetingPattern targetingPattern;

    private void Awake()
    {
        BLUFORTargetList = new List<IDamageable>();
        OPFORTargetList =  new List<IDamageable>();
        targetDecayTimer = new WaitForSeconds(targetDecayTimeInSeconds);
        targetingPattern = new DefaultTargetingPattern();
    }

    public void ReportTarget(IFF reportingSide, IDamageable target)
    {
        switch (reportingSide)
        {
            case IFF.BLUFOR:
                BLUFORTargetList.Add(target);
                break;
            case IFF.OPFOR:
                OPFORTargetList.Add(target);
                break;
            default:
                // do nothing
                break;
        }
        StartCoroutine(TargetDecayAfterTimeout(reportingSide, target));
    }

    public void ReportContact(IFF reporterIFF, Vector3 position)
    {
        GameObject dummyObject = Instantiate(dummyTargetPrefab, position, Quaternion.identity);
        DummyTarget dummyObjectScript = dummyObject.GetComponent<DummyTarget>();
        IDamageable target = dummyObject.GetComponent<IDamageable>();

        dummyObjectScript.Initialize(GameSettings.GetOpponentSide(reporterIFF), position);
        ReportTarget(reporterIFF, target); // is it even needed?
    }

    public IDamageable GetTarget(IMapEntity caller)
    {
        tempTarget = null; // reset from previously known datas

        switch (caller.IFF)
        {
            case IFF.BLUFOR:
                if (BLUFORTargetList.Count > 0)
                {
                    tempBLUFORTargetData = targetingPattern.GenerateTargetData(caller, BLUFORTargetList);
                    if (tempBLUFORTargetData.Count == 0) break;
                    tempTarget = tempBLUFORTargetData[0].DamageableEnemy;
                }
                break;
            case IFF.OPFOR:
                if (OPFORTargetList.Count > 0)
                {
                    tempOPFORTargetData = targetingPattern.GenerateTargetData(caller, OPFORTargetList);
                    if (tempOPFORTargetData.Count == 0) break;
                    tempTarget = tempOPFORTargetData[0].DamageableEnemy;
                } 
                break;
            default:
                tempTarget = null;
                break;
        }

        return tempTarget;

    }

    private IEnumerator TargetDecayAfterTimeout(IFF reportingSide, IDamageable target)
    {
        yield return targetDecayTimer;

        if (target == null || target.Equals(null)) yield break;

        switch (reportingSide)
        {
            case IFF.BLUFOR:
                if (BLUFORTargetList.Contains(target))
                {
                    BLUFORTargetList.Remove(target);
                }
                break;
            case IFF.OPFOR:
                if (OPFORTargetList.Contains(target))
                {
                    OPFORTargetList.Remove(target);
                }
                break;
            default:
                // do nothing
                break;
        }
    }
}
