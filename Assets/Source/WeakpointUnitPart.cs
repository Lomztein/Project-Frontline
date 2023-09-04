using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakpointUnitPart : UnitPart
{
    public float RangeThreshold;
    public Transform Transform;
    public Health Health;
    public float DamageMultiplier = 2f;

    private void Awake()
    {
        Health.OnTakeDamage += Health_OnTakeDamage;
        Health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath(Health obj)
    {
        Destroy(gameObject);
    }

    private void Health_OnTakeDamage(Health arg1, DamageInfo arg2)
    {
        float sqrDist = (Transform.position - arg2.Point).sqrMagnitude;
        if (sqrDist < RangeThreshold * RangeThreshold)
        {
            arg2.Damage *= DamageMultiplier;
        }
    }

    private void OnDrawGizmos ()
    {
        Gizmos.DrawWireSphere(Transform.position, RangeThreshold);
    }
}
