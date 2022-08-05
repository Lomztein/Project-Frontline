using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontline
{
    private Vector3[] _lastKills;
    private Vector3[] _lastDeaths;
    private Vector3[] _lastChanges;

    private int _killIndex;
    private int _deathIndex;
    private int _positionWindowSize;

    private int _changeIndex;
    private int _changeWindowSize;

    public Vector3 Position => GetPosition();
    public Vector3 Change => GetChange();

    private Vector3 _cache;
    private bool _dirty;
    private Vector3 _change;

    public Frontline (int positionWindowSize, int changeWindowSize)
    {
        _positionWindowSize = positionWindowSize;
        _changeWindowSize = changeWindowSize;

        _lastKills = new Vector3[_positionWindowSize];
        _lastDeaths = new Vector3[_positionWindowSize];
        _lastChanges = new Vector3[_changeWindowSize];
    }

    public Vector3 GetPosition ()
    {
        if (_dirty) UpdatePosition();
        _dirty = false;
        return _cache;
    }

    public Vector3 GetChange ()
    {
        if (_dirty) UpdatePosition();
        _dirty = false;
        return _change;
    }

    public void RegisterKill(Vector3 position)
    {
        _lastKills[_killIndex] = position;
        _killIndex++;
        if (_killIndex >= _positionWindowSize) _killIndex = 0;
        _dirty = true;
    }

    public void RegisterDeath (Vector3 position)
    {
        _lastDeaths[_deathIndex] = position;
        _deathIndex++;
        if (_deathIndex >= _positionWindowSize) _deathIndex = 0;
        _dirty = true;
    }

    private void UpdatePosition()
    {
        Vector3 pos = Vector3.zero;
        foreach (var kill in _lastKills) pos += kill;
        foreach (var death in _lastDeaths) pos += death;
        pos /= _positionWindowSize;

        RegisterChange(pos - _cache);
        _cache = pos;

        Vector3 change = Vector3.zero;
        foreach (var delta in _lastChanges) change += delta;

        _change = change / _changeWindowSize;

        _dirty = false;
    }

    private void RegisterChange(Vector3 change)
    {
        _lastChanges[_changeIndex] = change;
        _changeIndex++;
        if (_changeIndex >= _changeWindowSize) _changeIndex = 0;
        _dirty = true;
    }

}
