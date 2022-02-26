using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Tier Weight Tables", menuName ="Unit Weight Tables/Tier")]
public class TierUnitWeightTable : UnitWeightTable
{
    public TierWeight[] Weights;

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
        foreach (GameObject go in availableUnits)
        {
            Unit unit = go.GetComponent<Unit>();
            SetWeight(go, Weights.FirstOrDefault(x => x.Tier == unit.Info.UnitTier)?.Weight ?? 0);
        }
    }

    [System.Serializable]
    public class TierWeight
    {
        public UnitInfo.Tier Tier;
        public float Weight;
    }
}
