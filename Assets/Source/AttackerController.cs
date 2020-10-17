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

    public bool Enabled { get => enabled; set => enabled = value; }

    private void Awake()
    {
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

    public void SetFaction(Faction faction)
    {
        _targetLayer = faction.GetOtherLayerMasks();
    }

    public void SetWaypoint (Waypoint waypoint)
    {
        _currentWaypoint = waypoint;
    }

    private void FixedUpdate()
    {
        if (_currentTarget.ExistsAndValid())
        {
            Vector3 local = _currentTarget.GetPosition() - transform.position;
            float sqrDist = Vector3.SqrMagnitude(local);
            float angle = Mathf.Clamp(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg), -AngleClamp, AngleClamp);
            float speed = 1f;

            if (Turret != null)
            {
                Turret.AimTowards(_currentTarget.GetPosition());

                if (Weapon != null)
                {
                    float aimDelta = Turret.DeltaAngle(_currentTarget.GetPosition());
                    if (sqrDist < AttackRange * AttackRange && aimDelta < AimTolerance)
                    {
                        Weapon.TryFire(_currentTarget);
                    }

                    if (sqrDist < HoldRange * HoldRange)
                    {
                        speed = 0f;
                    }

                    if (sqrDist > LooseTargetRange * LooseTargetRange)
                    {
                        _currentTarget = null;
                    }
                }
            }

            Controllable.Accelerate(speed);
            Controllable.Turn(angle / AngleClamp);
        }
        else
        {
            _currentTarget = new ColliderTarget (_targetFinder.FindTarget(transform.position, AcquireTargetRange, _targetLayer));

            if (_currentWaypoint)
            {
                Controllable.Accelerate(1f);
                float angle = Mathf.Clamp(Vector3.SignedAngle(transform.forward, _currentWaypoint.OutgoingVector, Vector3.up), -AngleClamp, AngleClamp);
                Controllable.Turn(angle);
            }
        }
    }
}
