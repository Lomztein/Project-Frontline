using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBodyRecoilAnimator : MonoBehaviour
{
    public Transform BodyPivot;

    private Vector3 _velocity;
    private Vector3 _recoil; // X-component = pitch, z-component = roll;
    public Vector3 RecoilAnimMult = Vector3.one;
    public float Dampening = 1;
    public float SpringStrength = 1; // Something something hookes law

    public void Recoil (Vector3 velocity)
    {
        _velocity += BodyPivot.InverseTransformVector (velocity);
    }

    private Vector3 ComputeHookesLaw (Vector3 velocity, float springStrength) // This doesn't really make sense methinks but shut up im a programmer not a physics man
    {
        return -springStrength * velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _velocity += ComputeHookesLaw(_recoil, SpringStrength);
        _velocity *= Dampening;

        _recoil += _velocity * Time.fixedDeltaTime;

        Vector3 euler = MultiplyComponents(_recoil, RecoilAnimMult);
        BodyPivot.localRotation = Quaternion.Euler(euler.z, 0f, -euler.x);
    }

    private Vector3 MultiplyComponents (Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(
            lhs.x * rhs.x,
            lhs.y * rhs.y,
            lhs.z * rhs.z
        );
    }
}
