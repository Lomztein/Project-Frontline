using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DamageInfo
{
    public float Damage;
    public DamageMatrix.Damage Type;
    public Vector3 Point;
    public Vector3 Direction;

    public DamageInfo(float damage, DamageMatrix.Damage type, Vector3 point, Vector3 direction)
    {
        Damage = damage;
        Type = type;
        Point = point;
        Direction = direction;
    }
}
