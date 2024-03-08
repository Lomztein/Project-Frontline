using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeGameObjectFilter : GameObjectFilter
{
    public List<GameObjectFilter> Filters = new List<GameObjectFilter>();

    public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objects)
    {
        if (Filters.Count == 0)
            return objects;
        return Filters.SelectMany(x => x.Filter(objects));
    }

    public static CompositeGameObjectFilter Create(IEnumerable<GameObjectFilter> filters)
    {
        var filter = CreateInstance<CompositeGameObjectFilter>();
        filter.Filters = filters.ToList();
        return filter;
    }
}
