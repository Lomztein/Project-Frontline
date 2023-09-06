﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DamageInfo
{
    public float Damage;
    public float BaseDamage;
    public DamageMatrix.Damage Type;
    public Vector3 Point;
    public Vector3 Direction;
    public float DamageDone;
    public bool KilledTarget;

    public DamageInfo(float damage, DamageMatrix.Damage type, Vector3 point, Vector3 direction)
    {
        Damage = damage;
        BaseDamage = damage;
        Type = type;
        Point = point;
        Direction = direction;
    }

    public void SetRecieverInfo(float damageDone, bool killed)
    {
        DamageDone = damageDone;
        KilledTarget = killed;
    }
}
