using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "One Minus Weight Table", menuName = "Unit Weight Tables/One Minus")]
public class OneMinusUnitWeightTable : UnitWeightTableBase
{
    public UnitWeightTableBase WeightTable;

    public override UnitWeightTableBase DeepCopy()
    {
        OneMinusUnitWeightTable copy = Instantiate(this);
        copy.WeightTable = WeightTable.DeepCopy();
        return copy;
    }

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        var oneminus = new Dictionary<GameObject, float>();
        var weights = WeightTable.GetWeights(options);
        foreach (var pair in weights)
        {
            oneminus.Add (pair.Key, 1 - pair.Value);
        }
        return oneminus;
    }

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
        => WeightTable.Initialize(commander, availableUnits);
}
