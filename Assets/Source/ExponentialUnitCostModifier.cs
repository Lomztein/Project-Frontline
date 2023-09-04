using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExponentialUnitCostModifier : MonoBehaviour, IUnitCostModifier
{
    public float Coeffecient;

    public int Modify(int cost, Unit unit, Commander commander)
    {
        int count = commander.AliveAll.Count(x => x.Name == unit.Name);
        return Mathf.RoundToInt(cost * Mathf.Pow(Coeffecient, count));
    }
}
