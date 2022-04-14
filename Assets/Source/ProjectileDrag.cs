using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDrag : MonoBehaviour
{
    public Projectile Projectile;
    public float DragFactor;

    private void FixedUpdate()
    {
        Projectile.Velocity *= DragFactor;
    }
}
