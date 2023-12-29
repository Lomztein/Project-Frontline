using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public Transform Muzzle;
    public Weapon Weapon;
    public GameObject WeaponObject;
    private IWeapon _weapon;
    public VehicleBodyRecoilAnimator Animator;
    public float RecoilStrength;

    private void Start()
    {
        if (WeaponObject)
        {
            _weapon = WeaponObject.GetComponent<IWeapon>();
        }
        if (Weapon)
        {
            _weapon = Weapon;
        }
        _weapon.OnFire += OnFire;
    }

    public void SetWeapon(IWeapon weapon)
    {
        if (_weapon != null)
        {
            _weapon.OnFire -= OnFire;
        }
        _weapon = weapon;
        _weapon.OnFire += OnFire;
    }

    private void OnFire(IWeapon weapon)
    {
        Animator.Recoil(Muzzle.forward * -1 * RecoilStrength);
    }
}
