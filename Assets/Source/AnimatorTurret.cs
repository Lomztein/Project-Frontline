using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTurret : MonoBehaviour, ITurret
{
    public Transform Base;
    public Transform Muzzle;
    public Animator Animator;

    public string HorAimName;
    public string VerAimName;

    public Vector2 HorMinMax;
    public Vector2 VerMinMax;

    public float HorSpeed;
    public float VerSpeed;

    private Vector2 _currentAim;
    private Vector2 _targetAim;

    private void FixedUpdate()
    {
        Rotate();
        UpdateAnimator();
    }

    private void Rotate ()
    {
        float x = Mathf.MoveTowards(_currentAim.x, _targetAim.x, HorSpeed * Time.fixedDeltaTime);
        float y = Mathf.MoveTowards(_currentAim.y, _targetAim.y, VerSpeed * Time.fixedDeltaTime);
        _currentAim = new Vector2(x, y);
    }

    private void UpdateAnimator ()
    {
        Animator.SetFloat(HorAimName, _currentAim.y);
        Animator.SetFloat(VerAimName, -_currentAim.x);
    }

    public void AimTowards(Vector3 position)
    {
        Vector3 loc = Base.InverseTransformPoint(position);
        Vector2 angles = Turret.CalculateAngleTowards(loc);

        _targetAim = new Vector2(angles.x, angles.y);
    }

    public float DeltaAngle(Vector3 target)
    {
        Vector3 localPosition = Muzzle.InverseTransformPoint(target);
        float angle = Vector3.Angle(Vector3.forward, localPosition);
        return angle;
    }
    public bool CanHit(Vector3 target)
    {
        Vector3 localPosition = Base.InverseTransformPoint(target);

        float x = Mathf.Atan2(localPosition.y, localPosition.z) * Mathf.Rad2Deg;
        float y = Mathf.Atan2(localPosition.x, localPosition.z) * Mathf.Rad2Deg;

        return HorMinMax.x < x && x < HorMinMax.y
            && VerMinMax.x < y && y < VerMinMax.y;
    }
}
