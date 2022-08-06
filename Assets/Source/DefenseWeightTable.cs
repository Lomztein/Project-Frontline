using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Weight Table", menuName = "Unit Weight Tables/Defense")]
public class DefenseWeightTable : UnitGroupWeightTable
{
    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        float factor = Mathf.Clamp01(Commander.DefenseFactor);

        var weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            var unit = option.GetComponent<Unit>();
            if (unit.Info.UnitType == UnitInfo.Type.Defense || unit.Info.Tags.Contains("Defense"))
                weights.Add(option, factor);
            else
                weights.Add(option, GetOtherWeight(factor));
        }

        return weights;
    }
}
