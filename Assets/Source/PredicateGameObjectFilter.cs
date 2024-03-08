using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PredicateGameObjectFilter : GameObjectFilter
{
    public Predicate<GameObject> Predicate;

    public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objects)
        => objects.Where(x =>  Predicate(x));

    public static PredicateGameObjectFilter Create(Predicate<GameObject> predicate)
    {
        var filter = CreateInstance<PredicateGameObjectFilter>();
        filter.Predicate = predicate;
        return filter;
    }
}
