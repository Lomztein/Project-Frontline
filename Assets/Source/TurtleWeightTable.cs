using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turtle Weight Table", menuName = "Unit Weight Tables/Turtle")]
public class TurtleWeightTable : UnitWeightTable
{
    public int StructuresPerDefenceStructure = 5;
    public float NonDefenseWeight = 0f;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        int defenseStructures = Commander.AlivePlaced.Count(x => x.Info.UnitType == UnitInfo.Type.Defense);
        int otherStructures = Commander.AlivePlaced.Count(x => x.Info.UnitType != UnitInfo.Type.Defense);
        float desiredDefenseStructures = (float)otherStructures / StructuresPerDefenceStructure;

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (GameObject obj in options)
        {
            Unit unit = obj.GetComponent<Unit>();
            float factor = Mathf.Pow(Mathf.Clamp01(defenseStructures / desiredDefenseStructures), 2f);
            if (unit.Info.UnitType == UnitInfo.Type.Defense)
                weights.Add(obj, 1f - factor);
            else
                weights.Add(obj, factor);
        }

        return weights;
    }
}
