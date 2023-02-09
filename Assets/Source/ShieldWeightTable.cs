using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

[CreateAssetMenu(fileName = "New Shield Weight Table", menuName = "Unit Weight Tables/Shield")]
public class ShieldWeightTable : UnitWeightTable
{
    public float NonShieldWeight;

    public float TargetCoverage = 1.2f;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        var projectors = Commander.AlivePlaced.Select(x => ShieldUtils.GetShieldInObj(x.gameObject)).Where(x => x != null);
        int covers = Commander.AlivePlaced.Sum(x => GetNumCovers(x, projectors));
        float coverage = (float)covers / Commander.AlivePlaced.Count();

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            Transform shieldInOption = ShieldUtils.GetShieldInObj(option);
            if (shieldInOption != null)
            {
                weights.Add(option, 1f - Mathf.Clamp01(coverage / TargetCoverage));
            }
            else
            {
                weights.Add(option, NonShieldWeight);
            }
        }

        return weights;
    }

    private int GetNumCovers (Unit obj, IEnumerable<Transform> projectors)
    {
        int num = projectors.Count(y => Vector3.SqrMagnitude(obj.transform.position - y.transform.position) < Mathf.Pow(ShieldUtils.ComputeShieldRadius(y), 2f));
        return num;
    }
}
