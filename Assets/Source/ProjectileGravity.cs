using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGravity : MonoBehaviour
{
    public Projectile Projectile;
    public float Gravity;

    private void FixedUpdate()
    {
        Projectile.Velocity += Gravity * Time.fixedDeltaTime * Vector3.down;
        Projectile.transform.LookAt(Projectile.transform.position + Projectile.Velocity);
    }
}
