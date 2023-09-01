using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Weight Table", menuName = "Unit Weight Tables/Defense")]
public class DefenseWeightTable : UnitGroupWeightTable
{
    public string[] DefenderTags = { "Defense", "Trap" };
    public Vector2 WeightMinMax = new Vector2(0.2f, 0.8f);
    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        float factor = Mathf.Lerp(WeightMinMax.x, WeightMinMax.y, Mathf.Clamp01(Commander.DefenseFactor));

        var weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            var unit = option.GetComponent<Unit>();
            if (unit.Info.UnitType == UnitInfo.Type.Defense || DefenderTags.Any(x => unit.Info.Tags.Contains(x)))
                weights.Add(option, factor);
            else
                weights.Add(option, GetOtherWeight(factor));
        }

        return weights;
    }
}
