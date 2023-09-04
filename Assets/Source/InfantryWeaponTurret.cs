using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryWeaponTurret : MonoBehaviour, ITurret
{
    public Transform Transform;
    public Transform Pivot;

    private void Start()
    {
        Transform.SetParent(Pivot);
        Transform.localPosition = Vector3.zero;
        Transform.rotation = transform.root.rotation;
    }

    public void AimTowards(Vector3 position)
    {
        Transform.LookAt(position);
    }

    public bool CanHit(Vector3 target)
    {
        return true;
    }

    public float DeltaAngle(Vector3 target)
    {
        return 0f;
    }
}
