using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryController : MonoBehaviour, IFactionComponent, IController
{
    private LayerMask _targetLayer;
    private GameObject _currentTarget;
    private Collider _targetCollider;
    private TargetFinder _targetFinder;

    public float Range;
    public GameObject Turret;
    private ITurret _turret;
    public GameObject Weapon;
    private IWeapon _weapon;
    public float AimTolerance;

    public bool Enabled { get => enabled; set => enabled = value; }

    private void Awake()
    {
        _targetFinder = new TargetFinder(go => CanHit(go));

        _turret = Turret.GetComponent<ITurret>();
        if (Weapon)
        {
            _weapon = Weapon.GetComponent<IWeapon>();
        }
    }

    private bool CanHit (GameObject go)
    {
        return Vector3.SqrMagnitude(go.transform.position - transform.position) < Range * Range
            && _turret.CanHit(go.transform.position);
    }

    public void SetFaction(Faction faction)
    {
        _targetLayer = faction.GetOtherLayerMasks();
    }

    private void FixedUpdate()
    {
        if (_currentTarget)
        {
            if (CanHit(_currentTarget))
            {
                _turret.AimTowards(_targetCollider.bounds.center);
                float delta = _turret.DeltaAngle(_targetCollider.bounds.center);
                if (delta < AimTolerance && _weapon != null)
                {
                    _weapon.TryFire();
                }
            }
            else
            {
                _currentTarget = null;
            }
        }
        else
        {
            _currentTarget = _targetFinder.FindTarget(transform.position, Range, _targetLayer);
            if (_currentTarget)
            {
                _targetCollider = _currentTarget.GetComponentInChildren<Collider>();
            }
        }
    }
}
