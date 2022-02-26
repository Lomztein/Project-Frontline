using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Composite Weight Table", menuName = "Unit Weight Tables/Composite")]
public class CompositeUnitWeightTable : AggregateUnitWeightTable
{
    protected override float Aggregate(float aggregator, float value)
        => aggregator * value;
}
