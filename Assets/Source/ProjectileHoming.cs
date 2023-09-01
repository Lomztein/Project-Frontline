using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHoming : MonoBehaviour
{
    public Projectile Projectile;
    public float RotationSpeed;

    private void FixedUpdate()
    {
        if (Projectile.Target.ExistsAndValid())
        {
            Quaternion targetRotation = Quaternion.LookRotation(Projectile.Target.GetCenter() - transform.position);
            Projectile.transform.rotation = Quaternion.RotateTowards(Projectile.transform.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime);
            Projectile.Velocity = Projectile.transform.forward * Projectile.Speed;
        }
    }
}
