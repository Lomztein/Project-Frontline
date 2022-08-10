using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryIKControl : MonoBehaviour
{
    private Animator _animator;

    public bool IKActive = false;

    public Transform RightHandTarget;
    public Transform LeftHandTarget;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (_animator)
            {
                // Set the right hand target position and rotation, if one has been assigned
                if (RightHandTarget != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
                }

                if (LeftHandTarget != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetLookAtWeight(0);
            }
        }
    }
}
