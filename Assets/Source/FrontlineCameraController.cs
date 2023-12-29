using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrontlineCameraController : MonoBehaviour
{
    public Vector3 Offset;

    public float TargetMoveSpeed;
    public float TargetMoveSpeedDistanceMultiplier = 0.1f;
    public float LerpTime;
    public float TargetRotationSpeed;
    public float RotationLerpTime;
    public Vector2 SwitchTimeMinMax;
    public float DistanceBetweenPathSamples = 1f;

    private Vector3 _targetPos;
    private Quaternion _targetRot;

    private Commander _commander;
    private Frontline _frontline;
    private LineSegment _frontlineLineSegment;

    void Start()
    {
        Switch();
    }

    // Update is called once per frame
    void Update()
    {
        if (_frontline != null)
        {
            Vector3 frontlineDirection = (SamplePrevPoint(_frontline.Position) - SampleNextPoint(_frontline.Position)).normalized;
            Quaternion frontlineRotation = Quaternion.LookRotation(frontlineDirection, Vector3.up);

            Vector3 targetPos = (_frontline.Position + frontlineRotation * Offset + _frontline.Change) * (1f + _frontline.Change.magnitude);
            float dist = Vector3.Distance(targetPos, transform.position);
            _targetPos = Vector3.MoveTowards(_targetPos, targetPos, TargetMoveSpeed * Time.deltaTime + (dist * TargetMoveSpeedDistanceMultiplier) * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, _targetPos, LerpTime * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation((_frontline.Position - transform.position).normalized, Vector3.up);
            _targetRot = Quaternion.RotateTowards(_targetRot, targetRot, TargetRotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp (transform.rotation, _targetRot, RotationLerpTime * Time.deltaTime);
        }
    }

    private void Switch ()
    {
        var commanders = GameObject.FindGameObjectsWithTag("Commander").Select(x => x.GetComponent<Commander>()).ToArray();
        if (commanders.Length > 0)
        {
            _commander = commanders[Random.Range(0, commanders.Length)];
            _frontline = _commander.Frontline;
            var nodes = Navigation.GetPath(Navigation.GetNearestNode(_commander.transform.position), Navigation.GetNearestNode(_commander.Target.transform.position));
            _frontlineLineSegment = LineSegment.CreateFrom(nodes.Select(x => x.Position));
        }
        CancelInvoke(nameof(Switch));
        Invoke(nameof(Switch), Random.Range(SwitchTimeMinMax.x, SwitchTimeMinMax.y));
    }

    private Vector3 SamplePrevPoint(Vector3 point)
        => _frontlineLineSegment.GetPosition(_frontlineLineSegment.GetContinuousIndexOfPosition(point) - DistanceBetweenPathSamples / 2f);

    private Vector3 SampleNextPoint(Vector3 point)
    => _frontlineLineSegment.GetPosition(_frontlineLineSegment.GetContinuousIndexOfPosition(point) + DistanceBetweenPathSamples / 2f);

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(Vector3.zero, Offset);
        Gizmos.DrawSphere(Offset, 0.5f);
        Gizmos.color = Color.magenta;
        if (_frontlineLineSegment != null)
        {
            Vector3 currentPointOnLine = _frontlineLineSegment.GetNearestPointOnLines(_frontline.Position);
            Gizmos.DrawSphere(currentPointOnLine, 0.4f);
            Gizmos.DrawSphere(SamplePrevPoint(currentPointOnLine), 0.4f);
            Gizmos.DrawSphere(SampleNextPoint(currentPointOnLine), 0.4f);

            foreach (var line in _frontlineLineSegment.Lines)
            {
                Gizmos.DrawLine(line.From, line.To);
            }
        }
        Gizmos.color = Color.magenta;
    }
}
