using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Tier Weight Tables", menuName ="Unit Weight Tables/Tier")]
public class TierUnitWeightTable : UnitWeightTable
{
    public TierWeight[] Weights;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        var results = new Dictionary<GameObject, float>();
        foreach (GameObject go in options)
        {
            Unit unit = go.GetComponent<Unit>();
            results.Add (go, Weights.FirstOrDefault(x => x.Tier == unit.Info.UnitTier)?.Weight ?? 0);
        }
        return results;
    }

    [System.Serializable]
    public class TierWeight
    {
        public UnitInfo.Tier Tier;
        public float Weight;
    }
}
