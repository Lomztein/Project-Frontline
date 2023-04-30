using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBurnOnHit : MonoBehaviour
{
    public Projectile Projectile;
    public GameObject BurnPrefab;

    private void Start()
    {
        Projectile.OnHit += FireProjectile_OnHit;
    }


    private void FireProjectile_OnHit(Projectile arg1, Collider arg2, Vector3 arg3, Vector3 arg4)
    {
        ApplyBurn(arg2);
    }

    private void ApplyBurn(Collider target)
    {
        BurnController cont = target.transform.root.GetComponentInChildren<BurnController>();
        if (cont)
        {
            cont.Destroyer.ResetDestroyTimer();
        }
        else
        {
            if (BurnController.CanBurn(target))
            {
                GameObject newBurn = Instantiate(BurnPrefab, target.transform);
                newBurn.transform.localPosition = Vector3.zero;
                newBurn.GetComponent<BurnController>().SetTarget(target);
            }
        }
    }
}
