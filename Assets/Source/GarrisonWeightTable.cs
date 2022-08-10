using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Garrison Weight Table", menuName = "Unit Weight Tables/Garrison")]
public class GarrisonWeightTable : UnitWeightTable
{
    public int InfantryPerGarrisonUnit = 20;
    public float NonGarrisonWeight;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        int garrisonUnits = Commander.AlivePlaced.Count(x => IsGarrisonUnit(x));
        int infantryUnits = Commander.AlivePlaced.Count(x => IsInfantryUnit(x));
        float desiredGarrisonUnits = (float)infantryUnits / InfantryPerGarrisonUnit;

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (GameObject obj in options)
        {
            if (obj.GetComponentInChildren<InfantryGarrison>())
                weights.Add(obj, 1f - Mathf.Clamp01(garrisonUnits / desiredGarrisonUnits));
            else
                weights.Add(obj, NonGarrisonWeight);
        }

        return weights;
    }

    private bool IsGarrisonUnit (Unit unit)
    {
        if (unit.TryGetComponent(out UnitFactory factory))
        {
            return factory.UnitPrefab.GetComponentInChildren<InfantryGarrison>();
        }
        return unit.GetComponentInChildren<InfantryGarrison>();
    }

    private bool IsInfantryUnit (Unit unit)
    {
        if (unit.TryGetComponent(out UnitFactory factory))
        {
            return factory.UnitPrefab.GetComponent<Unit>().Info.UnitType == UnitInfo.Type.Infantry;
        }
        return false;
    }
}
