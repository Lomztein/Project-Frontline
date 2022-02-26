using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitWeightTableBase : ScriptableObject
{
    public abstract Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options);

    public abstract void Initialize(Commander commander, IEnumerable<GameObject> availableUnits);

    public abstract UnitWeightTableBase DeepCopy();
}
