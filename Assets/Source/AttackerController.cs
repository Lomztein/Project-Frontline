using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : MonoBehaviour, IFactionComponent, IController
{
    private LayerMask _targetLayer;
    private TargetFinder _targetFinder = new TargetFinder();

    private ITarget _currentTarget;

    public float AcquireTargetRange;
    public float LooseTargetRange;
    public float HoldRange;
    public float AttackRange;

    public IControllable Controllable;
    public GameObject TurretObject;
    public ITurret Turret;
    public float AimTolerance;
    public GameObject WeaponObject;
    public IWeapon Weapon;
    public float AngleClamp;

    private Waypoint _currentWaypoint;
    private Ticker _targetFindingTicker;
    private float _aimDelta;

    public bool Enabled { get => enabled; set => enabled = value; }

    private void Awake()
    {
        _targetFindingTicker = new Ticker(0.25f, TickerCallback);

        Controllable = GetComponentInChildren<IControllable>();
        if (TurretObject)
        {
            Turret = TurretObject.GetComponent<ITurret>();
        }
        if (WeaponObject)
        {
            Weapon = WeaponObject.GetComponent<IWeapon>();
        }
    }

    private void TickerCallback()
    {
        _currentTarget = new ColliderTarget(_targetFinder.FindTarget(transform.position, AcquireTargetRange, _targetLayer));
    }

    public void SetFaction(Faction faction)
    {
        _targetLayer = faction.GetOtherLayerMasks();
    }

    public void SetWaypoint (Waypoint waypoint)
    {
        _currentWaypoint = waypoint;
    }

    protected virtual void Aim()
    {
        if (Turret != null)
        {
            Turret.AimTowards(_currentTarget.GetPosition());
            _aimDelta = Turret.DeltaAngle(_currentTarget.GetPosition());
        }
        else
        {
            Vector3 local = GetTargetLocalPosition();
            _aimDelta = Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg);
        }
    }

    protected Vector3 GetTargetLocalPosition() => _currentTarget.GetPosition() - transform.position;
    protected float GetTargetSquareDistance() => Vector3.SqrMagnitude(GetTargetLocalPosition());

    protected virtual void Attack()
    {
        if (Weapon != null)
        {
            float sqrDist = GetTargetSquareDistance();
            if (sqrDist < AttackRange * AttackRange && _aimDelta <= AimTolerance)
            {
                Weapon.TryFire(_currentTarget);
            }
        }
    }

    protected virtual void MoveAlongWaypoints ()
    {
        if (_currentWaypoint)
        {
            Controllable.Accelerate(1f);
            float angle = Mathf.Clamp(Vector3.SignedAngle(transform.forward, _currentWaypoint.OutgoingVector, Vector3.up), -AngleClamp, AngleClamp);
            Controllable.Turn(angle);
        }
    }

    protected virtual void MoveTowardsTarget ()
    {
        Vector3 local = GetTargetLocalPosition();
        float angle = Mathf.Clamp(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg), -AngleClamp, AngleClamp);
        float speed = 1f;

        if (local.sqrMagnitude < HoldRange * HoldRange)
        {
            speed = 0f;
        }

        Controllable.Accelerate(speed);
        Controllable.Turn(angle / AngleClamp);
    }

    private void FixedUpdate()
    {
        if (_currentTarget.ExistsAndValid())
        {
            Aim();
            Attack();
            MoveTowardsTarget();

            if (GetTargetSquareDistance() > LooseTargetRange * LooseTargetRange)
            {
                _currentTarget = null;
            }
        }
        else
        {
            _targetFindingTicker.Tick();
            MoveAlongWaypoints();
        }
    }
}
