using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryController : MonoBehaviour, IFactionComponent, IController
{
    private LayerMask _targetLayer;
    private ITarget _currentTarget;
    private TargetFinder _targetFinder;

    public float Range;
    public GameObject Turret;
    private ITurret _turret;
    public GameObject Weapon;
    private IWeapon _weapon;
    public float AimTolerance;

    private Ticker _targetFindingTicker;

    public bool Enabled { get => enabled; set => enabled = value; }

    private void Awake()
    {
        _targetFindingTicker = new Ticker(0.5f, TickerCallback);
        _targetFinder = new TargetFinder(go => CanHit(go));

        if (Turret)
        {
            _turret = Turret.GetComponent<ITurret>();
        }
        if (Weapon)
        {
            _weapon = Weapon.GetComponent<IWeapon>();
        }
    }

    private bool CanHit (GameObject go)
    {
        return Vector3.SqrMagnitude(go.transform.position - transform.position) < Range * Range
            && CanTurretHitOrNoTurret(go);
    }

    private bool CanTurretHitOrNoTurret (GameObject go)
    {
        if (_turret != null)
        {
            return _turret.CanHit(go.transform.position);
        }
        else
        {
            return true;
        }
    }

    public void SetFaction(Faction faction)
    {
        _targetLayer = faction.GetOtherLayerMasks();
    }

    private void FixedUpdate()
    {
        if (_currentTarget.ExistsAndValid() && _currentTarget is ColliderTarget colTarget)
        {
            if (CanHit(colTarget.Collider.gameObject))
            {
                float delta = 0f;
                if (_turret != null)
                {
                    _turret.AimTowards(_currentTarget.GetPosition());
                    delta = _turret.DeltaAngle(_currentTarget.GetPosition());
                }
                if (delta < AimTolerance && _weapon != null)
                {
                    _weapon.TryFire(_currentTarget);
                }
            }
            else
            {
                _currentTarget = null;
            }
        }
        else
        {
            _targetFindingTicker.Tick();
        }
    }

    private void TickerCallback()
    {
        _currentTarget = new ColliderTarget(_targetFinder.FindTarget(transform.position, Range, _targetLayer));
    }
}
