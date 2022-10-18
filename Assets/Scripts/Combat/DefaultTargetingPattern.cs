using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DefaultTargetingPattern : ITargetingPattern
{

    // THIS IS AN EXAMPLE CODE FROM THE ALPHA STRIKE PROJECT:
    //
    //public List<TargetData> SortTargets(List<TargetData> availableTargets)
    //{
    //    // sort them by size and range
    //    // refactor note - introduce THREAT or some other kind of priority other than size?
    //    // missiles? carriers? transports? destroyers?
    //    // currently it would be -> carrier > transport > destroyer > missile and by range within size
    //    // ie missiles won't be engaged before carriers because they are smaller targets, even if they're closer
    //    List<TargetData> sortedTargets = availableTargets.OrderByDescending(target => target.TargetableEntity.MinSize)
    //        .ThenBy(target => target.Range).ToList();

    //    return sortedTargets;
    //}

    private List<TargetData> SortTargets(List<TargetData> availableTargets)
    {
        List<TargetData> sortedTargets = availableTargets.OrderByDescending(target => target.DamageableEnemy.VPCost)
            .ThenBy(target => target.Range).ToList();

        return sortedTargets;
    }

    public List<TargetData> GenerateTargetData(IMapEntity observer, List<IDamageable> availableTargets)
    {
        List<TargetData> targets = new List<TargetData>();

        foreach (IDamageable target in availableTargets)
        {
            if (target != null && !target.Equals(null))
            {
                if (target.Health > 0)
                {
                    targets.Add(new TargetData(observer, target));
                }
            }
        }

        targets = SortTargets(targets);

        return targets;
    }
}
