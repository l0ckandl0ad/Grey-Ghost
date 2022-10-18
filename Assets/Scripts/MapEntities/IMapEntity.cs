using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapEntity
{
    IFF IFF { get; }
    UnitType UnitType { get; }
    float Speed { get; }  // should be in a separate interface, but whatever...
    GameObject GameObject { get; }
    Transform Transform { get; }
    bool IsVisible { get; } // part of Fog of War system
    void MakeVisible(bool trueOrFalse); // part of Fog of War system
    void SetSpeedMultiplier(float speedMultiplier); // modifies original entity speed by certain amount to achieve temp effect 
    /// <summary>
    /// Used for chaning speed of the carrier during deck ops.
    /// </summary>
    /// <param name="slowDownFactor">1 means 100% of the speed, 0.5 = 50% etc.</param>
    /// <param name="durationInSeconds">Duration for the slowdown.</param>
    void SlowDown(float slowDownFactor, float durationInSeconds);

}