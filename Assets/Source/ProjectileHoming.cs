using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHoming : MonoBehaviour
{
    public Projectile Projectile;
    public float RotationSpeed;

    public bool PredictTargetPosition;
    private PositionPredicter _predicter = new PositionPredicter();

    private void FixedUpdate()
    {
        if (Projectile.Target.ExistsAndValid())
        {
            Vector3 pos = Projectile.Target.GetCenter();

            if (PredictTargetPosition)
            {
                float distToTarget = Vector3.Distance(transform.position, pos);
                _predicter.Tick(pos, Time.fixedDeltaTime);
                pos = _predicter.GetPredictedPosition(pos, distToTarget, Projectile.Speed);
                Debug.DrawLine(transform.position, pos);
            }

            Quaternion targetRotation = Quaternion.LookRotation(pos - transform.position);
            Projectile.transform.rotation = Quaternion.RotateTowards(Projectile.transform.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime);
            Projectile.Velocity = Projectile.transform.forward * Projectile.Speed;
        }
    }
}
