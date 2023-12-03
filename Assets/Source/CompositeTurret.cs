using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeTurret : MonoBehaviour, ITurret
{
    public GameObject[] Turrets;
    private List<ITurret> _turrets;

    public enum DeltaAngleAggregation { Min, Max, Zero }
    public enum CanHitAggregation { Any, All, Always }

    public DeltaAngleAggregation UseDeltaAngle;
    public CanHitAggregation UseCanHit;

    public void AimTowards(Vector3 position)
    {
        foreach (ITurret turret in _turrets)
            turret.AimTowards(position);
    }

    public bool CanHit(Vector3 target)
    {
        if (UseCanHit == CanHitAggregation.Any)
            return _turrets.Any(x => x.CanHit(target));
        if (UseCanHit == CanHitAggregation.All)
            return _turrets.All(x => x.CanHit(target));
        if (UseCanHit == CanHitAggregation.Always)
            return true;
        return false;
    }

    public float DeltaAngle(Vector3 target)
    {
        if (UseDeltaAngle == DeltaAngleAggregation.Min)
            return _turrets.Min(x => x.DeltaAngle(target));
        if (UseDeltaAngle== DeltaAngleAggregation.Max)
            return _turrets.Max(x => x.DeltaAngle(target));
        if (UseDeltaAngle == DeltaAngleAggregation.Zero)
            return 0f;
        return 180f;
    }

    public void AddTurret(ITurret turret) => _turrets.Add(turret);

    private void Awake()
    {
        _turrets = Turrets.Select(x => x.GetComponent<ITurret>()).ToList();
    }
}
