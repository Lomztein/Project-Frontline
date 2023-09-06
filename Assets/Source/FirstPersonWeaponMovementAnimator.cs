using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonWeaponMovementAnimator : MonoBehaviour
{
    public StarterAssetsInputs Inputs;
    public Transform Base;

    public float SwayForce;
    public Vector2 SwayVelocity;
    public float SwayDampen;

    public AnimationCurve HorizontalBobbing;
    public AnimationCurve VerticalBobbing;
    public float VerticalOffset;
    public float MoveFactor;
    public float BobbingForce;
    public float BobbingSpeed;
    public float BobbingTime;
    public float BobLerp;
    public Vector3 Bob;

    private void Update()
    {
        Vector2 sway = Inputs.look * SwayForce;
        SwayVelocity += sway * Time.deltaTime;
        SwayVelocity = Vector2.Lerp(SwayVelocity, Vector2.zero, SwayDampen * Time.deltaTime);
        Base.transform.localPosition = new Vector3(-SwayVelocity.x, SwayVelocity.y, 0f);
        MoveFactor = Mathf.Lerp(MoveFactor, Inputs.move.magnitude, BobLerp * Time.deltaTime);
        BobbingTime += MoveFactor * Time.deltaTime * BobbingSpeed;

        Vector3 targetBob = new Vector3(
            HorizontalBobbing.Evaluate(Mathf.PingPong(BobbingTime * 2f, 1f) % 1f),
            VerticalBobbing.Evaluate(Mathf.PingPong(BobbingTime + VerticalOffset, 1f) % 1f),
            0f) * BobbingForce * MoveFactor;

        Bob = Vector3.Lerp(Bob, targetBob, BobLerp * Time.deltaTime);

        Base.transform.localPosition += Bob;
    }
}
