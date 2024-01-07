using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MindControlChannelWeapon : ChannelWeapon
{
    public float ProgressPerSecond;
    public override float Damage => ProgressPerSecond / Firerate;
    public override float Speed => 1337;

    public DamageModifier ProgressModifier;
    public override DamageModifier Modifier => ProgressModifier;

    public Unit Unit;

    public float ControlProgress;
    public float ControlProgressTarget;
    public bool PermanentControl;

    private float _currentTargetModifierValue;
    private Health _currentTargetHealth;
    public Unit CurrentControllingUnit;

    [Header("Control Line Thingie")]
    public LineRenderer LineRenderer;
    public Transform LineOrigin;
    public float LineHeight;
    public float CurveWidth;
    public float CurveBurstWidth;
    public float CurveBurstTime;
    public AnimationCurve CurveBurstAnim;
    public float CurveLerpTime;

    private float _curveBurstStartTime;
    private float _currentCurveWidth;

    public override void BeginChannel(ITarget target)
    {
        GameObject targetObject = target.GetGameObject();
        if (target.ExistsAndValid() && targetObject != null)
        {
            _currentTargetHealth = targetObject.GetComponentInParent<Health>();
            if (_currentTargetHealth)
            {
                _currentTargetModifierValue = DamageModifier.Combine(_currentTargetHealth.Modifier, ProgressModifier);
                ControlProgress = 0;
            }
        }
    }

    public override void EndChannel(ITarget target)
    {
        if (!PermanentControl)
        {
            GameObject targetObject = target.GetGameObject();
            if (target.ExistsAndValid() && targetObject != null)
            {
                Unit targetUnit = targetObject.GetComponentInParent<Unit>();
                RelinquishControl(targetUnit);
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (ChannelState == State.Channeling && _currentTargetHealth)
        {
            ControlProgress += ProgressPerSecond * _currentTargetModifierValue * Time.fixedDeltaTime;
            if (ControlProgress > _currentTargetHealth.CurrentHealth)
            {
                if (!CurrentControllingUnit)
                {
                    CurrentControllingUnit = _currentTargetHealth.GetComponentInParent<Unit>();
                    MindControl(CurrentControllingUnit);
                    BurstCurve();
                }
                else
                {
                    if (CurrentControllingUnit.TeamInfo != Unit.TeamInfo)
                    {
                        MindControl(CurrentControllingUnit);
                    }
                }
            }

            if (CurrentControllingUnit)
            {
                LineRenderer.enabled = true;
                UpdateLineRenderer();
            }
            else
            {
                LineRenderer.enabled = false;
            }
        }
        else
        {
            LineRenderer.enabled = false;
        }
    }

    private void BurstCurve ()
    {
        _curveBurstStartTime = Time.time;
    }

    private void MindControl(Unit target)
    {
        Unit.TeamInfo.ApplyTeam(target.gameObject);
        if (Unit.Commander)
            Unit.Commander.AssignCommander(target.gameObject);
        CurrentControllingUnit = target;
        if (target.TryGetComponent(out AttackerController cont) && Unit.Commander)
        {
            cont.SetPath(Navigation.GetPath(Navigation.GetNearestNode(cont.transform.position), Navigation.GetNearestNode(Unit.Commander.Target.Fortress.position)).ToArray());
        }
    }

    private void RelinquishControl(Unit target)
    {
        target.InitialTeamInfo.ApplyTeam(target.gameObject);
        if (target.InitialCommander)
            target.InitialCommander.AssignCommander(target.gameObject);
        CurrentControllingUnit = null;
        if (target.TryGetComponent(out AttackerController cont) && target.InitialCommander)
        {
            cont.SetPath(Navigation.GetPath(Navigation.GetNearestNode(cont.transform.position), Navigation.GetNearestNode(target.InitialCommander.Target.Fortress.position)).ToArray());
        }
    }

    private void UpdateLineRenderer()
    {
        Vector3 end = CurrentChannelTarget.GetCenter();
        if (CurrentControllingUnit.Weakpoints.Any())
        {
            end = CurrentControllingUnit.Weakpoints.First().Transform.position;
        }

        float length = Vector3.Distance(LineOrigin.position, end);
        Quaternion rotationToTarget = Quaternion.LookRotation(end - LineOrigin.position);

        float burstProgress = Mathf.Clamp01((Time.time - _curveBurstStartTime) / CurveBurstTime);
        float targetWidth = CurveBurstWidth + CurveBurstWidth * CurveBurstAnim.Evaluate(burstProgress);
        _currentCurveWidth = Mathf.Lerp(_currentCurveWidth, targetWidth, CurveLerpTime * Time.fixedDeltaTime);

        LineRenderer.startWidth = _currentCurveWidth;
        LineRenderer.endWidth = _currentCurveWidth;

        for (int i = 0; i < LineRenderer.positionCount; i++)
        {
            float lengthProgress = i / (float)(LineRenderer.positionCount - 1);
            float y = ComputeLineHeight(lengthProgress * length, length, LineHeight);
            LineRenderer.SetPosition(i, rotationToTarget * new Vector3(0f, y, lengthProgress * length) + LineOrigin.position);
        }
    }

    private float ComputeLineHeight(float x, float length, float height)
    {
        float num = Mathf.Pow(2 * x - length, 2f) * height;
        float den = Mathf.Pow(length, 2);
        return -(num / den) + height;
    }
}
