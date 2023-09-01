using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Garrison Weight Table", menuName = "Unit Weight Tables/Garrison")]
public class GarrisonWeightTable : UnitWeightTable
{
    public int InfantryUnitsPerSlot = 6;
    public float NonGarrisonWeight;
    public int Margin = 6;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        int garrisonSlots = Commander.AlivePlaced.Sum(x => GetGarrisonSlotCount(x));
        int infantryUnits = Commander.AlivePlaced.Count(x => IsInfantryUnit(x));

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (GameObject obj in options)
        {
            var garrison = obj.GetComponentInChildren<InfantryGarrison>();
            if (garrison != null)
                weights.Add(obj, CalculateDesire(garrisonSlots, infantryUnits, 1f / InfantryUnitsPerSlot, Margin, Margin));
            else
                weights.Add(obj, NonGarrisonWeight);
        }

        return weights;
    }

    private int GetGarrisonSlotCount (Unit unit)
    {
        if (unit.TryGetComponent(out UnitFactory factory))
        {
            return GetGarrisonSlotCount(factory.UnitPrefab.GetComponent<Unit>());
        }
        InfantryGarrison garrison = unit.GetComponentInChildren<InfantryGarrison>();
        if (garrison != null)
        {
            if (unit.CompareTag("StructureUnit"))
            {
                return garrison.SlotCount / 2; // Stationary units such as bunkers shouldn't count for as much.
            }
            return garrison.SlotCount;
        }
        return 0;
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
