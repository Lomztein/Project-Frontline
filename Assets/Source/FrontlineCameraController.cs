using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrontlineCameraController : MonoBehaviour, ICompositeCameraController, IZoomableCameraController
{
    public Vector3 CameraOffsetMax;
    public Vector3 CameraOffsetMin;

    public float AutoSwitchTime = 60f;
    public float DistanceBetweenPathSamples = 1f;

    public Vector3 LookTargetPosition;
    public Quaternion LookTargetRotation;
    public Vector2 MovementThreshold;
    public Vector2 RotationThreshold;

    public Vector2 MovementSpeedMinMax;
    public AnimationCurve MovementSpeedCurve;
    public float MovementLerpTime;
    public Vector2 RotationSpeedMinMax;
    public AnimationCurve RotationSpeedCurve;
    public float RotationLerpTime;

    public float TargetZoom;
    private float _zoom;
    public float ZoomLerpSpeed;

    private Commander _commander;
    private Frontline _frontline;
    private LineSegment _frontlineLineSegment;

    int _currentIndex;

    void Start()
    {
        Change(0);
        ResetZoom();
    }

    // Update is called once per frame
    void Update()
    {
        if (_frontline != null)
        {
            Vector3 frontlinePosition = _frontline.Position;
            Quaternion rotationTowardsFrontline = Quaternion.LookRotation((frontlinePosition - transform.position).normalized, Vector3.up);

            Vector3 frontlineDirection = (SamplePrevPoint(frontlinePosition) - SampleNextPoint(frontlinePosition)).normalized;
            Quaternion frontlineRotation = Quaternion.LookRotation(frontlineDirection, Vector3.up);

            float frontlineDistFromTarget = Vector3.Distance(frontlinePosition, LookTargetPosition);
            float rotationDiff = Quaternion.Angle(rotationTowardsFrontline, LookTargetRotation);

            float movementFactor = Mathf.Clamp01(Mathf.InverseLerp(MovementThreshold.x, MovementThreshold.y, frontlineDistFromTarget));
            float rotationFactor = Mathf.Clamp01(Mathf.InverseLerp(RotationThreshold.x, RotationThreshold.y, rotationDiff));

            LookTargetPosition = Vector3.MoveTowards(LookTargetPosition, frontlinePosition, Mathf.Lerp(MovementSpeedMinMax.x, MovementSpeedMinMax.y, MovementSpeedCurve.Evaluate(movementFactor)) * Time.deltaTime);
            LookTargetRotation = Quaternion.RotateTowards(LookTargetRotation, rotationTowardsFrontline, Mathf.Lerp(RotationSpeedMinMax.x, RotationSpeedMinMax.y, RotationSpeedCurve.Evaluate(rotationFactor)) * Time.deltaTime);

            Vector3 cameraTargetPosition = LookTargetPosition + frontlineRotation * Vector3.Lerp(CameraOffsetMin, CameraOffsetMax, _zoom);
            Quaternion cameraTargetRotation = LookTargetRotation;

            transform.SetPositionAndRotation(
                Vector3.Lerp(transform.position, cameraTargetPosition, MovementLerpTime * Time.deltaTime),
                Quaternion.Slerp (transform.rotation, cameraTargetRotation, RotationLerpTime * Time.deltaTime)
            );

            _zoom = Mathf.Lerp(_zoom, TargetZoom, ZoomLerpSpeed * Time.fixedDeltaTime);
        }
    }
    
    private void SetRandomCommander ()
    {
        var commanders = GameObject.FindGameObjectsWithTag("Commander").Select(x => x.GetComponent<Commander>()).ToArray();
        if (commanders.Length > 0)
        {
            SetCommander(commanders[Random.Range(0, commanders.Length)]);
        }
    }

    private void SetCommander(Commander commander)
    {
        _commander = commander;
        _frontline = _commander.Frontline;
        var nodes = Navigation.GetPath(Navigation.GetNearestNode(_commander.transform.position), Navigation.GetNearestNode(_commander.Target.transform.position));
        _frontlineLineSegment = LineSegment.CreateFrom(nodes.Select(x => x.Position));
        Debug.Log("FrontlineCam SetCommander: " + commander.Name);
    }

    private Vector3 SamplePrevPoint(Vector3 point)
        => _frontlineLineSegment.GetPosition(_frontlineLineSegment.GetContinuousIndexOfPosition(point) - DistanceBetweenPathSamples / 2f);

    private Vector3 SampleNextPoint(Vector3 point)
    => _frontlineLineSegment.GetPosition(_frontlineLineSegment.GetContinuousIndexOfPosition(point) + DistanceBetweenPathSamples / 2f);

    private void OnDrawGizmosSelected()
    {
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
        Gizmos.color = Color.white;

        if (_commander)
        {
            Gizmos.DrawWireSphere(_commander.Frontline.GetPosition(), MovementThreshold.x);
            Gizmos.DrawWireSphere(_commander.Frontline.GetPosition(), MovementThreshold.y);
            Gizmos.DrawSphere(LookTargetPosition, 0.25f);
        }
    }

    public bool Change(int value)
    {
        CancelInvoke(nameof(SetRandomCommander));
        var commanders = GameObject.FindGameObjectsWithTag("Commander").Select(x => x.GetComponent<Commander>()).ToArray();

        _currentIndex += value;
        if (_currentIndex > commanders.Length - 1) { 
            _currentIndex = commanders.Length - 1;
            return false;
        }
        if (_currentIndex < -1)
        {
            _currentIndex = 0;
            return false;
        }

        if (_currentIndex == -1)
        {
            SetRandomCommander();
            InvokeRepeating(nameof(SetRandomCommander), AutoSwitchTime, AutoSwitchTime);
        }
        else
        {
            SetCommander(commanders[_currentIndex]);
        }

        return true;
    }

    public string GetName()
    {
        if (_currentIndex != -1)
        {
            return $"{name} ({_commander.Name})";
        }
        return $"{name} (Auto - {_commander.Name})";
    }

    public void Zoom(float amount)
    {
        TargetZoom += amount;
        TargetZoom = Mathf.Clamp01(TargetZoom);
    }

    public void ResetZoom()
    {
        TargetZoom = 1f;
    }
}
