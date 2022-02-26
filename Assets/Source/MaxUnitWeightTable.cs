using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Max Weight Table", menuName = "Unit Weight Tables/Max")]
public class MaxUnitWeightTable : AggregateUnitWeightTable
{
    protected override float Aggregate(float aggregator, float value)
        => Mathf.Max(aggregator, value);
}
