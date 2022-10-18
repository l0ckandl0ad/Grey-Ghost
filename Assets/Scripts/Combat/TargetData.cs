using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TargetData
{
    public IDamageable DamageableEnemy { get; private set; }
    public float Range { get; private set; }

    public TargetData(IMapEntity observer, IDamageable targetEntity)
    {
        DamageableEnemy = targetEntity;
        Range = Vector2.Distance(observer.Transform.position, targetEntity.Transform.position);
    }
}