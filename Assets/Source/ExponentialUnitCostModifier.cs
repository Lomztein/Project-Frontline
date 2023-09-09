using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class ExponentialUnitCostModifier : MonoBehaviour, IUnitCostModifier
{
    public float Coeffecient;

    public string GetDescription(int cost, Unit unit, Commander commander)
    {
        int count = commander.AliveAll.Count(x => x.Name == unit.Name);
        if (count > 0)
        {
            string factor = Mathf.Pow(Coeffecient, count).ToString("P0", CultureInfo.InvariantCulture).Replace(" ", "");
            return $"{count} {unit.Name}(s): Cost x{factor}";
        }
        return null;
    }


    public int Modify(int cost, Unit unit, Commander commander)
    {
        int count = commander.AliveAll.Count(x => x.Name == unit.Name);
        return Mathf.RoundToInt(cost * Mathf.Pow(Coeffecient, count));
    }
}
