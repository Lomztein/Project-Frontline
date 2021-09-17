using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryWeaponTurret : MonoBehaviour, ITurret
{
    public AnimatorTurret AnimatorTurret;
    public Transform Transform;

    public Transform Pivot;
    public Transform Muzzle;

    private void Start()
    {
        Transform.SetParent(Pivot);
        Transform.localPosition = Vector3.zero;
        Transform.rotation = transform.root.rotation;
    }

    public void AimTowards(Vector3 position)
    {
        ((ITurret)AnimatorTurret).AimTowards(position);
        if (CanHit(position))
        {
            Transform.LookAt(position);
        }
    }

    public bool CanHit(Vector3 target)
    {
        return ((ITurret)AnimatorTurret).CanHit(target);
    }

    public float DeltaAngle(Vector3 target)
    {
        return ((ITurret)AnimatorTurret).DeltaAngle(target);
    }
}
