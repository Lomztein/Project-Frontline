﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingWeapon : Weapon
{
    public float MaxChargeTime;
    public float CurrentChargeTime;
    public bool ResetChargeOnFire;
    public bool ChargeOnlyWhenCanFire;
    private bool _isCharging = false;

    public override bool TryFire(ITarget intendedTarget)
    {
        if (base.CanFire())
        {
            if (CanFire())
            {
                if (base.TryFire(intendedTarget))
                {
                    if (ResetChargeOnFire)
                    {
                        CurrentChargeTime = 0f;
                    }
                    return true;
                }
            }
            _isCharging = true;
            return false;
        }
        else
        {
            _isCharging = !ChargeOnlyWhenCanFire;
            return false;
        }
    }

    public override bool CanFire()
    {
        return base.CanFire() && (CurrentChargeTime >= MaxChargeTime);
    }

    public override float GetDPS()
    {
        if (ResetChargeOnFire)
        {
            return Damage / (1 / Firerate + MaxChargeTime);
        }
        else
        {
            return base.GetDPS();
        }
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
