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

    public void Assign (IWeapon weapon)
    {
        weapon.OnFire += Weapon_OnFire;
        _fireTime = 0f;
        _rechaberedTime = 1f;
    }

    private void FixedUpdate()
    {
        ReloadBar.value = Mathf.InverseLerp(_fireTime, _rechaberedTime, Time.time);
    }

    private void Weapon_OnFire(IWeapon obj)
    {
        _fireTime = Time.time;
        _rechaberedTime = _fireTime + (1f / obj.Firerate);
    }
}
