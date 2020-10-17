using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomUnitSelector : MonoBehaviour, IUnitSelector
{
    public GameObject SelectUnit(IEnumerable<GameObject> options)
    {
        var array = options.ToArray();
        return array.Length > 0 ? array[Random.Range(0, array.Length)] : null;
    }
}
