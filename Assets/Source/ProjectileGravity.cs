using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGravity : MonoBehaviour
{
    public Projectile Projectile;
    public float Gravity;

    private void FixedUpdate()
    {
        Projectile.Velocity += Vector3.down * Gravity * Time.fixedDeltaTime;
        Projectile.transform.LookAt(Projectile.transform.position + Projectile.Velocity);
    }
}
