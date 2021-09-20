using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChargingWeaponAnimator : MonoBehaviour
{
    public ChargingWeapon Weapon;
    public Renderer Renderer;
    public string ColorProperty;
    public Color EmissionColor;
    public float MaxIntensity;

    private Material _material;
    private Vector4 _color;

    private void Start()
    {
        _material = Instantiate(Renderer.material);
        Renderer.material = _material;
        _color = new Vector4(EmissionColor.r, EmissionColor.g, EmissionColor.b, EmissionColor.a);
    }

    private void FixedUpdate()
    {
        float factor = Weapon.CurrentChargeTime / Weapon.MaxChargeTime;
        _material.SetColor(ColorProperty, _color * MaxIntensity * factor);
    }
}
