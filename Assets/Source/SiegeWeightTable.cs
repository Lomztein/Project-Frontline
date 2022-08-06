using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Siege Weight Table", menuName = "Unit Weight Tables/Siege")]
public class SiegeWeightTable : UnitGroupWeightTable
{
    private const string SIEGE_TAG = "Siege";

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        float factor = Mathf.Clamp01(Commander.OffenseFactor);

        var weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            if (option.GetComponent<Unit>().Info.Tags.Contains(SIEGE_TAG))
                weights.Add(option, factor);
            else
                weights.Add(option, GetOtherWeight(factor));
        }

        return weights;
    }
}
