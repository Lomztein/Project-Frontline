using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingWeapon : Weapon
{
    public float MaxChargeTime;
    public float CurrentChargeTime;
    public bool ResetChargeOnFire;
    private bool _isCharging = false;

    public override bool TryFire(ITarget intendedTarget)
    {
        if (base.TryFire(intendedTarget))
        {
            if (ResetChargeOnFire)
            {
                CurrentChargeTime = 0f;
            }
            _isCharging = true;
            return true;
        }
        _isCharging = true;
        return false;

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
