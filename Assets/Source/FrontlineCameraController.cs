using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrontlineCameraController : MonoBehaviour
{
    public Vector3 Offset;

    public float TargetMoveSpeed;
    public float LerpTime;
    public float RotationLerpTime;
    public float SwitchTime;

    private Vector3 _targetPos;
    private Frontline _frontline;

    void Start()
    {
        Switch();
        InvokeRepeating(nameof(Switch), SwitchTime, SwitchTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (_frontline != null)
        {
            Vector3 targetPos = (_frontline.Position + Offset) * (1f + _frontline.Change.magnitude);
            _targetPos = Vector3.MoveTowards(_targetPos, targetPos, TargetMoveSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, _targetPos, LerpTime * Time.deltaTime);
            Quaternion targetRot = Quaternion.LookRotation((_frontline.Position - targetPos).normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, RotationLerpTime * Time.fixedDeltaTime);
        }
    }

    private void Switch ()
    {
        var commanders = GameObject.FindGameObjectsWithTag("Commander").Select(x => x.GetComponent<Commander>()).ToArray();
        if (commanders.Length > 0)
        {
            _frontline = commanders[Random.Range(0, commanders.Length)].Frontline;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(Vector3.zero, Offset);
        Gizmos.DrawSphere(Offset, 0.5f);
    }
}
