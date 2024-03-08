using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Projectile that deals area damage in a cone ahead of it.
public class BlastProjectile : Projectile
{
    public float ConeLength;
    public float ConeRadius;

    public AnimationCurve DamageByDistanceMultiplier = AnimationCurve.Linear(1f, 0f, 1f, 0f);
    public AnimationCurve DamageDelayCurve;
    public float MaxDamageDelay;

    // Start is called before the first frame update
    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);

        Collider[] colliders = Physics.OverlapSphere(transform.position, ConeLength, HitLayerMask | TerrainLayerMask);
        foreach (var col in colliders)
        {
            Debug.Log(col);
            if (IsWithinCone(col.transform))
            {
                float distNormalized = Vector3.Distance(col.transform.position, transform.position) / ConeLength;
                float time = DamageDelayCurve.Evaluate(distNormalized * MaxDamageDelay);
                float damage = DamageByDistanceMultiplier.Evaluate(distNormalized) * Damage;
                StartCoroutine(DoDamage(col, damage, time));
            }
        }
    }

    protected override void FixedUpdate()
    {
    }

    private bool IsWithinCone (Transform obj)
    {
        Vector3 rel = transform.InverseTransformPoint(obj.position);
        if (rel.z > ConeLength) return false;
        float angle = Vector3.Angle(Vector3.forward, rel);
        return angle < Mathf.Atan(ConeRadius / ConeLength) * Mathf.Rad2Deg;
    }

    IEnumerator DoDamage(Collider target, float damage, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (target)
        {
            Vector3 point = target.ClosestPoint(transform.position);
            DoDamage(target, damage, point);
            InvokeOnHit(target, point, (target.transform.position - transform.position).normalized);
        }
    }
}
