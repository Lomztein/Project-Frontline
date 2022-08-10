using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class ProjectileDrag : MonoBehaviour
{
    public Projectile Projectile;
    public float DragCoeffecient;

    private void FixedUpdate()
    {
        Projectile.Velocity += UnityUtils.ComputeSimpleDragForce(Projectile.Velocity, DragCoeffecient) * Time.fixedDeltaTime;
    }
}
