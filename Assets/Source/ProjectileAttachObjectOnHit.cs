using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttachObjectOnHit : MonoBehaviour
{
    public Projectile Projectile;
    public GameObject ObjectPrefab;

    private void Start()
    {
        Projectile.OnHit += Projectile_OnHit;
    }

    private void Projectile_OnHit(Projectile arg1, Collider arg2, Vector3 arg3, Vector3 arg4)
        => Instantiate(ObjectPrefab, arg3, arg1.transform.rotation, arg2.transform);
}
