using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorChargingWeaponAnimator : MonoBehaviour
{
    public ChargingWeapon Weapon;
    public Renderer Renderer;
    public string GlowProperty;
    public AnimationCurve Remap;
    public float LerpRate = 50;
    public float GlowTimeAfterFiring;

    private float _lastFireTime;
    private Material _material;
    private float _factor;

    private void Start()
    {
        Weapon.OnFire += Weapon_OnFire;
        _material = Instantiate(Renderer.material);
        Renderer.material = _material;
        _lastFireTime = float.MinValue;
    }

    private void Weapon_OnFire(IWeapon obj)
    {
        _lastFireTime = Time.time;
    }

    private void FixedUpdate()
    {
        float bonus = 0f;
        if ((_lastFireTime + GlowTimeAfterFiring) - Time.time > 0f) 
        {
            bonus = 1f;
        }

        _factor = Mathf.Lerp(_factor, (Weapon.CurrentChargeTime / Weapon.MaxChargeTime) + bonus, LerpRate * Time.fixedDeltaTime);
        _material.SetFloat(GlowProperty, Remap.Evaluate(Mathf.Clamp01(_factor)));
    }
}
