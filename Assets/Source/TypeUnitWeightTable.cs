using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Type Weight Table", menuName = "Unit Weight Tables/Type")]
public class TypeUnitWeightTable : UnitWeightTable
{
    public TypeWeight[] Weights;

    public override void Initialize(Commander commander, IEnumerable<GameObject> options)
    {
        foreach (GameObject go in options)
        {
            Unit unit = go.GetComponent<Unit>();
            SetWeight(go, Weights.FirstOrDefault(x => x.Type == unit.Info.UnitType)?.GetWeight() ?? 0);
        }
    }

    [System.Serializable]
    public class TypeWeight
    {
        public UnitInfo.Type Type;
        public Vector2 WeightMinMax = Vector2.one;

        public float GetWeight() => Random.Range(WeightMinMax.x, WeightMinMax.y);
    }
}
