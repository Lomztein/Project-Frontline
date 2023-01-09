using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontline
{
    private Vector3?[] _lastEvents;
    private Vector3?[] _lastChanges;

    private int _killIndex;
    private int _positionWindowSize;

    private int _changeIndex;
    private int _changeWindowSize;

    public Vector3 Position => GetPosition();
    public Vector3 Change => GetChange();

    private Vector3 _cache;
    private bool _dirty;
    private Vector3 _change;

    private int _numRegisters;

    public Frontline (int positionWindowSize, int changeWindowSize)
    {
        _positionWindowSize = positionWindowSize;
        _changeWindowSize = changeWindowSize;

        _lastEvents = new Vector3?[_positionWindowSize];
        _lastChanges = new Vector3?[_changeWindowSize];
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

    public void Register(Vector3 position)
    {
        _lastEvents[_killIndex] = position;
        _killIndex++;
        if (_killIndex >= _positionWindowSize) _killIndex = 0;
        _numRegisters++;
        _dirty = true;
    }
    private void UpdatePosition()
    {
        Vector3 pos = Vector3.zero;
        foreach (var @event in _lastEvents) if (@event.HasValue) pos += @event.Value;
        pos /= Mathf.Min(_positionWindowSize, _numRegisters);

        RegisterChange(pos - _cache);
        _cache = pos;

        Vector3 change = Vector3.zero;
        foreach (var delta in _lastChanges) if (delta.HasValue) change += delta.Value;

        _change /= Mathf.Min(_positionWindowSize, _numRegisters);

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
