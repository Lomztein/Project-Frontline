using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingWeapon : Weapon
{
    public float MaxChargeTime;
    public float CurrentChargeTime;
    private bool _isCharging = false;

    public override void TryFire(ITarget intendedTarget)
    {
        base.TryFire(intendedTarget);
        _isCharging = true;
    }

    public override bool CanFire()
    {
        return base.CanFire() && (CurrentChargeTime >= MaxChargeTime);
    }

    private void FixedUpdate()
    {
        if (_isCharging)
        {
            CurrentChargeTime += Time.fixedDeltaTime;
            _isCharging = false;

            if (CurrentChargeTime >= MaxChargeTime)
            {
                CurrentChargeTime = MaxChargeTime;
            }
        }
        else
        {
            CurrentChargeTime -= Time.fixedDeltaTime;
            if (CurrentChargeTime <= 0f)
            {
                CurrentChargeTime = 0f;
            }
        }
    }
}
