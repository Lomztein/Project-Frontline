using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour, IFactionComponent
{
    private LayerMask _targetLayer;
    private GameObject _currentTarget;
    private TargetFinder _targetFinder;

    public float Range;
    public Turret Turret;
    public Weapon Weapon;
    public float AimTolerance;

    private void Awake()
    {
        _targetFinder = new TargetFinder(go => CanHit(go));
    }

    private bool CanHit (GameObject go)
    {
        return Vector3.SqrMagnitude(go.transform.position - transform.position) < Range * Range
            && Turret.CanHit(go.transform.position);
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
                float delta = Turret.AimTowards(_currentTarget.transform.position);
                if (delta < AimTolerance)
                {
                    Weapon.TryFire();
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
        }
    }
}
