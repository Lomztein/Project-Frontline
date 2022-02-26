using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Min Weight Table", menuName = "Unit Weight Tables/Min")]
public class MinUnitWeightTable : AggregateUnitWeightTable
{
    protected override float Aggregate(float aggregator, float value)
        => Mathf.Min(aggregator, value);
}
