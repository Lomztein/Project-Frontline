using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker
{
    private const float DEFAULT_FIXED_DELTA_TIME = 0.02f;
    private float TicksPerSecondFixedTime => 1f / DEFAULT_FIXED_DELTA_TIME;

    private float _currentTicks = 0;
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
        float currentFixedDeltaTime = Time.fixedDeltaTime;
        _currentTicks += currentFixedDeltaTime / DEFAULT_FIXED_DELTA_TIME;
        if (_currentTicks >= _targetTicks)
        {
            _currentTicks = 0;
            _callback();
        }
    }
}
