using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBody : MonoBehaviour, IControllable
{
    public Animator AnimationController;
    public float JumpChargeTime;
    public float JumpCooldownTime;
    public float JumpInvokeDelay;
    public Vector3 JumpVelocity;
    public float TurnSpeed;

    private float _jumpCooldown;
    private float _jumpCharge;
    private float _currentAngle;

    [Header("Physics")]
    private Vector3 _velocity;
    public LayerMask Terrain;
    public float RaycastHeight;
    public float ContactHeight;
    private bool _isGrounded;

    public float GroundedDampening = 0.85f;
    public Vector3 Gravity = new Vector3(0f, -9.81f, 0f);

    private float _curAcceleration;
    private float _curTurn;

    public Vector3 Accelerate(float factor)
    {
        _curAcceleration = factor;
        return Vector3.zero;
    }

    public Vector3 Turn(float factor)
    {
        _curTurn = factor;
        return Vector3.zero;
    }

    private void Jump ()
    {
        _velocity += transform.rotation * JumpVelocity;
        _jumpCooldown = JumpCooldownTime;
    }

    private void FixedUpdate()
    {
        if (_jumpCooldown > 0f)
        {
            _jumpCooldown -= Time.fixedDeltaTime;
        }
        else
        {
            _jumpCooldown = 0;
        }

        if (_jumpCharge > 0f)
        {
            _jumpCharge -= Time.fixedDeltaTime;
        }
        else
        {
            _jumpCharge = 0f;
        }

        DoMove();
        DoTurn();
        DoPhysics();
    }

    private void DoPhysics()
    {
        if (Physics.Raycast(transform.position + transform.up * RaycastHeight, transform.up * -1, out RaycastHit hit, RaycastHeight - ContactHeight, Terrain) && _jumpCooldown < JumpCooldownTime - 0.2f)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y * ContactHeight, transform.position.z);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        if (!_isGrounded)
        {
            _velocity += Gravity * Time.fixedDeltaTime;
        }
        else
        {
            _velocity = new Vector3(_velocity.x * GroundedDampening, 0f, _velocity.z * GroundedDampening);
        }

        transform.position += _velocity * Time.fixedDeltaTime;
    }

    private void DoTurn()
    {
        _currentAngle += _curTurn * TurnSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0f, _currentAngle, 0f);
    }

    private void DoMove()
    {
        if (Mathf.Abs(_curAcceleration) > 0.1f && _isGrounded)
        {
            if (_jumpCooldown < 0.1f)
            {
                _jumpCharge += Time.fixedDeltaTime * 2f;
                if (_jumpCharge > JumpChargeTime)
                {
                    AnimationController.SetTrigger("Jump");
                    _jumpCharge = 0f;

                    if (!IsInvoking())
                    {
                        Invoke("Jump", JumpInvokeDelay);
                    }
                }
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position + transform.up * RaycastHeight, transform.up * -1 * (RaycastHeight - ContactHeight));
    }
}
