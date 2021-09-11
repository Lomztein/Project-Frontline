using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeTurret : MonoBehaviour, ITurret
{
    public GameObject[] Turrets;
    private ITurret[] _turrets;

    public void AimTowards(Vector3 position)
    {
        foreach (ITurret turret in _turrets)
            turret.AimTowards(position);
    }

    public bool CanHit(Vector3 target)
    {
        return _turrets.Any(x => x.CanHit(target));
    }

    public float DeltaAngle(Vector3 target)
    {
        return _turrets.Min(x => x.DeltaAngle(target));
    }

    private void Awake()
    {
        _turrets = Turrets.Select(x => x.GetComponent<ITurret>()).ToArray();
    }
}
