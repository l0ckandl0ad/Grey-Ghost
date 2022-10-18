using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Repeats the function for n seconds for every frame. Update needs to be called.
/// </summary>
public class RepeatFunctionForSeconds
{
    private Action action;
    private float timer;
    private bool isDestroyed = false;

    public RepeatFunctionForSeconds(Action action, float timer)
    {
        this.action = action;
        this.timer = timer;
    }

    public void Update()
    {
        if (isDestroyed) return;

        timer -= Time.deltaTime;

        while (timer > 0)
        {
            action();
        }

        if (timer <= 0)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        isDestroyed = true;
    }
}
