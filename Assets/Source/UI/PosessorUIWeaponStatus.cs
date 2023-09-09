using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PosessorUIWeaponStatus : MonoBehaviour
{
    public Slider ReloadBar;

    private float _fireTime;
    private float _rechaberedTime;

    private Text AmmoText;
    IWeapon _weapon;

    public void Assign (IWeapon weapon, int index)
    {
        weapon.OnFire += Weapon_OnFire;
        _fireTime = 0f;
        _rechaberedTime = 1f;
        if (index < 2)
            AmmoText = PosessorUI.Instance.WeaponAmmoTexts[index];
        _weapon = weapon;
    }
    private void FixedUpdate()
    {
        ReloadBar.value = Mathf.InverseLerp(_fireTime, _rechaberedTime, Time.time);
        if (AmmoText)
            AmmoText.text = $"{_weapon.Ammo} / {_weapon.MaxAmmo}";
    }

    private void Weapon_OnFire(IWeapon obj)
    {
        if (Time.time < _fireTime + 0.1f || Time.time > _rechaberedTime - 0.1f)
        {
            _fireTime = Time.time;
            _rechaberedTime = _fireTime + (1f / obj.Firerate);
        }
    }
}
