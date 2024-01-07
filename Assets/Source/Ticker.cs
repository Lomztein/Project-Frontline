using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker
{
    private const int TICKS_PER_SECOND = 50;
    private const float FIXED_DELTA_TIME = 0.02f;

    private float _currentTicks = 0;
    private int _targetTicks;
    private Action _callback;

    public Ticker (int ticks, Action callback)
    {
        _targetTicks = ticks;
        _callback = callback;
        _currentTicks = UnityEngine.Random.Range(0, _targetTicks);
    }

    public Ticker (float seconds, Action callback) : this(Mathf.RoundToInt(TICKS_PER_SECOND * seconds), callback)
    {
    }

    public void Tick ()
    {
        float currentFixedDeltaTime = Time.fixedDeltaTime;
        _currentTicks += currentFixedDeltaTime / FIXED_DELTA_TIME;
        if (_currentTicks >= _targetTicks)
        {
            _currentTicks = 0;
            _callback();
        }
    }
}
