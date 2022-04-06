using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryAnimator : MonoBehaviour
{
    public enum WeaponType { None, Pistol, Rifle, Bazooka }

    public Animator Animator;
    public InfantryBody Body;
    public float SpeedMultiplier = 1f;
    public WeaponType WeaponPoseType;

    public void Awake()
    {
        Animator.SetFloat("RunMult", SpeedMultiplier);
        Animator.SetInteger("WeaponType", (int)WeaponPoseType);
    }

    private void FixedUpdate()
    {
        Animator.SetBool("Moving", Body.IsMoving);
    }
}
