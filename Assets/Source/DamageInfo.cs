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
    public DamageModifier Modifier;
    public Vector3 Point;
    public Vector3 Direction;
    public float DamageDone;
    public bool KilledTarget;

    public DamageInfo(float damage, DamageModifier modifier, Vector3 point, Vector3 direction, object source, object target)
    {
        Source = source;
        Target = target;

        Damage = damage;
        BaseDamage = damage;
        Modifier = modifier;
        Point = point;
        Direction = direction;
    }

    public void SetRecieverInfo(float damageDone, bool killed)
    {
        DamageDone = damageDone;
        KilledTarget = killed;
    }

    public float GetDamage(DamageModifier target)
        => Damage * DamageModifier.Combine(target, Modifier);

    public T SourceAs<T>() where T : class => Source as T;
    public T TargetAs<T>() where T : class => Target as T;
}
