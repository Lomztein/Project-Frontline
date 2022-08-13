using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLerpHoming : MonoBehaviour
{
    public Projectile Projectile;
    public float LerpSpeed;

    private void FixedUpdate()
    {
        if (Projectile.Target.ExistsAndValid())
        {
            Quaternion targetRotation = Quaternion.LookRotation(Projectile.Target.GetPosition() - transform.position);
            var rot = Quaternion.Lerp(transform.rotation, targetRotation, LerpSpeed * Time.fixedDeltaTime);
            Projectile.transform.rotation = rot;
            Projectile.Velocity = Projectile.transform.forward * Projectile.Speed;
        }
    }
}
