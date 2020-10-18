using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker
{
    private float TicksPerSecondFixedTime => 1f / Time.fixedDeltaTime;

    private int _currentTicks = 0;
    private int _targetTicks;
    private Action _callback;

    public Ticker (int ticks, Action callback)
    {
        _targetTicks = ticks;
        _callback = callback;
    }

    public Ticker (float seconds, Action callback)
    {
        _targetTicks = Mathf.RoundToInt(TicksPerSecondFixedTime * seconds);
        _callback = callback;
    }

    public void Tick ()
    {
        _currentTicks++;
        if (_currentTicks >= _targetTicks)
        {
            _currentTicks = 0;
            _callback();
        }
    }
}
