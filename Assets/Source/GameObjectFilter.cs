using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameObjectFilter : ScriptableObject
{
    public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> objects);

    public bool Check(GameObject single)
        => Filter(single.SingleObjectAsEnumerable()).FirstOrDefault() == single;
}

public abstract class UnitFilter : GameObjectFilter
{
    public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            Unit unit = obj.GetComponent<Unit>();
            if (unit && Check(unit))
            {
                yield return obj;
            }
        }
    }

    protected abstract bool Check(Unit unit);
}
