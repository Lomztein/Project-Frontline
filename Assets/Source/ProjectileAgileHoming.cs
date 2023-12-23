using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileAgileHoming : MonoBehaviour
{
    public Projectile Projectile;

    private float _speedMax;
    public float SpeedMin;
    public AnimationCurve SpeedByDeviance;
    
    public Vector2 TurnrateMinMax;
    public AnimationCurve TurnrateByDeviance;
    
    public bool PredictTargetPosition;
    private PositionPredicter _predictor = new PositionPredicter();

    private void Start()
    {
        _speedMax = Projectile.Speed;
        if (Projectile.Target.ExistsAndValid())
        {
            _predictor.Tick(Projectile.Target.GetCenter(), Time.fixedDeltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (Projectile.Target.ExistsAndValid())
        {
            Vector3 targetPosition = Projectile.Target.GetCenter();
            float dist = Vector3.Distance(transform.position, targetPosition);

            _predictor.Tick(targetPosition, Time.fixedDeltaTime);
            targetPosition = PredictTargetPosition ? _predictor.GetPredictedPosition(targetPosition, dist, _speedMax) : targetPosition;

            Vector3 forward = Projectile.transform.forward;
            Vector3 target = (targetPosition - Projectile.transform.position).normalized;
            float factor = 1f - (Vector3.Dot(forward, target) + 1f) / 2f; // 1 = needs most correction (180 deg), 0 = needs no correction (0 deg).

            float speed = Mathf.Lerp(SpeedMin, _speedMax, SpeedByDeviance.Evaluate(factor));
            float turnrate = Mathf.Lerp(TurnrateMinMax.x, TurnrateMinMax.y, TurnrateByDeviance.Evaluate(factor));

            Debug.DrawRay(transform.position, target);
            Quaternion targetRot = Quaternion.LookRotation(target);

            Projectile.transform.rotation = Quaternion.RotateTowards(Projectile.transform.rotation, targetRot, turnrate * Time.fixedDeltaTime);
            Projectile.Velocity = Projectile.transform.forward * speed;
        }
    }
}
