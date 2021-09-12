using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitWeightTableBase : ScriptableObject
{
    public abstract float GetWeight(string unitName);
}
