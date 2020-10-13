﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : MonoBehaviour, IFactionComponent
{
    private LayerMask _targetLayer;
    private TargetFinder _targetFinder = new TargetFinder();

    private GameObject _currentTarget;
    private Collider _targetCol;

    public float AcquireTargetRange;
    public float LooseTargetRange;
    public float AttackRange;

    public IControllable Controllable;
    public Turret Turret;
    public float AimTolerance;
    public Weapon Weapon;
    public float AngleClamp;

    private void Awake()
    {
        Controllable = GetComponent<IControllable>();
    }

    public void SetFaction(Faction faction)
    {
        _targetLayer = faction.GetOtherLayerMasks();
    }

    private void FixedUpdate()
    {
        if (_currentTarget)
        {
            Vector3 local = _currentTarget.transform.position - transform.position;
            float sqrDist = Vector3.SqrMagnitude(local);
            float angle = Mathf.Clamp (Mathf.DeltaAngle (transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg), -AngleClamp, AngleClamp);
            float speed = 1f;

            float aimDelta = Turret.AimTowards(_targetCol.bounds.center);
            if (sqrDist < AttackRange * AttackRange && aimDelta < AimTolerance)
            {
                Weapon.TryFire();
                speed = 0f;
            }

            if (sqrDist > LooseTargetRange * LooseTargetRange)
            {
                _currentTarget = null;
            }

            Controllable.Accelerate(speed);
            Controllable.Turn(angle / AngleClamp);
        }
        else
        {
            _currentTarget = _targetFinder.FindTarget(transform.position, AcquireTargetRange, _targetLayer);
            if (_currentTarget)
            {
                _targetCol = _currentTarget.GetComponentInChildren<Collider>();
            }
        }
    }
}