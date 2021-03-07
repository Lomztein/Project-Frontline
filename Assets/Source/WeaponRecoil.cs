using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public Transform Muzzle;
    public Weapon Weapon;
    public VehicleBodyRecoilAnimator Animator;
    public float RecoilStrength;

    private void Start()
    {
        Weapon.OnFire += OnFire;
    }

    private void OnFire()
    {
        Animator.Recoil(Muzzle.forward * -1 * RecoilStrength);
    }
}
