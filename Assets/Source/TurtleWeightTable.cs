using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turtle Weight Table", menuName = "Unit Weight Tables/Turtle")]
public class TurtleWeightTable : UnitGroupWeightTable
{
    public int StructuresPerDefenceStructure = 5;
    public int Margin;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        int defenseStructures = Commander.AlivePlaced.Count(x => x.Info.UnitType == UnitInfo.Type.Defense);
        int otherStructures = Commander.AlivePlaced.Count(x => x.Info.UnitType != UnitInfo.Type.Defense);
        float desiredDefenseStructures = (float)otherStructures / StructuresPerDefenceStructure;

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (GameObject obj in options)
        {
            Unit unit = obj.GetComponent<Unit>();
            float factor = CalculateDesire(defenseStructures, otherStructures, 1f / StructuresPerDefenceStructure, Margin);

            if (unit.Info.UnitType == UnitInfo.Type.Defense)
                weights.Add(obj, factor);
            else
                weights.Add(obj, GetOtherWeight(factor));
        }

        return weights;
    }
}
