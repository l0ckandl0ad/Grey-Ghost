using System.Collections.Generic;

public interface ITargetingPattern
{
    List<TargetData> GenerateTargetData(IMapEntity observer, List<IDamageable> availableTargets);
}
