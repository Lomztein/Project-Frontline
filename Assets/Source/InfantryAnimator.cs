using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfantryAnimator : MonoBehaviour
{
    public Animator Animator;
    public InfantryBody Body;

    public GameObject WeaponObject;
    public Transform Muzzle;
    public float WeaponRecoil;
    public Health Health;
    public float RecoilDampening;

    private Vector2 _recoil;

    private void Start()
    {
        if (!WeaponObject)
        {
            var wep = GetComponentInParent<Unit>().GetWeapons().FirstOrDefault();
            if (wep != null) WeaponObject = (wep as Component).gameObject;
        }

        if (WeaponObject)
        {
            WeaponObject.GetComponent<IWeapon>().OnFire += OnFire;
        }
        if (Health)
        {
            Health.OnTakeDamage += OnTakeDamage;
        }
    }
    private void OnTakeDamage(Health arg1, DamageInfo arg2)
    {
        Vector3 force = arg2.Direction * arg2.Damage;
        Impact(force);
    }

    // Update is called once per frame
    private void OnFire(IWeapon obj)
    {
        Transform muzzle = Muzzle;
        if (muzzle == null && obj is Component comp)
            muzzle = comp.transform;
        Vector3 force = muzzle.forward * WeaponRecoil;
        Impact(force);
    }

    private void Impact(Vector3 force)
    {
        if (this != null)
        {
            force = transform.InverseTransformDirection(force);
            Vector2 f = new Vector2(force.x, force.z);
            _recoil += f;
        }
    }

    private void FixedUpdate()
    {
        _recoil = Vector2.Lerp(_recoil, Vector2.zero, RecoilDampening * Time.fixedDeltaTime);
        Animator.SetFloat("RecoilZ", _recoil.y);
        Animator.SetFloat("RecoilX", _recoil.x);
        Animator.SetFloat("Speed", Body.CurrentSpeed);
    }
}
