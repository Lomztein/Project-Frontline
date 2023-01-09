using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRangeOverride : MonoBehaviour, ITurret
{
    public GameObject TurretObject;
    public Transform Base;

    private ITurret _turret;

    public Vector2 HorizontalRange;
    public Vector2 VerticalRange;

    private void Awake()
    {
        _turret = TurretObject.GetComponent<ITurret>();
    }

    public void AimTowards(Vector3 position)
    {
        _turret.AimTowards(position);
    }

    public bool CanHit(Vector3 target)
    {
        Vector3 localPosition = Base.InverseTransformPoint(target);
        Vector3 angles = Turret.CalculateAngleTowards(localPosition);

        return HorizontalRange.x < angles.y && angles.y < HorizontalRange.y
            && VerticalRange.x < angles.x && angles.x < VerticalRange.y;
    }

    public float DeltaAngle(Vector3 target)
    {
        return _turret.DeltaAngle(target);
    }
}
