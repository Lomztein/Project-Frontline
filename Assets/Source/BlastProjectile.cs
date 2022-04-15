using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Projectile that deals area damage in a cone ahead of it.
public class BlastProjectile : Projectile
{
    public float ConeLength;
    public float ConeRadius;

    public AnimationCurve DamageDelayCurve;
    public float MaxDamageDelay;

    // Start is called before the first frame update
    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);

        Collider[] colliders = Physics.OverlapSphere(transform.position, ConeLength, HitLayerMask);
        foreach (var col in colliders)
        {
            if (IsWithinCone(col.transform))
            {
                float time = DamageDelayCurve.Evaluate(Vector3.Distance(col.transform.position, transform.position) / ConeLength) * MaxDamageDelay;
                StartCoroutine(DoDamage(col, time));
            }
        }
    }

    private bool IsWithinCone (Transform obj)
    {
        Vector3 rel = transform.InverseTransformPoint(obj.position);
        if (rel.z > ConeLength) return false;
        float angle = Mathf.Abs(Mathf.Atan2(Mathf.Max(rel.x, rel.y), rel.z) * Mathf.Rad2Deg);
        return angle < Mathf.Atan(ConeRadius / ConeLength) * Mathf.Rad2Deg;
    }

    IEnumerator DoDamage(Collider target, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (target)
        {
            DoDamage(target, target.transform.position);
        }
    }
}
