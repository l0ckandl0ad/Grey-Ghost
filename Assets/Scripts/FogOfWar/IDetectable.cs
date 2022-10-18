using System;
using UnityEngine;

public interface IDetectable
{
    IFF IFF { get; }
    IMapEntity MapEntity { get; }
    CircleCollider2D DetectableTriggerCollider2D { get; } // this collider needs to be set to IsTrigger!
    bool IsDetectedByEnemy { get; }
    void Reveal();

    event Action<IDetectable> OnDetection;
}