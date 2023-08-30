using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class BuilderController : ControllableController, ICommanderComponent, ITeamComponent
{
    public enum BuildArea { 
        Frontline, // Builds along the frontline. 
        Defense, // Builds in front of the base.
        Headquarters // Builds around the headquaters.
    }

    public BuildArea Area;
    public float BuildOffset;
    public float BuildDistanceThreshold;
    public float BuildRange;
    public float BuildSize;
    public bool BuildOnDeath;
    private Health _health;

    private BuilderWeapon[] _factoryWeapons;
    private Vector3? _nextBuildPosition;

    private Commander _commander;

    protected override void Start()
    {
        base.Start();
        foreach (var wep in Weapons)
        {
            if (wep is not BuilderWeapon)
            {
                throw new System.Exception("Builders should only carry BuilderWeapon weapons.");
            }
        }
        _factoryWeapons = Weapons.Select(x => x as BuilderWeapon).ToArray();
        _health = GetComponent<Health>();
        if (BuildOnDeath)
        {
            _health.OnDeath += OnDeath;
        }
    }

    private void OnDeath(Health obj)
    {
        foreach (var weapon in _factoryWeapons)
        {
            weapon.TryFire(null);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_nextBuildPosition.HasValue)
        {
            float distToBuildPos = Vector3.SqrMagnitude(_nextBuildPosition.Value - transform.position);
            if (distToBuildPos < BuildRange * BuildRange) 
            {
                if (_factoryWeapons.All(x => x.CanFire()))
                {
                    foreach (var weapon in _factoryWeapons)
                    {
                        weapon.TryFire(null);
                    }
                    _nextBuildPosition = null;
                }
                Stop();
            }
            else
            {
                MoveTowardsPosition(_nextBuildPosition.Value);
            }
        }
        else
        {
            _nextBuildPosition = GetNextBuildPosition();
        }
    }

    private Vector3? GetNextBuildPosition ()
    {
        switch (Area)
        {
            case BuildArea.Frontline: return GetFrontlineBuildPosition();
            case BuildArea.Headquarters: return GetHeadquartersBuildPosition();
            case BuildArea.Defense: return GetDefenseBuildPosition();
        }
        throw new System.Exception("Invalid build area selected.");
    }

    private Vector3? GetFrontlineBuildPosition ()
    {
        throw new NotImplementedException();
    }

    private Vector3? GetDefenseBuildPosition ()
    {
        Vector3 pos = _commander.DefenseVolumeLocalBounds.RandomPointInside().Flat() + Vector3.forward * BuildOffset;
        pos = _commander.transform.TransformPoint(pos);
        return pos;
    }

    private Vector3 GetHeadquartersBuildPosition ()
    {
        throw new NotImplementedException();
    }

    public void AssignCommander(Commander commander)
    {
        _commander = commander;
    }
}
