using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchProductionTimeUnitCostModifier : IUnitCostModifier
{
    public float ProductionTime;

    public MatchProductionTimeUnitCostModifier(float productionTime)
    {
        ProductionTime = productionTime;
    }

    public string GetDescription(int cost, Unit unit, Commander commander)
    {
        return string.Empty;
    }

    public int Modify(int cost, Unit unit, Commander commander)
    {
        if (unit.TryGetComponent<ProductionInfo>(out var info))
        {
            float ratio = info.ProductionTime / ProductionTime;
            return Mathf.RoundToInt(ratio *= cost);
        }
        return cost;
    }
}
