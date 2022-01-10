using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWeaponAnimator : MonoBehaviour
{
    public Weapon Weapon;
    public Animator Animator;

    public void Start()
    {
        Weapon.OnFire += OnFire;
    }

    private void OnFire(IWeapon weapon)
    {
        Animator.SetTrigger("Fire");
    }
}
