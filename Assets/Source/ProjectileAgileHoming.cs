using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAgileHoming : MonoBehaviour
{
    public Projectile Projectile;

    private float _speedMax;
    public float SpeedMin;
    public AnimationCurve SpeedByDeviance;
    
    public Vector2 TurnrateMinMax;
    public AnimationCurve TurnrateByDeviance;

    private void Start()
    {
        _speedMax = Projectile.Speed;
    }

    private void FixedUpdate()
    {
        if (Projectile.Target.ExistsAndValid())
        {
            Vector3 forward = Projectile.transform.forward;
            Vector3 target = (Projectile.Target.GetCenter() - Projectile.transform.position).normalized;
            float factor = 1f - (Vector3.Dot(forward, target) + 1f) / 2f; // 1 = needs most correction (180 deg), 0 = needs no correction (0 deg).

            float speed = Mathf.Lerp(SpeedMin, _speedMax, SpeedByDeviance.Evaluate(factor));
            float turnrate = Mathf.Lerp(TurnrateMinMax.x, TurnrateMinMax.y, TurnrateByDeviance.Evaluate(factor));

            Quaternion targetRot = Quaternion.LookRotation(target);
            Projectile.transform.rotation = Quaternion.RotateTowards(Projectile.transform.rotation, targetRot, turnrate * Time.fixedDeltaTime);
            Projectile.Velocity = Projectile.transform.forward * speed;
        }
    }
}
