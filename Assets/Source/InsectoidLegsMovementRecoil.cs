using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectoidLegsMovementRecoil : MonoBehaviour
{
    public InsectoidLegsProceduralAnimation LegsAnimator;
    public VehicleBodyRecoilAnimator RecoilAnimator;
    public float ForceMultiplier;

    private void Start()
    {
        LegsAnimator.OnLegMovement += LegsAnimator_OnLegMovement;
    }

    private void LegsAnimator_OnLegMovement(InsectoidLegsProceduralAnimation.Leg leg, Vector3 from, Vector3 to, float dt)
    {
        Vector3 velocity = (from - to) / dt;
        velocity = leg.LegTarget.parent.localRotation * velocity;
        RecoilAnimator.Recoil(velocity * ForceMultiplier);
    }
}
