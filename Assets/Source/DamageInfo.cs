using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DamageInfo
{
    public object Source;
    public object Target;

    public float Damage;
    public float BaseDamage;
    public DamageMatrix.Damage Type;
    public Vector3 Point;
    public Vector3 Direction;
    public float DamageDone;
    public bool KilledTarget;

    public DamageInfo(float damage, DamageMatrix.Damage type, Vector3 point, Vector3 direction, object source, object target)
    {
        Source = source;
        Target = target;

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

    public T SourceAs<T>() where T : class => Source as T;
    public T TargetAs<T>() where T : class => Target as T;

    public float ComputeDamage(ArmorType target)
    {
        float damage = Damage;
        DamageType type = null;
        damage = type.ModifyDamage(damage, this, target);
        damage = target.MitigateDamage(damage, this, type);
        return damage;
    }
}
